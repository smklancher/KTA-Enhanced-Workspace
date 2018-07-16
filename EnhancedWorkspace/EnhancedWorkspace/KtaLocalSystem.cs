using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EnhancedWorkspace
{

    public class KtaLocalSystem
    {
        private const string ServicesKey = "SYSTEM\\CurrentControlSet\\Services\\";
        private const string StreamingServiceName = "TotalAgility Streaming Service";
        private const string CoreWorkerServiceName = "TotalAgility Core Worker";
        private const string TransformationServiceName = "KofaxTransformationServerService";
        private const string ReportingServiceName = "KofaxTAReportingServerService";
        private const string KicServiceName = "KIC-ED-MC";
        private const string KsalServiceName = "KSALicenseService";

        /// <summary>
        /// When KTA libraries are needed, add to resolve handler upon startup:
        /// AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf LocalSystem.SdkResolveHandler
        /// AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SdkResolveHandler);
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Assembly SdkResolveHandler(object sender, ResolveEventArgs args)
        {
            // Load file from first place it exists
            // No attempt is made to load subsequent files from the same folder
            // This could be a problem if files were missing from one folder and different version from another, but low chance of that

            string AssemblyName = args.Name.Split(',')[0];
            List<DirectoryInfo> Folders = (new KtaLocalSystem()).ApiFolders();
            foreach (var f in Folders)
            {
                string PathWithoutExtension = Path.Combine(f.FullName, AssemblyName);
                if (File.Exists($"{PathWithoutExtension}.dll"))
                {
                    return Assembly.LoadFile($"{PathWithoutExtension}.dll");
                }
                if (File.Exists($"{PathWithoutExtension}.exe"))
                {
                    return Assembly.LoadFile($"{PathWithoutExtension}.exe");
                }
            }

            return null;
        }

        private static bool IsResolveHandlerSet = false;

        public static void SetResolveHandler()
        {
            if (!IsResolveHandlerSet)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SdkResolveHandler);
                IsResolveHandlerSet = true;
            }
        }

        public List<DirectoryInfo> ApiFolders()
        {
            List<DirectoryInfo> Folders = new List<DirectoryInfo>();
            Folders.Add(ServiceFolder(CoreWorkerServiceName));
            if (WebFolder() != string.Empty)
            {
                Folders.Add(new DirectoryInfo(WebFolder()));
                Folders.Add(new DirectoryInfo(Path.Combine(WebFolder(), "bin")));
            }
            Folders.Add(ServiceFolder(TransformationServiceName));
            Folders.Add(ServiceFolder(ReportingServiceName));

            return Folders;
        }

        public List<string> ConfigFiles()
        {
            // Add the folders to consider
            List<DirectoryInfo> Folders = ApiFolders();
            // Try default path for repo browser
            Folders.Add(new DirectoryInfo("C:\\Program Files (x86)\\Kofax\\TotalAgility Repository Browser\\"));
            // Collect all of the config file paths in all folders
            List<string> Files = new List<string>();
            foreach (DirectoryInfo CurFolder in Folders)
            {
                if (CurFolder.Exists)
                {
                    Files.AddRange(CurFolder.EnumerateFiles("*.config").Select(File => File.FullName));
                }
            }

            return Files;
        }

        private DirectoryInfo ServiceFolder(string ServiceName)
        {
            string RegString = ReadRegString(RegistryHive.LocalMachine, ServicesKey + ServiceName, "ImagePath");
            // Take the quoted path ignoring any command lines added after
            string FileString = RegString;
            if (RegString.StartsWith("\""))
            {
                Match m = Regex.Match(RegString, "\"(.*?)\"");
                FileString = m.Groups[1].Value;
            }
            // Fileinfo can handle paths that don't exist, but not blank, so this is a silly workaround for a valid non existant path
            // Could allow returning nulls (and adapt callers) instead of this
            if (FileString == string.Empty)
            {
                FileString = Path.Combine("C:\\", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }
            return new FileInfo(FileString).Directory;
        }

        /// <summary>
        /// Base folder used for the website which defaults to C:\Program Files\Kofax\TotalAgility\Agility.Server.Web\
        /// </summary>
        /// <returns></returns>
        public string WebFolder()
        {
            return (KtaBaseFolder() == string.Empty) ? string.Empty : Path.Combine(KtaBaseFolder(), "Agility.Server.Web");
        }

        /// <summary>
        /// Base folder which defaults to C:\Program Files\Kofax\TotalAgility\
        /// </summary>
        /// <returns></returns>
        public string KtaBaseFolder()
        {
            // parent of core worker folder if available
            DirectoryInfo CurFolder = ServiceFolder(CoreWorkerServiceName);
            if (CurFolder.Exists)
                return CurFolder.Parent.FullName;
            // two levels up from streaming service on web
            CurFolder = ServiceFolder(StreamingServiceName);
            if (CurFolder.Exists)
                return CurFolder.Parent.Parent.FullName;
            // or KTA not installed
            return string.Empty;
        }

        /// <summary>
        /// License server folder which defaults to C:\Program Files (x86)\Kofax\TotalAgility\LicenseServer\
        /// </summary>
        /// <returns></returns>
        public string LicenseServerFolder()
        {
            DirectoryInfo CurFolder = ServiceFolder(KsalServiceName);
            if (CurFolder.Exists)
                return CurFolder.FullName;
            return string.Empty;
        }

        /// <summary>
        /// License client folder which defaults to C:\Program Files\Kofax\TotalAgility\Licensing\
        /// </summary>
        /// <returns></returns>
        public string LicenseClientFolder()
        {
            string folder = KtaBaseFolder();
            return (folder == string.Empty) ? string.Empty : Path.Combine(folder, "Licensing\\");
        }

        public string LicenseUtilityPath()
        {
            string folder = LicenseClientFolder();
            return (folder == string.Empty) ? string.Empty : Path.Combine(folder, "KSALicenseUtility.exe");
        }

        public static string ReadRegString(RegistryHive BaseKey, string subKey, string KeyName, RegistryView View = RegistryView.Default)
        {
            try
            {
                RegistryKey rk = RegistryKey.OpenBaseKey(BaseKey, View);
                RegistryKey sk1 = rk.OpenSubKey(subKey);

                // Return the value as string, or empty string if key or value does not exist
                return sk1?.GetValue(KeyName)?.ToString() ?? String.Empty;
            }
            catch (Exception e)
            {
                Debug.Print((e.Message + (": " + KeyName.ToUpper())));
                return String.Empty;
            }

        }

        public static void WriteRegString(RegistryHive BaseKey, string subKey, string KeyName, string Value, RegistryView View = RegistryView.Default)
        {
            try
            {
                RegistryKey rk = RegistryKey.OpenBaseKey(BaseKey, View);
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                sk1.SetValue(KeyName, Value, RegistryValueKind.String);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message + ": " + KeyName.ToUpper());
            }

        }

    }

}
