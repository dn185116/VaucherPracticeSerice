using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;

namespace VaucherPracticeService.SharedTool
{
    [ExcludeFromCodeCoverage]
    public class Utility
    {
        public static bool RegisterManifest(bool is32bit)
        {
            if (is32bit)
            {
                try
                {
                    const string keyName32 = @"SOFTWARE\NCR\RAL\Manifests";
                    var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    var key = baseKey.OpenSubKey(keyName32, true);
                    if (key != null)
                    {
                        key.SetValue("KfcSgPaymentService", $"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace("KfcSgPaymentService.exe", "Manifest.xml")}");
                        return true;
                    }
                }
                catch (Exception e)
                {

                }
                return false;
            }
            else
            {
                try
                {
                    const string keyName64 = @"SOFTWARE\Wow6432Node\NCR\RAL\Manifests";
                    var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    var key = baseKey.OpenSubKey(keyName64, true);
                    if (key != null)
                    {
                        key.SetValue("KfcSgPaymentService", $"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace("KfcSgPaymentService.exe", "Manifest.xml")}");
                        return true;
                    }
                }
                catch (Exception e)
                {

                }
                return false;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
