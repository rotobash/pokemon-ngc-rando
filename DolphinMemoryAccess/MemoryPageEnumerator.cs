using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DolphinMemoryAccess.Native;

namespace DolphinMemoryAccess
{
    /// <summary>
    /// Simple enumerator that enumerates the pages of a given process' memory.
    /// </summary>
    public class MemoryPageEnumerator : IEnumerator<MEMORY_BASIC_INFORMATION>
    {
        public MEMORY_BASIC_INFORMATION Current { get; private set; }
        object IEnumerator.Current => Current;

        private Process _process;

        private ulong _currentAddress = 0;
        private ulong _maxAddress = 0x7FFFFFFF; // 32bit

        /* Setup */
        public MemoryPageEnumerator(Process process)
        {
            _process = process;

            IsWow64Process(process.Handle, out bool isWow64); // Check if this is an x86 application running on x64.
            GetSystemInfo(out SYSTEM_INFO systemInfo);

            // Max address defaults to 32bit maximum. If this is a 64bit process, switch to 64bit max address.
            _maxAddress = (ulong)systemInfo.lpMaximumApplicationAddress;
        }

        /* Interface Implementation */
        public unsafe bool MoveNext()
        {
            if (_currentAddress > _maxAddress)
                return false;

            // Get our info from VirtualQueryEx.
            VirtualQueryEx(_process.Handle, (IntPtr)_currentAddress, out var memoryInformation, (uint)sizeof(MEMORY_BASIC_INFORMATION));

            // Add the page and increment address iterator to go to next page.
            Current = memoryInformation;
            _currentAddress += (ulong)memoryInformation.RegionSize;

            return true;
        }

        public void Reset() => _currentAddress = 0;
        public void Dispose() { }
    }
}
