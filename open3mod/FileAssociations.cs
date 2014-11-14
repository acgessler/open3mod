using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Forms;
using Microsoft.Win32;

namespace open3mod
{
    public static class FileAssociations
    {
        /// <summary>
        /// Set file-system associations for the given list of file formats
        /// </summary>
        /// <param name="extensionList"></param>
        public static bool SetAssociations(string[] extensionList)
        {
            // based on the old assimp viewer code (<assimp-repo>/tools/assimp_cmd) and
            // http://stackoverflow.com/questions/2681878/associate-file-extension-with-application
            foreach(var extension in extensionList)
            {
                using (var baseKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + extension))
                {
                    if (baseKey != null)
                    {
                        baseKey.SetValue("", "OPEN3MOD_CLASS");
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            using (var currentUser = Registry.CurrentUser.CreateSubKey(@"Software\Classes\OPEN3MOD_CLASS"))
            {
                using (var openKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\OPEN3MOD_CLASS\shell\open\command"))
                {
                    if (openKey != null)
                    {
                        openKey.SetValue("", Application.ExecutablePath + " \"%1\"");
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Tell explorer the file association has been changed
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            return true;
        }


        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
