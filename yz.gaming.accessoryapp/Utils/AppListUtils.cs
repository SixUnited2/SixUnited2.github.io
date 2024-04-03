using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace yz.gaming.accessoryapp.Utils
{
    public class AppListUtils
    {
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        private const int MAX_PATH = 260;
        private const int CSIDL_COMMON_STARTMENU = 0x0017;
        private const int CSIDL_PROGRAMS = 0x0002;

        [DllImport("msi.dll", SetLastError = true)]
        static extern int MsiEnumProducts(int iProductIndex, StringBuilder lpProductBuf);
        [DllImport("msi.dll", SetLastError = true)]
        static extern int MsiGetProductInfo(string szProduct, string szProperty, StringBuilder lpValueBuf, ref int pcchValueBuf);

        /// <summary>
        /// 获取本机All User开始程序路径(C:\Documents and Settings\All Users\「开始」菜单\程序)
        /// </summary>
        /// <returns></returns>
        public static string GetAllUsersStartMenuPath()
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_STARTMENU, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }
        /// <summary>
        /// 获取本机管理员开始程序路径(C:\Documents and Settings\All Users\「开始」菜单\程序)
        /// </summary>
        /// <returns></returns>
        public static string GetUsersStartMenuPath()
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, CSIDL_PROGRAMS, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }


        [Flags()]
        public enum SLR_FLAGS
        {
            SLR_NO_UI = 0x1,
            SLR_ANY_MATCH = 0x2,
            SLR_UPDATE = 0x4,
            SLR_NOUPDATE = 0x8,
            SLR_NOSEARCH = 0x10,
            SLR_NOTRACK = 0x20,
            SLR_NOLINKINFO = 0x40,
            SLR_INVOKE_MSI = 0x80
        }

        [Flags()]
        public enum SLGP_FLAGS
        {
            SLGP_SHORTPATH = 0x1,
            SLGP_UNCPRIORITY = 0x2,
            SLGP_RAWPATH = 0x4
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        // Unicode version
        public struct WIN32_FIND_DATA
        {
            public int dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
            private const int MAX_PATH = 260;
        }

        [
         ComImport(),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         Guid("000214F9-0000-0000-C000-000000000046")
         ]

        // Unicode version
        public interface IShellLink
        {
            void GetPath(
              [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
              int cchMaxPath,
              out WIN32_FIND_DATA pfd,
              SLGP_FLAGS fFlags);

            void GetIDList(
              out IntPtr ppidl);

            void SetIDList(
              IntPtr pidl);

            void GetDescription(
              [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
              int cchMaxName);

            void SetDescription(
              [MarshalAs(UnmanagedType.LPWStr)] string pszName);

            void GetWorkingDirectory(
              [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
              int cchMaxPath);

            void SetWorkingDirectory(
              [MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            void GetArguments(
              [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
              int cchMaxPath);

            void SetArguments(
              [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            void GetHotkey(
              out short pwHotkey);

            void SetHotkey(
              short wHotkey);

            void GetShowCmd(
              out int piShowCmd);

            void SetShowCmd(
              int iShowCmd);

            void GetIconLocation(
              [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
              int cchIconPath,
              out int piIcon);

            void SetIconLocation(
              [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
              int iIcon);

            void SetRelativePath(
              [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
              int dwReserved);

            void Resolve(
              IntPtr hwnd,
              SLR_FLAGS fFlags);

            void SetPath(
              [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport(), Guid("00021401-0000-0000-C000-000000000046")]
        public class ShellLink
        {
        }

        public static Dictionary<string, string> LoadAppList(List<string> apps)
        {
            string allUserStartMenuPath = GetAllUsersStartMenuPath() + "\\";
            string admStartMenuPath = GetUsersStartMenuPath() + "\\";
            IShellLink shellLink = (IShellLink)new ShellLink();
            IPersistFile vPersistFile = shellLink as IPersistFile;
            List<string> temp = new List<string>();
            List<string> appLnks = new List<string>();
            List<string> appNames = new List<string>();
            List<string> appPaths = new List<string>();
            GetInfo(appNames, appLnks, allUserStartMenuPath);
            GetInfo(appNames, appLnks, admStartMenuPath);

            Dictionary<string, string> ret = new Dictionary<string, string>();

            for (int j = 0; j < appLnks.Count; j++)
            {
                if (!apps.Contains(appNames[j])) continue;

                //获取快捷键运行路径
                vPersistFile.Load(appLnks[j], 0);
                StringBuilder stringBuilder = new StringBuilder(300);
                WIN32_FIND_DATA vWIN32_FIND_DATA;
                shellLink.GetPath(stringBuilder, stringBuilder.Capacity,
                    out vWIN32_FIND_DATA, SLGP_FLAGS.SLGP_RAWPATH);
                string appPath = stringBuilder.ToString();

                //过滤不想要的程序
                if (!appPath.ToLower().Contains("exe") || appPath.ToLower().Contains("unins")
                     || appPath.ToLower().Contains("tool") || appPath.ToLower().Contains("config")
                    || appPath.ToLower().Contains("{") || appPath.ToLower().Contains("%")
                    || appPath.ToLower().Contains("plug") || appPath.ToLower().Contains("activex")
                    || appPath.ToLower().Contains("help") || appPath.ToLower().Contains("extension")
                    || appPath.ToLower().Contains("driver") || appPath.ToLower().Contains("system32")
                    || appPath.ToLower().Contains("update") || appPath.ToLower().Contains("cmd")
                    || appPath.ToLower().Contains("ati"))
                {
                    continue;
                }
                if (appNames[j].ToLower().Contains("command") || appNames[j].ToLower().Contains("卸载") || appNames[j].ToLower().Contains("logg") || appNames[j].ToLower().Contains("unins")
                    || appNames[j].ToLower().Contains("setting") || appNames[j].ToLower().Contains("wizard")
                    || appNames[j].ToLower().Contains("debugger") || appNames[j].ToLower().Contains("idea touch")
                    || appNames[j].ToLower().Contains("touch-out"))
                {
                    continue;
                }

                if (temp.Contains(appPath))
                {
                    continue;
                }
                temp.Add(appPath);
                appPaths.Add(appPath);

                ret.Add(appNames[j], appPath);
            }

            return ret;
        }

        //获取当前文件夹下所有的快捷方式的名称和路径
        private static void GetInfo(List<string> appNames, List<string> appLnks, string path)
        {
            try
            {
                DirectoryInfo userFolder = new DirectoryInfo(path);
                foreach (DirectoryInfo NextFolder in userFolder.GetDirectories())
                {
                    if (NextFolder.Name.ToLower() == "touch game")
                    {
                        continue;
                    }
                    if (NextFolder.Attributes == FileAttributes.ReadOnly)
                    {
                        continue;
                    }
                    foreach (DirectoryInfo innerFolder in NextFolder.GetDirectories())
                    {
                        GetInfo(appNames, appLnks, NextFolder.FullName);
                    }
                    foreach (FileInfo NextFile in NextFolder.GetFiles())
                    {

                        if (NextFile.Extension.ToLower() != ".lnk")
                        {
                            continue;
                        }
                        string name = NextFile.Name.Substring(0, NextFile.Name.LastIndexOf("."));
                        if (appNames.Contains(name))
                        {
                            continue;
                        }
                        appNames.Add(name);
                        appLnks.Add(NextFile.FullName);
                    }
                }
                foreach (FileInfo NextFolder in userFolder.GetFiles())
                {
                    if (NextFolder.Name.ToLower() == "touch game")
                    {
                        continue;
                    }
                    if (NextFolder.Extension.ToLower() != ".lnk")
                    {
                        continue;
                    }
                    string name = NextFolder.Name.Substring(0, NextFolder.Name.LastIndexOf("."));
                    if (appNames.Contains(name))
                    {
                        continue;
                    }
                    appNames.Add(name);
                    appLnks.Add(NextFolder.FullName);

                }
            }
            catch (Exception)
            {

            }
        }
    }
}
