﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(IntPtr classname, string title); // extern method: FindWindow

        [DllImport("user32.dll")]
        static extern void MoveWindow(IntPtr hwnd, int X, int Y,
            int nWidth, int nHeight, bool rePaint); // extern method: MoveWindow

        [DllImport("user32.dll")]
        static extern bool GetWindowRect
            (IntPtr hwnd, out Rectangle rect); // extern method: GetWindowRect
        public Form1()
        {
            InitializeComponent();
            Dictionary<string, string> AuthorList = new Dictionary<string, string>();
            //Dictionary comboSource = new Dictionary();
            AuthorList.Add("1", "CB");
            AuthorList.Add("2", "CS");
            AuthorList.Add("3", "CS SCH");
            AuthorList.Add("4", "DSS");
            AuthorList.Add("5", "DS");
            AuthorList.Add("6", "MRK");
            AuthorList.Add("7", "INS");
            AuthorList.Add("8", "CL");
            comboBox1.DataSource = new BindingSource(AuthorList, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";

            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = "HH:mm:ss";

            dateTimePicker4.Format = DateTimePickerFormat.Custom;
            dateTimePicker4.CustomFormat = "HH:mm:ss";
            label2.Size = new System.Drawing.Size(200, 26);
            label2.AutoSize = true;
            progressBar1.Minimum = 0;
            
            //progressBar1.Maximum = 100;
        }
        private DateTimePicker timePicker;
        public string networkPath = @"\\172.21.20.92\d$";
        NetworkCredential credentials = new NetworkCredential(@"{User Name}", "{Password}");
        public string myNetworkPath = string.Empty;
        private void button1_Click(object sender, EventArgs e)
        {
            string fname = textBox1.Text;
            if (fname == "")
            {
                //MessageBox.Show("Please Input Folder Name");
                int x = this.Width;
                
                Point fx = this.Location;
                FindAndMoveMsgBox(fx.X +x/3, fx.Y, true, "Alert");
                MessageBox.Show("Please Input Folder Name", "Alert");
            }
            else
            {
                //Process.Start("explorer.exe", @"C:\Local\");
                //for get IP address from client
                string selected = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
                string iptarget = "";
                switch (selected)
                {
                    case "CB":
                        iptarget = @"\\172.21.86.150\d$\PICT\";
                        break;
                    case "CS":
                        iptarget = @"\\172.21.86.47\d$\PICT\";
                        break;
                    case "CS SCH":
                        iptarget = @"\\172.21.86.48\d$\PICT\";
                        break;
                    case "DSS":
                        iptarget = @"\\172.21.86.50\d$\PICT\";
                        break;
                    case "DS":
                        iptarget = @"\\172.21.86.44\d$\PICT\";
                        break;
                    case "MRK":
                        iptarget = @"\\172.21.86.56\d$\PICT\";
                        break;
                    case "INS":
                        iptarget = @"\\172.21.86.52\d$\PICT\";
                        break;
                    case "CL":
                        iptarget = @"\\172.21.86.58\d$\PICT\";
                        break;
                }
                string dest = @"D:\"+ fname+ @"\";
               
                // For get Date
                string theDate1 = dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + dateTimePicker3.Value.ToString("HH:mm:ss");
                string theDate2 = dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + dateTimePicker4.Value.ToString("HH:mm:ss");
                DateTime dt = DateTime.Parse(theDate1);
                DateTime dt2 = DateTime.Parse(theDate2);
                TimeSpan rt = dt2 - dt;
                int tothour = rt.Days * 24 * 60 * 60 + rt.Hours * 60 * 60 + rt.Minutes * 60 + rt.Seconds;
                progressBar1.Maximum = tothour;
                string dx = "";
                if (tothour > 0)
                {
                    
                    //Directory.CreateDirectory(dest);
                    for (int i = 1; i <= tothour; i++)
                    {
                        DateTime d2 = dt.AddSeconds(i);
                        string ssname = "Cap " + d2.ToString("yyyy-MM-dd HH-mm-ss")+".png";
                        string target = iptarget + ssname;
                        //if (File.Exists(target))
                        //{
                        //    File.Copy(target, dest + ssname, true);
                        //}
                        //ExecuteSecure(() => label2.Text = "Customer Id " + i);
                        label2.Text = "counting " + i;
                        label2.Refresh();
                        progressBar1.Value = i;
                        progressBar1.BackColor = Color.Red;

                    }
                    MessageBox.Show("Copy Done :" + tothour);
                }
                else
                {
                    MessageBox.Show("Please Check Date and Time");

                }
                
               
            }
            
        }

        public class ConnectToSharedFolder : IDisposable
        {
            readonly string _networkName;

            public ConnectToSharedFolder(string networkName, NetworkCredential credentials)
            {
                _networkName = networkName;

                var netResource = new NetResource
                {
                    Scope = ResourceScope.GlobalNetwork,
                    ResourceType = ResourceType.Disk,
                    DisplayType = ResourceDisplaytype.Share,
                    RemoteName = networkName
                };

                var userName = string.IsNullOrEmpty(credentials.Domain)
                    ? credentials.UserName
                    : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

                var result = WNetAddConnection2(
                    netResource,
                    credentials.Password,
                    userName,
                    0);

                if (result != 0)
                {
                    throw new Win32Exception(result, "Error connecting to remote share");
                }
            }

            ~ConnectToSharedFolder()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                WNetCancelConnection2(_networkName, 0, true);
            }

            [DllImport("mpr.dll")]
            private static extern int WNetAddConnection2(NetResource netResource,
                string password, string username, int flags);

            [DllImport("mpr.dll")]
            private static extern int WNetCancelConnection2(string name, int flags,
                bool force);

            [StructLayout(LayoutKind.Sequential)]
            public class NetResource
            {
                public ResourceScope Scope;
                public ResourceType ResourceType;
                public ResourceDisplaytype DisplayType;
                public int Usage;
                public string LocalName;
                public string RemoteName;
                public string Comment;
                public string Provider;
            }

            public enum ResourceScope : int
            {
                Connected = 1,
                GlobalNetwork,
                Remembered,
                Recent,
                Context
            };

            public enum ResourceType : int
            {
                Any = 0,
                Disk = 1,
                Print = 2,
                Reserved = 8,
            }

            public enum ResourceDisplaytype : int
            {
                Generic = 0x0,
                Domain = 0x01,
                Server = 0x02,
                Share = 0x03,
                File = 0x04,
                Group = 0x05,
                Network = 0x06,
                Root = 0x07,
                Shareadmin = 0x08,
                Directory = 0x09,
                Tree = 0x0a,
                Ndscontainer = 0x0b
            }
        }
        private void ExecuteSecure(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => action()));
            }
            else
            {
                action();
            }
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //MessageBox.Show("Enter key pressed");
                //textBox1.AutoCompleteMode();
                //textBox1.Focus();
                //e.Handled = true;
                button1.PerformClick();
                //if (sender is Control)
                //{
                //    // Move to next control
                //    SelectNextControl((Control)sender, true, true, true, true);
                //}
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //MessageBox.Show("Enter key pressed");
                //textBox1.AutoCompleteMode();
                //textBox1.Focus();
                //e.Handled = true;
                button1.PerformClick();
                //if (sender is Control)
                //{
                //    // Move to next control
                //    SelectNextControl((Control)sender, true, true, true, true);
                //}
            }
        }

        void FindAndMoveMsgBox(int x, int y, bool repaint, string title)
        {
            Thread thr = new Thread(() => // create a new thread
            {
                IntPtr msgBox = IntPtr.Zero;
                // while there's no MessageBox, FindWindow returns IntPtr.Zero
                while ((msgBox = FindWindow(IntPtr.Zero, title)) == IntPtr.Zero) ;
                // after the while loop, msgBox is the handle of your MessageBox
                Rectangle r = new Rectangle();
                GetWindowRect(msgBox, out r); // Gets the rectangle of the message box
                MoveWindow(msgBox /* handle of the message box */, x, y,
                   r.Width - r.X /* width of originally message box */,
                   r.Height - r.Y /* height of originally message box */,
                   repaint /* if true, the message box repaints */);
            });
            thr.Start(); // starts the thread
        }
    }
}
