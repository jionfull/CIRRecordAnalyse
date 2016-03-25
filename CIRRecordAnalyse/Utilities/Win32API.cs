using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace CIRRecordAnalyse.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEM_INFO
    {
        public uint dwOemId;
        public uint dwPageSize;
        public uint lpMinimumApplicationAddress;
        public uint lpMaximumApplicationAddress;
        public uint dwActiveProcessorMask;
        public uint dwNumberOfProcessors;
        public uint dwProcessorType;
        public uint dwAllocationGranularity;
        public uint dwProcessorLevel;
        public uint dwProcessorRevision;
    }
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }
    class Win32API
    {
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const int OPEN_EXISTING = 3;
        public const int INVALID_HANDLE_VALUE = -1;
        public const int FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
        public const uint PAGE_READWRITE = 0x04;

        public const int FILE_MAP_COPY = 1;
        public const int FILE_MAP_WRITE = 2;
        public const int FILE_MAP_READ = 4;

      public  const int WM_COPYDATA = 0x004A;
        /// <summary>
        /// 内存映射文件句柄
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpFileMappingAttributes"></param>
        /// <param name="flProtect"></param>
        /// <param name="dwMaximumSizeHigh"></param>
        /// <param name="dwMaximumSizeLow"></param>
        /// <param name="lpName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFileMapping(IntPtr hFile,
            IntPtr lpFileMappingAttributes, uint flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow, string lpName);
        /// <summary>
        /// 内存映射文件
        /// </summary>
        /// <param name="hFileMappingObject"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwFileOffsetHigh"></param>
        /// <param name="dwFileOffsetLow"></param>
        /// <param name="dwNumberOfBytesToMap"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint
            dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
            uint dwNumberOfBytesToMap);

        /// <summary>
        /// 撤消文件映像
        /// </summary>
        /// <param name="lpBaseAddress"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        /// <summary>
        /// 关闭内核对象句柄
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 打开要映射的文件
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="securityAttrs"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFile(string lpFileName,
            uint dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs,
            FileMode dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern bool ReadFile(IntPtr File, byte[] Buffer, int NumberOfBytesToRead, out int NumberOfBytesRead, IntPtr Overlapped);

        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(IntPtr File, byte[] Buffer, int NumberOfBytesToWrite, out int NumberOfBytesWritten, IntPtr Overlapped);

        [DllImport("kernel32.dll")]
        public static extern UInt32 SetFilePointer(IntPtr hFile, int lDistanceToMove, IntPtr lpDistanceToMoveHigh, int dwMoveMethod);

        /// <summary>
        /// 得到文件大小
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="highSize"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetFileSize(IntPtr hFile, out uint highSize);

        /// <summary>
        /// 得到系统信息
        /// </summary>
        /// <param name="lpSystemInfo"></param>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);


        [DllImport("Gdi32.dll")]
        public static extern bool InvertRgn(IntPtr hdc, IntPtr hrgn);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
         IntPtr hWnd, //目标窗口的handle
         int Msg, // 消息
         int wParam, // 第一个消息参数
         ref COPYDATASTRUCT lParam // 第二个消息参数
         );

    }
}
