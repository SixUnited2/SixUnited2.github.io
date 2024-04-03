using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace yz.gaming.accessoryservice
{
    class UserProcess
    {
        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_LINKED_TOKEN
        {
            public IntPtr LinkedToken;
        }
        #endregion

        #region Enumerations
        enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }
        enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }
        enum WTSInfoClass
        {
            InitialProgram,
            ApplicationName,
            WorkingDirectory,
            OEMId,
            SessionId,
            UserName,
            WinStationName,
            DomainName,
            ConnectState,
            ClientBuildNumber,
            ClientName,
            ClientDirectory,
            ClientProductId,
            ClientHardwareId,
            ClientAddress,
            ClientDisplay,
            ClientProtocolType
        }

        enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            TokenIsAppContainer,
            TokenCapabilities,
            TokenAppContainerSid,
            TokenAppContainerNumber,
            TokenUserClaimAttributes,
            TokenDeviceClaimAttributes,
            TokenRestrictedUserClaimAttributes,
            TokenRestrictedDeviceClaimAttributes,
            TokenDeviceGroups,
            TokenRestrictedDeviceGroups,
            TokenSecurityAttributes,
            TokenIsRestricted,
            TokenProcessTrustLevel,
            TokenPrivateNameSpace,
            TokenSingletonAttributes,
            TokenBnoIsolation,
            TokenChildProcessFlags,
            TokenIsLessPrivilegedAppContainer,
            TokenIsSandboxed,
            TokenIsAppSilo,
            MaxTokenInfoClass
        }
        #endregion

        #region Constants
        public const int TOKEN_ALL_ACCESS = 0x10000000;
        public const int GENERIC_ALL_ACCESS = 0x10000000;
        public const int TOKEN_DUPLICATE = 0x0002;
        public const uint MAXIMUM_ALLOWED = 0x2000000;
        public const int CREATE_NEW_CONSOLE = 0x00000010;
        public const int IDLE_PRIORITY_CLASS = 0x40;
        public const int NORMAL_PRIORITY_CLASS = 0x20;
        public const int HIGH_PRIORITY_CLASS = 0x80;
        public const int REALTIME_PRIORITY_CLASS = 0x100;
        #endregion

        #region Win32 API Imports
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern bool WTSQuerySessionInformation(System.IntPtr hServer, int sessionId, WTSInfoClass wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);
        
        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        
        [DllImport("kernel32.dll")]
        static extern bool ProcessIdToSessionId(uint dwProcessId, ref uint pSessionId);
        
        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
            int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);
        
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        
        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(uint sessionId, out IntPtr Token);

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(IntPtr tokenHandle, TOKEN_INFORMATION_CLASS tokenInformationClass, IntPtr tokenInformation, int tokenInformationLength, out int returnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool SetTokenInformation(IntPtr tokenHandle, TOKEN_INFORMATION_CLASS tokenInformationClass, ref uint TokenInformation, uint TokenInformationLength);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
        public static extern int GetLastError();
        #endregion

        public static string GetCurrentActiveUser()
        {
            IntPtr hServer = IntPtr.Zero, state = IntPtr.Zero;
            uint bCount = 0;
            // obtain the currently active session id; every logged on user in the system has a unique session id  
            uint dwSessionId = WTSGetActiveConsoleSessionId();
            string domain = string.Empty, userName = string.Empty;
            if (WTSQuerySessionInformation(hServer, (int)dwSessionId, WTSInfoClass.DomainName, out state, out bCount))
            {
                domain = Marshal.PtrToStringAuto(state);
            }
            if (WTSQuerySessionInformation(hServer, (int)dwSessionId, WTSInfoClass.UserName, out state, out bCount))
            {
                userName = Marshal.PtrToStringAuto(state);
            }
            return string.Format("{0}\\{1}", domain, userName);
        }

        /// <summary>  
        /// Launches the given application with full admin rights, and in addition bypasses the Vista UAC prompt  
        /// </summary>  
        /// <param name="applicationName">The name of the application to launch</param>  
        /// <param name="procInfo">Process information regarding the launched application that gets returned to the caller</param>  
        /// <returns></returns>  
        public static bool StartProcessAndBypassUAC(String applicationName, String command, out PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new PROCESS_INFORMATION();
            // obtain the currently active session id; every logged on user in the system has a unique session id  
            uint dwSessionId = WTSGetActiveConsoleSessionId();
            // obtain the process id of the winlogon process that is running within the currently active session  
            Process[] processes = Process.GetProcessesByName("winlogon");
            foreach (Process p in processes)
            {
                if ((uint)p.SessionId == dwSessionId)
                {
                    winlogonPid = (uint)p.Id;
                }
            }
            // obtain a handle to the winlogon process  
            hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);
            // obtain a handle to the access token of the winlogon process  
            if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                CloseHandle(hProcess);
                return false;
            }
            // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser  
            // I would prefer to not have to use a security attribute variable and to just   
            // simply pass null and inherit (by default) the security attributes  
            // of the existing token. However, in C# structures are value types and therefore  
            // cannot be assigned the null value.  
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);
            // copy the access token of the winlogon process; the newly created token will be a primary token  
            if (!DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)TOKEN_TYPE.TokenPrimary, ref hUserTokenDup))
            {
                CloseHandle(hProcess);
                CloseHandle(hPToken);
                return false;
            }
            // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning  
            // the window station has a desktop that is invisible and the process is incapable of receiving  
            // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user   
            // interaction with the new process.  
            STARTUPINFO si = new STARTUPINFO();
            si.cb = (int)Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop  
            // flags that specify the priority and creation method of the process  
            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;
            // create a new process in the current user's logon session  
            bool result = CreateProcessAsUser(hUserTokenDup,        // client's access token  
                                            applicationName,        // file to execute  
                                            command,                // command line  
                                            ref sa,                 // pointer to process SECURITY_ATTRIBUTES  
                                            ref sa,                 // pointer to thread SECURITY_ATTRIBUTES  
                                            false,                  // handles are not inheritable  
                                            dwCreationFlags,        // creation flags  
                                            IntPtr.Zero,            // pointer to new environment block   
                                            null,                   // name of current directory   
                                            ref si,                 // pointer to STARTUPINFO structure  
                                            out procInfo            // receives information about new process  
                                            );
            // invalidate the handles  
            CloseHandle(hProcess);
            CloseHandle(hPToken);
            CloseHandle(hUserTokenDup);
            return result; // return the result  
        }

        /// <summary>  
        /// Launches the given application with full admin rights, and in addition bypasses the Vista UAC prompt  
        /// </summary>  
        /// <param name="applicationName">The name of the application to launch</param>  
        /// <param name="procInfo">Process information regarding the launched application that gets returned to the caller</param>  
        /// <returns></returns>  
        public static bool CurrentUserStartProcessAndBypassUAC(String applicationName, String command, out PROCESS_INFORMATION procInfo)
        {
            IntPtr hUserTokenDup = IntPtr.Zero;
            procInfo = new PROCESS_INFORMATION();
            // obtain the currently active session id; every logged on user in the system has a unique session id  
            uint dwSessionId = 0xFFFFFFFF;
            dwSessionId = WTSGetActiveConsoleSessionId();
            // obtain the process id of the winlogon process that is running within the currently active session  
            if (dwSessionId == 0xFFFFFFFF) return false;

            if (!WTSQueryUserToken(dwSessionId, out IntPtr hUserTok))
            {
                CloseHandle(hUserTok);
                return false;
            }

            // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser  
            // I would prefer to not have to use a security attribute variable and to just   
            // simply pass null and inherit (by default) the security attributes  
            // of the existing token. However, in C# structures are value types and therefore  
            // cannot be assigned the null value.  
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);
            // copy the access token of the winlogon process; the newly created token will be a primary token  
            if (!DuplicateTokenEx(hUserTok,
                GENERIC_ALL_ACCESS,
                ref sa,
                (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                (int)TOKEN_TYPE.TokenPrimary,
                ref hUserTokenDup))
            {
                CloseHandle(hUserTok);
                return false;
            }

            if (!CreateEnvironmentBlock(out IntPtr lpEnvironment, hUserTokenDup, false))
            {
                CloseHandle(hUserTokenDup);
                CloseHandle(hUserTok);
                return false;
            }

            // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning  
            // the window station has a desktop that is invisible and the process is incapable of receiving  
            // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user   
            // interaction with the new process.  
            STARTUPINFO si = new STARTUPINFO();
            si.cb = (int)Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop  
            // flags that specify the priority and creation method of the process  
            //int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;
            int dwCreationFlags = 0;
            // create a new process in the current user's logon session  
            bool result = CreateProcessAsUser(hUserTokenDup,        // client's access token  
                                              applicationName,        // file to execute  
                                              command,                // command line  
                                              ref sa,                 // pointer to process SECURITY_ATTRIBUTES  
                                              ref sa,                 // pointer to thread SECURITY_ATTRIBUTES  
                                              false,                  // handles are not inheritable  
                                              dwCreationFlags,        // creation flags  
                                              IntPtr.Zero,            // pointer to new environment block   
                                              null,                   // name of current directory   
                                              ref si,                 // pointer to STARTUPINFO structure  
                                              out procInfo            // receives information about new process  
                                            );
            // invalidate the handles  
            CloseHandle(lpEnvironment);
            CloseHandle(hUserTok);
            CloseHandle(hUserTokenDup);
            return result; // return the result  
        }

        /// <summary>
        /// Launches the given application with full admin rights, and in addition bypasses the Vista UAC prompt
        /// </summary>
        /// <param name="applicationName">The name of the application to launch</param>
        /// <param name="procInfo">Process information regarding the launched application that gets returned to the caller</param>
        /// <returns></returns>
        public static bool StartProcessAndBypassUAC(String applicationName, out PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new PROCESS_INFORMATION();

            // obtain the currently active session id; every logged on user in the system has a unique session id
            TSControl.WTS_SESSION_INFO[] pSessionInfo = TSControl.SessionEnumeration();
            uint dwSessionId = 100;
            for (int i = 0; i < pSessionInfo.Length; i++)
            {
                if (pSessionInfo[i].SessionID != 0)
                {
                    try
                    {
                        int count = 0;
                        IntPtr buffer = IntPtr.Zero;
                        StringBuilder sb = new StringBuilder();

                        bool bsuccess = TSControl.WTSQuerySessionInformation(
                           IntPtr.Zero, pSessionInfo[i].SessionID,
                           TSControl.WTSInfoClass.WTSUserName, out sb, out count);

                        if (bsuccess)
                        {
                            if (sb.ToString().Trim() == "Administrator")//Administrator
                            {
                                dwSessionId = (uint)pSessionInfo[i].SessionID;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LoaderService.WriteLog(ex.Message.ToString(), "Monitor");
                    }
                }
            }

            // obtain the process id of the winlogon process that is running within the currently active session
            Process[] processes = Process.GetProcessesByName("explorer");
            foreach (Process p in processes)
            {
                if ((uint)p.SessionId == dwSessionId)
                {
                    winlogonPid = (uint)p.Id;
                }
            }

            //LoaderService.WriteLog(winlogonPid.ToString(), "Monitor");

            // obtain a handle to the winlogon process
            hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            // obtain a handle to the access token of the winlogon process
            if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                CloseHandle(hProcess);
                return false;
            }

            // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser
            // I would prefer to not have to use a security attribute variable and to just 
            // simply pass null and inherit (by default) the security attributes
            // of the existing token. However, in C# structures are value types and therefore
            // cannot be assigned the null value.
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            // copy the access token of the winlogon process; the newly created token will be a primary token
            if (!DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)TOKEN_TYPE.TokenPrimary, ref hUserTokenDup))
            {
                CloseHandle(hProcess);
                CloseHandle(hPToken);
                return false;
            }

            // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning
            // the window station has a desktop that is invisible and the process is incapable of receiving
            // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user 
            // interaction with the new process.
            STARTUPINFO si = new STARTUPINFO();
            si.cb = (int)Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop

            // flags that specify the priority and creation method of the process
            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

            // create a new process in the current user's logon session
            bool result = CreateProcessAsUser(hUserTokenDup,        // client's access token
                                            null,                   // file to execute
                                            applicationName,        // command line
                                             ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                                             ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                             false,                  // handles are not inheritable
                                             dwCreationFlags,        // creation flags
                                             IntPtr.Zero,            // pointer to new environment block 
                                             null,                   // name of current directory 
                                             ref si,                 // pointer to STARTUPINFO structure
                                             out procInfo            // receives information about new process
                                             );

            // invalidate the handles
            CloseHandle(hProcess);
            CloseHandle(hPToken);
            CloseHandle(hUserTokenDup);
            //LoaderService.WriteLog("launch Task", "Monitor");

            return result; // return the result
        }

        /// <summary>
        /// Launches the given application with full admin rights, and in addition bypasses the Vista UAC prompt
        /// </summary>
        /// <param name="applicationName">The name of the application to launch</param>
        /// <param name="procInfo">Process information regarding the launched application that gets returned to the caller</param>
        /// <returns></returns>
        public static bool CurrentUserStartProcessAndBypassUAC(String applicationName, out PROCESS_INFORMATION procInfo)
        {
            procInfo = new PROCESS_INFORMATION();
            // obtain the currently active session id; every logged on user in the system has a unique session id  
            uint dwSessionId = 0xFFFFFFFF;
            dwSessionId = WTSGetActiveConsoleSessionId();
            // obtain the process id of the winlogon process that is running within the currently active session  
            if (dwSessionId == 0xFFFFFFFF) return false;

            if (!WTSQueryUserToken(dwSessionId, out IntPtr hUserTok))
            {
                CloseHandle(hUserTok);
                return false;
            }

            // Get privileged(with admin enabled) token if had, or just use the original token
            IntPtr hLnkTok = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TOKEN_LINKED_TOKEN)));
            if (!GetTokenInformation(hUserTok, TOKEN_INFORMATION_CLASS.TokenLinkedToken, hLnkTok, Marshal.SizeOf(typeof(TOKEN_LINKED_TOKEN)), out int dwSize))
            {
                CloseHandle(hUserTok);
                CloseHandle(hLnkTok);
                return false;
            }

            IntPtr hRunTok = IntPtr.Zero;
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);
            TOKEN_LINKED_TOKEN lnkTok = (TOKEN_LINKED_TOKEN)Marshal.PtrToStructure(hLnkTok, typeof(TOKEN_LINKED_TOKEN));
            if (!DuplicateTokenEx(lnkTok.LinkedToken,
                                  TOKEN_ALL_ACCESS,
                                  ref sa,
                                  (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                                  (int)TOKEN_TYPE.TokenPrimary,
                                  ref hRunTok))
            {
                CloseHandle(hUserTok);
                CloseHandle(lnkTok.LinkedToken);
                //CloseHandle(hRunTok);
                return false;
            }

            if (!SetTokenInformation(hRunTok,
                                     TOKEN_INFORMATION_CLASS.TokenSessionId,
                                     ref dwSessionId,
                                     sizeof(uint)))
            {
                CloseHandle(hUserTok);
                CloseHandle(lnkTok.LinkedToken);
                CloseHandle(hRunTok);
                return false;
            }
            // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning
            // the window station has a desktop that is invisible and the process is incapable of receiving
            // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user 
            // interaction with the new process.
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop

            // flags that specify the priority and creation method of the process
            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

            // create a new process in the current user's logon session
            bool result = CreateProcessAsUser(hRunTok,        // client's access token
                                              applicationName,        // file to execute
                                              string.Empty,           // command line
                                              ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                                              ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                              false,                  // handles are not inheritable
                                              dwCreationFlags,        // creation flags
                                              IntPtr.Zero,            // pointer to new environment block 
                                              null,                   // name of current directory 
                                              ref si,                 // pointer to STARTUPINFO structure
                                              out procInfo            // receives information about new process
                                             );

            // invalidate the handles
            CloseHandle(hUserTok);
            CloseHandle(lnkTok.LinkedToken);
            CloseHandle(hRunTok);
            //LoaderService.WriteLog("launch Task", "Monitor");

            return result; // return the result
        }
    }
}
