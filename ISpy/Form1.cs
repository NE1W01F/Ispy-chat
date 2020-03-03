using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ISpy
{
    public partial class Form1 : Form
    {
        public static TcpClient client;
        public static Thread Server;
        public static string username = "AppleWolf";
        public static string password = "239di939fd8943u39";
        public static string IP = "127.0.0.1";
        public static int port = 2355;
        public static bool IsMin = false;
        public Form1()
        {
            InitializeComponent();
        }

        public static bool User_Check(string username_, ListBox Users)
        {
            int a = Users.Items.Count;
            for (int i = 0; i < a; i++)
            {
                if(username_ == (string)Users.Items[i])
                {
                    return true;
                }
            }
            return false;
        }
        public static void Server_Thread(ListBox Users, ListBox Messages)
        {
            bool exit = true;
            while (exit)
            {
                try
                {
                    try
                    {
                        client = new TcpClient(IP, port);
                        while (true)
                        {
                            Stream sw = client.GetStream();
                            StreamReader sr = new StreamReader(sw);
                            string[] datastr = sr.ReadLine().Split(':');
                            string[] data;
                            try
                            {
                                string message = Decrypt(datastr[1], password);
                                string user = datastr[0];
                                data = $"{user}:{message}".Split(':');
                            }
                            catch (Exception)
                            {
                                data = datastr;
                            }
                            if (!String.IsNullOrEmpty(data[1]))
                            {
                                if (!User_Check(data[0], Users))
                                {
                                    if (data[0] != "Unknown")
                                    {
                                        Users.Invoke((MethodInvoker)delegate { Users.Items.Add(data[0]); });
                                    }
                                    Messages.Invoke((MethodInvoker)delegate { Messages.Items.Add($"{data[0]} >> {data[1]}"); });
                                }
                                else
                                {
                                    Messages.Invoke((MethodInvoker)delegate { Messages.Items.Add($"{data[0]} >> {data[1]}"); });
                                }
                            }
                        }
                    }
                    catch (ThreadAbortException) { exit = false; }
                }
                catch (Exception) { }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Server.Abort();
            try
            {
                client.Close();
            }
            catch (Exception) { }
            base.OnClosing(e);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "iSpy - Not Connected to Web Chat";
            Server = new Thread(() => Server_Thread(listBox2, listBox1));
            listBox2.Items.Add(username);
            Server.Start();
            timer1.Enabled = true;
            timer1.Start();
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

        private void button1_Click(object sender, EventArgs e)
        {
            Stream sw = client.GetStream();
            StreamWriter writer = new StreamWriter(sw);
            string data = $"{username}:{Encrypt(textBox1.Text, password)}";
            listBox1.Items.Add($"{username} >> {textBox1.Text}");
            writer.WriteLine(data);
            writer.Flush();
            textBox1.Text = "";
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Server.Abort();
            client.Close();
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (client.Connected)
                {
                    this.Text = "iSpy - Connected to Web Chat";
                }
                else
                {
                    this.Text = "iSpy - Not Connected to Web Chat";
                }
                
            }
            catch (Exception) { }
        }

        private void restartConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Server.IsAlive)
            {
                Server.Abort();
                Server = new Thread(() => Server_Thread(listBox2, listBox1));
                Server.Start();
                MessageBox.Show("Restart Connection");
            }
            else
            {
                Server = new Thread(() => Server_Thread(listBox2, listBox1));
                Server.Start();
                MessageBox.Show("Restart Connection");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Server.Abort();
            client.Close();
            Application.Exit();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                IsMin = true;
            }
            else
            {
                IsMin = false;
            }
        }
    }
}
