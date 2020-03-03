using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ISpy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Load_Settings())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        static bool Load_Settings()
        {
            if (File.Exists("config.ini"))
            {
                string username;
                string password;
                string ip;
                string port;
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\ISpy", RegistryKeyPermissionCheck.ReadWriteSubTree);
                string encrypt_key = (string)key.GetValue("key");
                StreamReader sr = new StreamReader("config.ini");
                try
                {
                    username = Decrypt(sr.ReadLine(), encrypt_key).Split('=')[1];
                    password = Decrypt(sr.ReadLine(), encrypt_key).Split('=')[1];
                    ip = Decrypt(sr.ReadLine(), encrypt_key).Split('=')[1];
                    port = Decrypt(sr.ReadLine(), encrypt_key).Split('=')[1];
                }
                catch (Exception)
                {
                    MessageBox.Show("config.ini is corrupted, please delete it");
                    return false;
                }
                sr.Close();
                if(String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    MessageBox.Show("config.ini is corrupted, please delete it");
                    return false;
                }
                else
                {
                    if(username == "Unknown")
                    {
                        MessageBox.Show("config.ini has been changed, please delete it");
                        return false;
                    }
                    else
                    {
                        Form1.username = username;
                        Form1.password = password;
                        Form1.IP = ip;
                        Form1.port = int.Parse(port);
                        return true;
                    }
                }
            }
            else
            {
                Form1.username = "User";
                Form1.password = "password";
                return true;
            }
        }
        public static byte[] GetHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            return hash;
        }
        public static string Decrypt(string text, string key_)
        {
            byte[] input = Convert.FromBase64String(text);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] IV = { 23, 29, 49, 100, 233, 47, 10, 49, 50, 28, 54, 72, 58, 10, 09, 34 };
            aes.IV = IV;
            aes.Key = GetHashSha256(key_);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypto = aes.CreateDecryptor();
            byte[] output = crypto.TransformFinalBlock(input, 0, input.Length);
            return Encoding.UTF8.GetString(output);
        }
    }
}
