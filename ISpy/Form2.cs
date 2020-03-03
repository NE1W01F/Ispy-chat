using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ISpy
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = Form1.username;
            textBox2.Text = Form1.password;
            textBox3.Text = Form1.IP;
            textBox4.Text = Form1.port.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("config.ini"))
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\ISpy", RegistryKeyPermissionCheck.ReadWriteSubTree);
                string encrypt_key = (string)key.GetValue("key");
                key.Close();
                if (textBox1.Text == "Unknown" || String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter a username");
                }
                else
                {
                    int ave;
                    if (String.IsNullOrEmpty(textBox2.Text))
                    {
                        MessageBox.Show("Please enter a password");
                    }
                    else if (String.IsNullOrEmpty(textBox3.Text))
                    {
                        MessageBox.Show("Enter Server IP");
                    }
                    else if (String.IsNullOrEmpty(textBox4.Text) || !int.TryParse(textBox4.Text, out ave))
                    {
                        MessageBox.Show("Please enter a Port Number");
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter("config.ini");
                        sw.WriteLine(Encrypt($"USERNAME={textBox1.Text}", encrypt_key));
                        sw.WriteLine(Encrypt($"PASSWORD={textBox2.Text}", encrypt_key));
                        sw.WriteLine(Encrypt($"IP={textBox3.Text}", encrypt_key));
                        sw.WriteLine(Encrypt($"PORT={textBox4.Text}", encrypt_key));
                        sw.Close();
                        Form1.username = textBox1.Text;
                        Form1.password = textBox2.Text;
                        Form1.IP = textBox3.Text;
                        Form1.port = int.Parse(textBox4.Text);
                        MessageBox.Show("Settings Saved");
                    }
                }
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\ISpy", RegistryKeyPermissionCheck.ReadWriteSubTree);
                string encrypt_key = Guid.NewGuid().ToString();
                key.SetValue("key", encrypt_key);
                key.Close();
                if (textBox1.Text == "Unknown" || String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter a username");
                }
                else
                {
                    int ave;
                    if (String.IsNullOrEmpty(textBox2.Text))
                    {
                        MessageBox.Show("Please enter a password");
                    }
                    else if (String.IsNullOrEmpty(textBox3.Text))
                    {
                        MessageBox.Show("Enter Server IP");
                    }
                    else if (String.IsNullOrEmpty(textBox4.Text) || !int.TryParse(textBox4.Text, out ave))
                    {
                        MessageBox.Show("Please enter a Port Number");
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter("config.ini");
                        sw.WriteLine(Encrypt($"USERNAME={textBox1.Text}", encrypt_key));
                        sw.WriteLine(Encrypt($"PASSWORD={textBox2.Text}", encrypt_key));
                        sw.WriteLine(Encrypt($"IP={textBox3.Text}", encrypt_key));
                        sw.WriteLine(Encrypt($"PORT={textBox4.Text}", encrypt_key));
                        sw.Close();
                        Form1.username = textBox1.Text;
                        Form1.password = textBox2.Text;
                        Form1.IP = textBox3.Text;
                        Form1.port = int.Parse(textBox4.Text);
                        MessageBox.Show("Settings Saved");
                    }
                }
            }
            this.Close();
        }

        public static string Encrypt(string text, string key_)
        {
            byte[] input = Encoding.UTF8.GetBytes(text);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] IV = { 23, 29, 49, 100, 233, 47, 10, 49, 50, 28, 54, 72, 58, 10, 09, 34 };
            aes.IV = IV;
            aes.Key = GetHashSha256(key_);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypto = aes.CreateEncryptor();
            byte[] output = crypto.TransformFinalBlock(input, 0, input.Length);
            aes.Clear();
            return Convert.ToBase64String(output);
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
