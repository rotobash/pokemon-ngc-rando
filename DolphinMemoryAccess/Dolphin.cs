using Reloaded.Memory;
using Reloaded.Memory.Interfaces;
using Reloaded.Memory.Streams;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Buffers.Binary;

namespace DolphinMemoryAccess
{
    /// <summary>
    /// Stolen with <3 from https://github.com/Sewer56/Dolphin.Memory.Access 
    /// Modified to control the lifecycle of the process (i.e. starting and stoppping) and to add convienence methods for manipulating memory.
    /// Perhaps I should fork it and add it as a submodule to be respectful, but I'm hacking this to work right now so I will do it later.
    /// </summary>
    public class Dolphin
    {
        private const long EmulatedMemorySize = 0x2000000;
        public const long EmulatedMemoryBase = 0x80000000;

        private ICanReadWriteMemory? memoryAccessor;
        Process dolphinProcess;
        private IntPtr pointerToEmulatedMemory = IntPtr.Zero;
        private IntPtr dolphinBaseAddress;
        private IntPtr dolphinEmulatedBaseAddress;
        private int dolphinModuleSize;
        BigEndianReader reader;

        public ushort GameTitleCode
        {
            get 
            {
                if (IsRunning) 
                {
                    var data = ReadData(EmulatedMemoryBase + 2, 2);
                    return data.Length > 0 ? BinaryPrimitives.ReadUInt16BigEndian(data) : (byte)0;
                }
                return 0; 
            }
        }

        public byte GameRegionCode 
        {
            get 
            {
                if (IsRunning) 
                {
                    var data = ReadData(EmulatedMemoryBase + 3, 1);
                    return data.Length > 0 ? data[0] : (byte)0;
                }
                return 0; 
            }
        }

        public bool IsRunning => dolphinProcess != null && !dolphinProcess.HasExited;

        public Dolphin(string dolphinPath)
        {
            dolphinProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = dolphinPath,
                    WorkingDirectory = Path.GetDirectoryName(dolphinPath),
                    UseShellExecute = true,
                }
            };
        }

        private void SetDolphinStartArguments(string gamePath)
        {
            dolphinProcess.StartInfo.Arguments = $"-b -e \"{gamePath}\"";
        }

        /// <summary>
        /// Tries to retrieve the base address of GameCube emulated memory.
        /// </summary>
        /// <returns></returns>
        public unsafe bool TryGetBaseAddress(out IntPtr emulatedBaseAddress)
        {
            if (memoryAccessor is null)
                throw new AccessViolationException("Dolphin process has not been setup.");

            // Check cached page address for potential valid page.
            if (pointerToEmulatedMemory != IntPtr.Zero)
            {
                memoryAccessor.Read((nuint)pointerToEmulatedMemory.ToInt64(), out emulatedBaseAddress);
                return emulatedBaseAddress != IntPtr.Zero; // If it's zero, a game is not running.
            }

            if (TryGetDolphinPage(out emulatedBaseAddress))
            {
                Span<byte> dolphinMainModule = new byte[dolphinModuleSize];

                // Find pointer to emulated memory.
                memoryAccessor.ReadRaw((nuint)dolphinBaseAddress.ToInt64(), dolphinMainModule);
                long readCount = dolphinModuleSize - sizeof(IntPtr);

                fixed (byte* mainModulePtr = dolphinMainModule)
                {
                    var lastAddress = (long)mainModulePtr + readCount;
                    var currentAddress = (long)mainModulePtr;

                    while (currentAddress < lastAddress)
                    {
                        var current = *(IntPtr*)currentAddress;
                        if (current == emulatedBaseAddress)
                        {
                            var offset = currentAddress - (long)mainModulePtr;
                            pointerToEmulatedMemory = (IntPtr)((long)dolphinBaseAddress + offset);
                            return true;
                        }

                        currentAddress += 1;
                    }
                }

                // Pointer not found but memory was found.
                // Suspicious but I'll allow it.
                return true;
            }

            // Not found.
            emulatedBaseAddress = IntPtr.Zero;
            return false;
        }

        /// <summary>
        /// Attempts to get the memory page belonging to Dolphin emulator, spitting out the address of emulated memory if it succeeds.
        /// </summary>
        private bool TryGetDolphinPage(out IntPtr baseAddress)
        {
            // Otherwise enumerate remaining pages.
            var enumerator = new MemoryPageEnumerator(dolphinProcess);
            while (enumerator.MoveNext())
            {
                var page = enumerator.Current;
                if (IsDolphinPage(page))
                {
                    baseAddress = page.BaseAddress;
                    return true;
                }
            }

            baseAddress = IntPtr.Zero;
            return false;
        }

        /// <summary>
        /// Verifies whether a given memory page belongs to Dolphin.
        /// </summary>
        private unsafe bool IsDolphinPage(Native.MEMORY_BASIC_INFORMATION memoryPage)
        {
            // Check if page mapped and right size.
            if (memoryPage.RegionSize == (IntPtr)EmulatedMemorySize && memoryPage.lType == Native.PageType.Mapped)
            {
                // Note taken from from Dolphin Memory Engine:

                /*
                    Here, it's likely the right page, but it can happen that multiple pages with these criteria
                    exist and have nothing to do with the emulated memory. Only the right page has valid
                    working set information so an additional check is required that it is backed by physical
                    memory.
                */

                var setInformation = new Native.PSAPI_WORKING_SET_EX_INFORMATION[1];
                setInformation[0].VirtualAddress = memoryPage.BaseAddress;

                if (!Native.QueryWorkingSetEx(dolphinProcess.Handle, setInformation, sizeof(Native.PSAPI_WORKING_SET_EX_INFORMATION) * setInformation.Length))
                    return false;

                if (setInformation[0].VirtualAttributes.Valid)
                    return true;
            }

            return false;
        }

        public async Task<bool> SetupDolphin(string gamePath)
        {
            SetDolphinStartArguments(gamePath);
            dolphinProcess.Start();
            await Task.Delay(5000).ConfigureAwait(false);

            if (IsRunning)
            {
                var mainModule = dolphinProcess.MainModule;
                dolphinBaseAddress = mainModule.BaseAddress;
                dolphinModuleSize = mainModule.ModuleMemorySize;
                memoryAccessor = new ExternalMemory(dolphinProcess);

                var counter = 0;
                while (counter <= 5)
                {
                    try
                    {
                        if (TryGetBaseAddress(out dolphinEmulatedBaseAddress))
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        await Task.Delay(5000).ConfigureAwait(false);
                        if (counter == 5)
                        {
                            throw;
                        }
                    }
                    counter++;
                }
            }
            return false;
        }

        private long TranslateAddressToOffset(long dolphinAddress)
        {
            return dolphinAddress > EmulatedMemoryBase ? dolphinAddress - EmulatedMemoryBase : dolphinAddress;
        }

        public byte[] GuardedReadData(long guardDolphinAddress, int guardLength, byte[] guardValue, long dolphinAddress, int size)
        {
            if (memoryAccessor is null)
                throw new AccessViolationException("Dolphin process has not been setup.");

            var translatedGuardAddress = TranslateAddressToOffset(guardDolphinAddress);
            var guard = memoryAccessor.ReadRaw((nuint)(dolphinEmulatedBaseAddress.ToInt64() + translatedGuardAddress), guardLength);
            if (guard.SequenceEqual(guardValue))
                return ReadData(dolphinAddress, size);

            return Array.Empty<byte>();
        }

        public byte[] ReadData(long dolphinAddress, int size)
        {
            if (memoryAccessor is null)
                throw new AccessViolationException("Dolphin process has not been setup.");

            try
            {
                var translatedAddress = TranslateAddressToOffset(dolphinAddress);
                var buffer = memoryAccessor.ReadRaw((nuint)(dolphinEmulatedBaseAddress.ToInt64() + translatedAddress), size);

                return buffer;
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public void WriteData(long dolphinAddress, byte[] buffer)
        {
            if (memoryAccessor is null)
                throw new AccessViolationException("Dolphin process has not been setup.");

            try
            {
                var translatedAddress = TranslateAddressToOffset(dolphinAddress);
                memoryAccessor.WriteRaw((nuint)(dolphinEmulatedBaseAddress.ToInt64() + translatedAddress), buffer);
            }
            catch
            {
                // ignored
            }
        }

        public void GuardedWriteData(long guardDolphinAddress, int guardLength, byte[] guardValue, long dolphinAddress, byte[] buffer)
        {
            if (memoryAccessor is null)
                throw new AccessViolationException("Dolphin process has not been setup.");

            var guard = memoryAccessor.ReadRaw((nuint)(dolphinEmulatedBaseAddress.ToInt64() + guardDolphinAddress), guardLength);
            if (guard.SequenceEqual(guardValue))
                WriteData(dolphinAddress, buffer);
        }

        public Dictionary<long, byte[]> ReadDataBatch(Dictionary<long, int> dolphinAddresses)
        {
            var result = new Dictionary<long, byte[]>();
            foreach (var dolphinAddress in dolphinAddresses)
            {
                result.Add(dolphinAddress.Key, ReadData(dolphinAddress.Key, dolphinAddress.Value));
            }
            return result;
        }

        public void WriteDataBatch(Dictionary<long, byte[]> dataBatch)
        {
            foreach (var data in dataBatch)
            {
                WriteData(data.Key, data.Value);
            }
        }

        public void CloseDolphinProcess()
        {
            if (IsRunning)
                dolphinProcess.CloseMainWindow();
        }
    }
}
