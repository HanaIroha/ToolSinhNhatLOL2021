using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Toolcodell
{
    public partial class Form1 : Form
    {
        static List<string> logList;
        static int index = 0;
        static int allz;
        Thread th;
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

#pragma warning disable 649
        internal struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

#pragma warning restore 649


        public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
        {
            var oldPos = Cursor.Position;

            /// get screen coordinates
            ClientToScreen(wndHandle, ref clientPoint);

            /// set cursor on coords, and press mouse
            Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

            var inputMouseDown = new INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            var inputMouseUp = new INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            /// return mouse 
            Cursor.Position = oldPos;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Action DoWorkTool = () =>
            {
                var all = Process.GetProcessesByName("LeagueClientUx");
                IntPtr MainWindowz = all[0].MainWindowHandle;
                foreach (var z in all)
                {
                    IntPtr h = z.MainWindowHandle;
                    ShowWindow(h, 10);
                    SetForegroundWindow(h);
                    MainWindowz = h;
                }
                for (; ; )
                {
                    label1.Text = "Tổng mã: " + index + "/" + allz;
                    Thread.Sleep(500);
                    if (index < allz)
                    {
                        ClickOnPoint(MainWindowz, new Point(617, 531));
                        Thread.Sleep(500);
                        SendKeys.SendWait("^{a}");
                        Clipboard.Clear();
                        Clipboard.SetText(logList[index]);
                        index++;
                        Thread.Sleep(250);
                        SendKeys.SendWait("^{v}");
                        Thread.Sleep(500);
                        ClickOnPoint(MainWindowz, new Point(773, 526));
                        Thread.Sleep(500);
                        ClickOnPoint(MainWindowz, new Point(485, 430));
                        Thread.Sleep(500);
                        ClickOnPoint(MainWindowz, new Point(485, 430));
                    }
                    else
                    {
                        MessageBox.Show("Đã hết mã!");
                        break;
                    }
                }
            };
            th = new Thread(new ThreadStart(DoWorkTool));
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            th.Abort();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var logFile = File.ReadAllLines("code.txt");
            logList = new List<string>(logFile);
            allz = logList.Count();
            label1.Text = "Tổng mã: 0/"+ allz;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                th.Abort();
            }
            catch
            {

            }
        }
    }
}
