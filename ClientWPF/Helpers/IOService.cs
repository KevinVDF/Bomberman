﻿using System;
using Microsoft.Win32;

namespace ClientWPF.Helpers
{
    public static class IOService
    {
        public static string SaveFileDialog(string defaultFilename, string defaultExtension)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = defaultFilename,
                DefaultExt = defaultExtension,
                AddExtension = true,
                Filter = String.Format("{0} File|*.{0}", defaultExtension)
            };
            if (dlg.ShowDialog() == true)
                return dlg.FileName;
            else
                return null;
        }
    }
}
