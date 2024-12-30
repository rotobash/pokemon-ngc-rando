using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DolphinMemoryAccess
{
    public static class Native
    {
        /// <summary>
        /// Retrieves information about a range of pages within the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process whose memory information is queried.
        ///     The handle must have been opened with the PROCESS_QUERY_INFORMATION access right,
        ///     which enables using the handle to read information from the process object.
        /// </param>
        /// <param name="lpAddress">
        ///     A pointer to the base address of the region of pages to be queried.
        ///     This value is rounded down to the next page boundary.
        ///     To determine the size of a page on the host computer, use the GetSystemInfo function.
        /// </param>
        /// <param name="lpBuffer">
        ///     A pointer to a MEMORY_BASIC_INFORMATION structure in which information about the specified page range is returned.
        /// </param>
        /// <param name="dwLength">
        ///     The size of the buffer pointed to by the lpBuffer parameter, in bytes.
        /// </param>
        /// <returns></returns>

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("psapi", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryWorkingSetEx(IntPtr hProcess, [In, Out] PSAPI_WORKING_SET_EX_INFORMATION[] pv, int cb);

        /// <summary>Determines whether the specified process is running under WOW64.</summary>
        /// <param name="hProcess">
        /// <para>
        /// A handle to the process. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right. For more information,
        /// see Process Security and Access Rights.
        /// </para>
        /// <para><c>Windows Server 2003 and Windows XP:</c> The handle must have the PROCESS_QUERY_INFORMATION access right.</para>
        /// </param>
        /// <param name="Wow64Process">
        /// A pointer to a value that is set to TRUE if the process is running under WOW64. If the process is running under 32-bit Windows, the value is set to
        /// FALSE. If the process is a 64-bit application running under 64-bit Windows, the value is also set to FALSE.
        /// </param>
        /// <returns>
        /// <para>If the function succeeds, the return value is a nonzero value.</para>
        /// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool Wow64Process);

        /// <summary>
        /// <para>Retrieves information about the current system.</para>
        /// <para>To retrieve accurate information for an application running on WOW64, call the <c>GetNativeSystemInfo</c> function.</para>
        /// </summary>
        /// <param name="lpSystemInfo">A pointer to a <c>SYSTEM_INFO</c> structure that receives the information.</param>
        /// <returns>This function does not return a value.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        /// <summary>
        /// Contains information about the current computer system. This includes the architecture and type of the processor, the number of
        /// processors in the system, the page size, and other such information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct SYSTEM_INFO
        {
            /// <summary>
            /// <para>The processor architecture of the installed operating system. This member can be one of the following values.</para>
            /// <para>
            /// <list type="table">
            /// <listheader>
            /// <term>Value</term>
            /// <term>Meaning</term>
            /// </listheader>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_AMD649</term>
            /// <term>x64 (AMD or Intel)</term>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_ARM5</term>
            /// <term>ARM</term>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_ARM6412</term>
            /// <term>ARM64</term>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_IA646</term>
            /// <term>Intel Itanium-based</term>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_INTEL0</term>
            /// <term>x86</term>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_UNKNOWN0xffff</term>
            /// <term>Unknown architecture.</term>
            /// </item>
            /// </list>
            /// </para>
            /// </summary>
            public ProcessorArchitecture wProcessorArchitecture;

            /// <summary>This member is reserved for future use.</summary>
            public ushort wReserved;
            /// <summary>
            /// The page size and the granularity of page protection and commitment. This is the page size used by the <c>VirtualAlloc</c> function.
            /// </summary>
            public uint dwPageSize;

            /// <summary>A pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs).</summary>
            public IntPtr lpMinimumApplicationAddress;

            /// <summary>A pointer to the highest memory address accessible to applications and DLLs.</summary>
            public IntPtr lpMaximumApplicationAddress;

            /// <summary>
            /// A mask representing the set of processors configured into the system. Bit 0 is processor 0; bit 31 is processor 31.
            /// </summary>
            public UIntPtr dwActiveProcessorMask;

            /// <summary>
            /// The number of logical processors in the current group. To retrieve this value, use the <c>GetLogicalProcessorInformation</c> function.
            /// </summary>
            public uint dwNumberOfProcessors;

            /// <summary>
            /// An obsolete member that is retained for compatibility. Use the <c>wProcessorArchitecture</c>, <c>wProcessorLevel</c>, and
            /// <c>wProcessorRevision</c> members to determine the type of processor.
            /// </summary>
            public uint dwProcessorType;

            /// <summary>
            /// The granularity for the starting address at which virtual memory can be allocated. For more information, see <c>VirtualAlloc</c>.
            /// </summary>
            public uint dwAllocationGranularity;

            /// <summary>
            /// <para>
            /// The architecture-dependent processor level. It should be used only for display purposes. To determine the feature set of a
            /// processor, use the <c>IsProcessorFeaturePresent</c> function.
            /// </para>
            /// <para>If <c>wProcessorArchitecture</c> is PROCESSOR_ARCHITECTURE_INTEL, <c>wProcessorLevel</c> is defined by the CPU vendor.</para>
            /// <para>If <c>wProcessorArchitecture</c> is PROCESSOR_ARCHITECTURE_IA64, <c>wProcessorLevel</c> is set to 1.</para>
            /// </summary>
            public ushort wProcessorLevel;

            /// <summary>
            /// <para>
            /// The architecture-dependent processor revision. The following table shows how the revision value is assembled for each type of
            /// processor architecture.
            /// </para>
            /// <para>
            /// <list type="table">
            /// <listheader>
            /// <term>Processor</term>
            /// <term>Value</term>
            /// </listheader>
            /// <item>
            /// <term>Intel Pentium, Cyrix, or NextGen 586</term>
            /// <term>
            /// The high byte is the model and the low byte is the stepping. For example, if the value is xxyy, the model number and stepping
            /// can be displayed as
            /// follows: Model xx, Stepping yy
            /// </term>
            /// </item>
            /// <item>
            /// <term>Intel 80386 or 80486</term>
            /// <term>
            /// A value of the form xxyz. If xx is equal to 0xFF, y - 0xA is the model number, and z is the stepping identifier.If xx is not
            /// equal to 0xFF, xx + 'A' is the stepping letter and yz is the minor stepping.
            /// </term>
            /// </item>
            /// <item>
            /// <term>ARM</term>
            /// <term>Reserved.</term>
            /// </item>
            /// </list>
            /// </para>
            /// </summary>
            public ushort wProcessorRevision;
        }

        /// <summary>
        /// Contains information about a range of pages in the virtual address space of a process.
        /// The VirtualQuery and VirtualQueryEx functions use this structure.
        /// </summary>
        public struct MEMORY_BASIC_INFORMATION
        {
            /// <summary>A pointer to the base address of the region of pages.</summary>
            public IntPtr BaseAddress;

            /// <summary>
            /// A pointer to the base address of a range of pages allocated by the VirtualAlloc function.
            /// The page pointed to by the BaseAddress member is contained within this allocation range.
            /// </summary>
            public IntPtr AllocationBase;

            /// <summary>
            /// The memory protection option when the region was initially allocated.
            /// This member can be one of the memory protection constants or 0 if the caller does not have access.
            /// </summary>
            public MemoryProtections AllocationProtect;

            /// <summary>
            /// The size of the region beginning at the base address in which all pages have identical attributes, in bytes.
            /// </summary>
            public IntPtr RegionSize;

            /// <summary>The state of the pages in the region.</summary>
            public PageState State;

            /// <summary>
            /// The access protection of the pages in the region.
            /// This member is one of the values listed for the AllocationProtect member.
            /// </summary>
            public MemoryProtections Protect;

            /// <summary>
            /// The type of pages in the region.
            /// The following types are defined.
            /// </summary>
            public PageType lType;
        }

        /// <summary>
        /// MemoryProtections
        ///     Specifies the memory protection constants for the region of pages
        ///     to be allocated, referenced or used for a similar purpose.
        ///     https://msdn.microsoft.com/en-us/library/windows/desktop/aa366786(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum MemoryProtections
        {
            Execute = 16, // 0x00000010
            ExecuteRead = 32, // 0x00000020
            ExecuteReadWrite = 64, // 0x00000040
            ExecuteWriteCopy = 128, // 0x00000080
            NoAccess = 1,
            ReadOnly = 2,
            ReadWrite = 4,
            WriteCopy = 8,
            GuardModifierflag = 256, // 0x00000100
            NoCacheModifierflag = 512, // 0x00000200
            WriteCombineModifierflag = 1024, // 0x00000400
        }

        /// <summary>Processor architecture</summary>
        public enum ProcessorArchitecture : ushort
        {
            /// <summary>x86</summary>
            PROCESSOR_ARCHITECTURE_INTEL = 0,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_MIPS = 1,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_ALPHA = 2,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_PPC = 3,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_SHX = 4,
            /// <summary>ARM</summary>
            PROCESSOR_ARCHITECTURE_ARM = 5,
            /// <summary>Intel Itanium-based</summary>
            PROCESSOR_ARCHITECTURE_IA64 = 6,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_ALPHA64 = 7,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_MSIL = 8,
            /// <summary>x64 (AMD or Intel)</summary>
            PROCESSOR_ARCHITECTURE_AMD64 = 9,
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_IA32_ON_WIN64 = 10, // 0x000A
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_NEUTRAL = 11, // 0x000B
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_ARM64 = 12, // 0x000C
            /// <summary>Unspecified</summary>
            PROCESSOR_ARCHITECTURE_ARM32_ON_WIN64 = 13, // 0x000D
            /// <summary>Unknown architecture.</summary>
            PROCESSOR_ARCHITECTURE_UNKNOWN = 65535, // 0xFFFF
        }

        /// <summary>
        /// PageState
        ///     The state of the pages in the region. This member can be one of the following values.
        ///     https://msdn.microsoft.com/en-us/library/windows/desktop/aa366775(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum PageState
        {
            /// <summary>
            /// Indicates committed pages for which physical storage has been allocated, either in memory or in the paging file on disk.
            /// </summary>
            Commit = 4096, // 0x00001000

            /// <summary>
            /// Indicates free pages not accessible to the calling process and available to be allocated.
            /// For free pages, the information in the AllocationBase, AllocationProtect, Protect, and Type members is undefined.
            /// </summary>
            Free = 65536, // 0x00010000

            /// <summary>
            /// Indicates reserved pages where a range of the process's virtual address space is reserved without any physical storage being allocated.
            /// For reserved pages, the information in the Protect member is undefined.
            /// </summary>
            Reserve = 8192, // 0x00002000
        }

        /// <summary>
        /// PageState
        ///     The type of pages in the region. The following types are defined.
        ///     https://msdn.microsoft.com/en-us/library/windows/desktop/aa366775(v=vs.85).aspx
        /// </summary>
        [Flags]
        public enum PageType
        {
            /// <summary>
            /// Indicates that the memory pages within the region are mapped into the view of an image section.
            /// </summary>
            Image = 16777216, // 0x01000000

            /// <summary>
            /// Indicates that the memory pages within the region are mapped into the view of a section.
            /// </summary>
            Mapped = 262144, // 0x00040000

            /// <summary>
            /// Indicates that the memory pages within the region are private (that is, not shared by other processes).
            /// </summary>
            Private = 131072, // 0x00020000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PSAPI_WORKING_SET_EX_INFORMATION
        {
            public IntPtr VirtualAddress;
            public PSAPI_WORKING_SET_EX_BLOCK VirtualAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PSAPI_WORKING_SET_EX_BLOCK
        {
            /// <summary>The working set information.</summary>
            public UIntPtr Flags;

            /// <summary>If <see langword="true"/>, the page is valid; otherwise, the page is not valid.</summary>
            public bool Valid => ((long)Flags & 0b1) == 1;
        }
    }
}
