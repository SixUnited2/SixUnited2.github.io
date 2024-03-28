using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace yz.gaming.accessoryapp.Utils
{
    public class UwpPackageUtils
    {
        [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern long FindPackagesByPackageFamily(
            string packageFamilyName,
            uint packageFilters,
            ref uint count,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)]
            string[] packageFullNames,
            ref uint bufferLength,
            IntPtr buffer,
            ref uint packageProperties);


        [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern long GetPackagesByPackageFamily(
            string packageFamilyName,
            ref uint count,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)]
            string[] packageFullNames,
            ref uint bufferLength,
            IntPtr buffer);

        [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true)]
        public static extern long PackageIdFromFullName(
            [MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
            uint flags,
            ref uint bufferLength,
            IntPtr buffer);

        [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true)]
        public static extern long GetPackagePath(
            ref PACKAGE_ID packageId,
            uint reserved,
            ref uint pathLength,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)]
            string[] path);

        public enum APPX_PACKAGE_ARCHITECTURE
        {
            /// <summary>The x86 processor architecture.</summary>
            APPX_PACKAGE_ARCHITECTURE_X86 = 0,

            /// <summary>The ARM processor architecture.</summary>
            APPX_PACKAGE_ARCHITECTURE_ARM = 5,

            /// <summary>The x64 processor architecture.</summary>
            APPX_PACKAGE_ARCHITECTURE_X64 = 9,

            /// <summary>Any processor architecture.</summary>
            APPX_PACKAGE_ARCHITECTURE_NEUTRAL = 11,

            /// <summary>The 64-bit ARM processor architecture.</summary>
            APPX_PACKAGE_ARCHITECTURE_ARM64 = 12,
        }

        const uint PACKAGE_INFORMATION_BASIC = 0x00000000;

        [StructLayout(LayoutKind.Explicit)]
        public struct PACKAGE_VERSION
        {
            [FieldOffset(0)]
            public ulong Version;

            [FieldOffset(0)]
            public DUMMYSTRUCTNAME Parts;

            public PACKAGE_VERSION(ulong version, DUMMYSTRUCTNAME parts)
            {
                Version = version;
                Parts = parts;
            }

            public struct DUMMYSTRUCTNAME
            {
                public ushort Revision;
                public ushort Build;
                public ushort Minor;
                public ushort Major;

                public DUMMYSTRUCTNAME(ushort revision, ushort build, ushort minor, ushort major)
                {
                    Revision = revision;
                    Build = build;
                    Minor = minor;
                    Major = major;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PACKAGE_INFO
        {
            public uint reserved;
            public uint flags;
            public string path;
            public string packageFullName;
            public string packageFamilyName;
            PACKAGE_ID packageId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PACKAGE_ID
        {
            public uint reserved;
            public APPX_PACKAGE_ARCHITECTURE processorArchitecture;
            public PACKAGE_VERSION version;
            public string name;
            public string publisher;
            public string resourceId;
            public string publisherId;

            public PACKAGE_ID(uint reserved, APPX_PACKAGE_ARCHITECTURE processorArchitecture, PACKAGE_VERSION version, string name, string publisher, string resourceId, string publisherId)
            {
                this.reserved = reserved;
                this.processorArchitecture = processorArchitecture;
                this.version = version;
                this.name = name;
                this.publisher = publisher;
                this.resourceId = resourceId;
                this.publisherId = publisherId;
            }
            public override string ToString()
            {
                return $"{version.Parts.Major}.{version.Parts.Minor}.{version.Parts.Build}.{version.Parts.Revision}";
            }
        }

        public static PACKAGE_ID GetUwpPackageId(string packageFamilyName)
        {
            PACKAGE_VERSION pACKAGE_VERSION = new PACKAGE_VERSION(0, new PACKAGE_VERSION.DUMMYSTRUCTNAME(0, 0, 0, 0));
            PACKAGE_ID packageId = new PACKAGE_ID(0, APPX_PACKAGE_ARCHITECTURE.APPX_PACKAGE_ARCHITECTURE_X86, pACKAGE_VERSION, "", "", "", "");

            uint packageCount = 0;
            uint bufferLength = 0;
            IntPtr buffer = IntPtr.Zero;
            IntPtr pIdBuffer = IntPtr.Zero;
            long res2 = GetPackagesByPackageFamily(packageFamilyName, ref packageCount, null, ref bufferLength, IntPtr.Zero);
            if (packageCount > 0)
            {
                string[] packageFullNames = new string[packageCount];
                buffer = Marshal.AllocHGlobal((int)bufferLength);
                res2 = GetPackagesByPackageFamily(packageFamilyName, ref packageCount, packageFullNames, ref bufferLength, buffer);

                uint nFlags = PACKAGE_INFORMATION_BASIC;
                string pFirstPackage = packageFullNames[0];

                uint nIdLen = 0;
                long nRet = PackageIdFromFullName(pFirstPackage, nFlags, ref nIdLen, IntPtr.Zero);
                if (nIdLen > 0)
                {
                    pIdBuffer = Marshal.AllocHGlobal((int)nIdLen);
                    nRet = PackageIdFromFullName(pFirstPackage, nFlags, ref nIdLen, pIdBuffer);
                    var temp = Marshal.PtrToStructure(pIdBuffer, typeof(PACKAGE_ID));
                    packageId = (PACKAGE_ID)temp;
                }
            }

            return packageId;
        }

        public static string GetUwpPackagePath(string packageFamilyName)
        {
            PACKAGE_ID packageId = GetUwpPackageId(packageFamilyName);

            uint reserved = 0;
            uint bufferLength = 0;
            long result = GetPackagePath(ref packageId, reserved, ref bufferLength, null);

            if (bufferLength > 0)
            {
                //PACKAGE_ID package = GetUwpPackageId(packageFamilyName);
                uint bufferLength2 = 0;
                string[] path = new string[bufferLength];
                result = GetPackagePath(ref packageId, reserved, ref bufferLength2, path);
            }

            return string.Empty;
        }
    }
}
