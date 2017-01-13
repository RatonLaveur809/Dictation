using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech;
using SpeechLib;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Dictation_new
{
    public partial class SetVoiceForm : Form
    {
        private readonly List<string> list = new List<string>();
        string dialog = string.Empty;
        int count = 10;
        bool flag = false;

        public SpVoice speech = new SpVoice();
        SpeechVoiceSpeakFlags Async = SpeechVoiceSpeakFlags.SVSFlagsAsync;
        SpeechVoiceSpeakFlags Sync = SpeechVoiceSpeakFlags.SVSFDefault;
        SpeechVoiceSpeakFlags Cancel = SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak;
        ISpeechObjectTokens Sotc;
        //private System.Windows.Forms.Timer timer1;
        //private IContainer components;

        Phraser Defvoice = new Phraser();

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);


        //Declaring Global objects
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        public SetVoiceForm()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule; //Get Current Module
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey); //Assign callback function each time keyboard process
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0); //Setting Hook of Keyboard Process for current module

            InitializeComponent();
            Sotc = speech.GetVoices("", "");
            speech.Rate = 1;
            speech.Volume = 100;
            

            
            Defvoice.loadtext(Application.StartupPath + "\\DefVoice.txt");



        }

        void button_KeyDown(object sender, KeyEventArgs e)
        {
            speech.Speak("", Cancel);
            speech.Speak(dialog, Async);
        }

        void button_Leave(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            count = 10;
            timer1.Stop();
        }

        void button_Enter(object sender, EventArgs e)
        {
            if (flag == true)
            {
                timer1.Start();
            }
            ((Button)sender).BackColor = System.Drawing.Color.Red;
            speech.Voice = Sotc.Item(((Button)sender).TabIndex);
            speech.Speak("", Cancel);
            speech.Speak(((Button)sender).Text, Async);
        }

        void button_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            DialogResult = DialogResult.OK;

        }

        private void SetVoiceForm_Shown(object sender, EventArgs e)
        {

            foreach (ISpeechObjectToken Sot in Sotc)
            {
                var button = new Button()
                {
                    Text = Sot.GetDescription(0),
                    Font = new Font("Microsoft Sans Serif", 36, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    Location = new Point(10, 50 + list.Count * 70),
                    FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                    AutoSize = true,

                };

                button.Click += new EventHandler(button_Click);
                button.Enter += new EventHandler(button_Enter);
                button.Leave += new EventHandler(button_Leave);
                button.KeyDown += new KeyEventHandler(button_KeyDown);
                Controls.Add(button);
                list.Add(Sot.GetDescription(0));

                if (list[list.Count - 1] == Defvoice.list[0])
                {
                    speech.Voice = Sotc.Item(list.Count - 1);
                    ActiveControl = button;
                }

            }

            speech.Speak(dialog, Async);

            flag = true;
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = (count).ToString();
            count--;
            if (count == 0)
            {
                timer1.Stop();
                this.Close();
            }
        }


        private void SetVoiceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            speech.Speak("", Cancel);
            SaveSelectedVoice();
        }

        private void SaveSelectedVoice()
        {
            File.Delete(Application.StartupPath + "\\SelectedVoice.txt");
            FileStream file = new FileStream(Application.StartupPath + "\\SelectedVoice.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(speech.Voice.GetDescription(0));
            writer.Close();
            file.Close();
        }

        private void SetVoiceForm_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

             

    }
}
