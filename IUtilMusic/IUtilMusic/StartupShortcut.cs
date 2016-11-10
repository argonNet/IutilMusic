using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using System.Reflection;

namespace IUtilMusic
{
    class StartupShortcut
    {

        /// <summary>
        /// Add a shortcut to the startup menu, to load the application on login
        /// </summary>
        public static void Add()
        {
            String startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            WshShell shell = new WshShell();
            String shortCutLinkFilePath = Path.Combine(startupFolderPath,
                Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().GetName().Name) + ".lnk");

            IWshShortcut windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(shortCutLinkFilePath);

            windowsApplicationShortcut.Description = String.Format("{0} - Music player based on Leap Motion Controller",Application.ProductName);
            windowsApplicationShortcut.WorkingDirectory = Application.StartupPath;
            windowsApplicationShortcut.TargetPath = Application.ExecutablePath;
            windowsApplicationShortcut.Save();
        }

        /// <summary>
        /// Remove the shortcut in startup menu
        /// </summary>
        /// <returns></returns>
        public static void Remove()
        {
            System.IO.File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName) + ".lnk"));
        }

        /// <summary>
        /// Check if a startup shortcut is existing
        /// </summary>
        /// <returns></returns>
        public static Boolean Exist()
        {
            return System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName) + ".lnk"));
        }



    }
}
