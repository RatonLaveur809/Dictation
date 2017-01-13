using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SpeechLib;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;

namespace Dictation
{

    public partial class Form1 : Form
    {
        Wordreader wordreader = new Wordreader();
        SpeechVoiceSpeakFlags Async = SpeechVoiceSpeakFlags.SVSFlagsAsync;
        SpeechVoiceSpeakFlags Sync = SpeechVoiceSpeakFlags.SVSFDefault;
        SpeechVoiceSpeakFlags Cancell = SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak;
        SpVoice speech = new SpVoice();
        SpeechSynthesizer speaker = new SpeechSynthesizer();

         FileStream Hfile;
         StreamWriter Hwriter;
         FileStream Tfile;
         StreamWriter Twriter;
         
        Readtxt readtxt = new Readtxt();
        KeyHook Keyhook = new KeyHook();
        Wordreader reader = new Wordreader();
        
        bool ctrlhook = false;
        string History = string.Empty;
        string Time = string.Empty;

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


        public Form1()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule; //Get Current Module
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey); //Assign callback function each time keyboard process
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0); //Setting Hook of Keyboard Process for current module
            InitializeComponent();
            SetVoiceForm SVF = new SetVoiceForm();
            SVF.ShowDialog();

            try
            {
                speech.Rate = 0;
                speech.Volume = 100;
                speech.Voice = SVF.speech.Voice;

            }

            catch (Exception) { }

            File.Delete(Application.StartupPath + "\\DictationText\\History.txt");
            Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.OpenOrCreate);
            Hwriter = new StreamWriter(Hfile);

           

            timer1.Start();
            
            //label1.Text = speaker.Voice.Name;
            

            readtxt.readphrases();
            readtxt.readlines("DictationText\\Text.txt");
            foreach (string str in readtxt.strings )
            {
                textBox3.Text += str + Environment.NewLine;
            }
            readtxt.readoffers(textBox3.Text);
            textBox1.Text = readtxt.offer[readtxt.CurrentOffer];
            speech.Speak(readtxt.Greetings);
            ActiveControl = textBox2;
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }
              
       

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            #region Help
            if (e.KeyCode.Equals(Keys.F1))
            {
                speech.Speak("", Cancell);
                speech.Speak(readtxt.Help, Async);
                Time = System.DateTime.Now.ToLongTimeString();
                History = Time + " F1 - справка" + Environment.NewLine;
                Hfile.Close();
                Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                Hwriter = new StreamWriter(Hfile);
                Hwriter.Write(History);
                Hwriter.Close();
            }
            #endregion Help

            e.Handled = true;
            e.SuppressKeyPress = true;

            //read next offer
            #region next offer
            if (e.KeyCode.Equals(Keys.F6))
            {
                ctrlhook = true;
                speech.Speak ("",Cancell);
                try
                {

                    readtxt.CurrentOffer++; 
                    speech.Speak(readtxt.offer[readtxt.CurrentOffer], Async);
                    textBox1.Text = readtxt.offer[readtxt.CurrentOffer];
                    Time = System.DateTime.Now.ToLongTimeString();
                    History = Time + " F6 - следующая фраза" + Environment.NewLine;
                    Hfile.Close();
                    Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                    Hwriter = new StreamWriter(Hfile);
                    Hwriter.Write(History);
                    Hwriter.Close();
                }
                catch (ArgumentOutOfRangeException) { speech.Speak(readtxt.EndOfText, Async); }
            }
            #endregion next offer

            if (e.KeyCode.Equals(Keys.F4))
            {
                ctrlhook = true;
                speech.Speak("", Cancell);
                if (readtxt.CurrentOffer != 0)
                {
                    readtxt.CurrentOffer--;
                    speech.Speak(readtxt.offer[readtxt.CurrentOffer], Async);
                    textBox1.Text = readtxt.offer[readtxt.CurrentOffer];
                    Time = System.DateTime.Now.ToLongTimeString();
                    History = Time + " F4 - предыдущая фраза" + Environment.NewLine;
                    Hfile.Close();
                    Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                    Hwriter = new StreamWriter(Hfile);
                    Hwriter.Write(History);
                    Hwriter.Close();
                }
               
               
            }

            if (e.KeyCode.Equals(Keys.F8))
            {
                ctrlhook = true;
                speech.Speak("", Cancell);
                Time = System.DateTime.Now.ToLongTimeString();
                History = Time + " F8 - стоп" + Environment.NewLine;
                Hfile.Close();
                Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                Hwriter = new StreamWriter(Hfile);
                Hwriter.Write(History);
                Hwriter.Close();
            }

            
                

              if (e.KeyCode.Equals(Keys.F7))
                    {
                        speech.Speak("", Cancell);
                        speech.Speak(textBox2.Text, Async);
                        Time = System.DateTime.Now.ToLongTimeString();
                        History = Time + " F7 - чтение написанного" + Environment.NewLine;
                        Hfile.Close();
                        Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                        Hwriter = new StreamWriter(Hfile);
                        Hwriter.Write(History);
                        Hwriter.Close();
                    }

                if (e.KeyCode.Equals(Keys.F3))
                {
                    speech.Speak ("",Cancell);
                    speech.Speak (textBox3.Text, Async);
                    Time = System.DateTime.Now.ToLongTimeString();
                    History += Time+" F3 - чтение всего текста" + Environment.NewLine;
                    Hfile.Close();
                    Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                    Hwriter = new StreamWriter(Hfile);
                    Hwriter.Write(History);
                    Hwriter.Close();
                } 


                    if (e.KeyCode.Equals(Keys.F5))
                    {

                        speech.Speak("", Cancell);
                        speech.Speak(readtxt.offer[readtxt.CurrentOffer], Async);
                        Time = System.DateTime.Now.ToLongTimeString();
                        History = Time + " F5 - чтение текущей фразы" + Environment.NewLine;
                        Hfile.Close();
                        Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                        Hwriter = new StreamWriter(Hfile);
                        Hwriter.Write(History);
                        Hwriter.Close();

                    }

                    if (e.KeyCode.Equals(Keys.F9))
                    {
                        try {
                        speech.Rate--;                        
                        Time = System.DateTime.Now.ToLongTimeString();
                        History = Time + " F9 - понижение темпа" + Environment.NewLine;
                        Hfile.Close();
                        Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                        Hwriter = new StreamWriter(Hfile);
                        Hwriter.Write(History);
                        Hwriter.Close();
                        }
                        catch (ArgumentOutOfRangeException) { }

                    }

                    if (e.KeyCode.Equals(Keys.F10))
                    {

                        try {
                        speech.Rate++;
                        Time = System.DateTime.Now.ToLongTimeString();
                        History = Time + " F10 - повышение темпа" + Environment.NewLine;
                        Hfile.Close();
                        Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                        Hwriter = new StreamWriter(Hfile);
                        Hwriter.Write(History);
                        Hwriter.Close();
                        }
                        catch (ArgumentOutOfRangeException) { }

                    }
           

            #region view text
            if (e.KeyCode.Equals(Keys.F2))
            {
                label2.Visible = true;
                textBox1.Visible = true;
                this.textBox1.Select();
                this.textBox1.SelectAll();
                this.textBox1.DeselectAll();
            }
            if (e.Alt && e.KeyCode.Equals(Keys.F2))
            {
                label2.Visible = false;
                textBox1.Visible = false;
                textBox2.Focus();
            }
            #endregion view text

            #region delete

            if (e.KeyCode.Equals(Keys.Delete))
            {
                speech.Speak("", Cancell);
                speech.Speak("Удалить", Async);
                Time = System.DateTime.Now.ToLongTimeString();
                History = Time + " Удаление" + Environment.NewLine;
                Hfile.Close();
                Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                Hwriter = new StreamWriter(Hfile);
                Hwriter.Write(History);
                Hwriter.Close();

            }
            #endregion delete


            #region letters

            if (e.KeyCode == Keys.Left
                || e.KeyCode == Keys.Right
                || e.KeyCode == Keys.Up
                || e.KeyCode == Keys.Down)
            {
                if (ActiveControl == textBox2)
                {
                    if (ctrlhook == false)
                    {
                        speech.Speak("", Cancell);
                        if (textBox2.SelectionStart == textBox2.Text.Length)
                        {
                            speech.Speak("Конец текста", Async);
                            return;
                        }
                        speech.Speak("<spell>" + textBox2.Text[textBox2.SelectionStart] + "</spell>", Async);

                        string t = textBox2.Text[textBox2.SelectionStart].ToString();
                        if (t == " ")
                            speech.Speak("Пробел", Async);


                        if (e.KeyCode == Keys.Left)
                        {
                            Time = System.DateTime.Now.ToLongTimeString();
                            History = Time + "<-" + Environment.NewLine;
                            Hfile.Close();
                            Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                            Hwriter = new StreamWriter(Hfile);
                            Hwriter.Write(History);
                            Hwriter.Close();
                        }

                        if (e.KeyCode == Keys.Right)
                        {
                            Time = System.DateTime.Now.ToLongTimeString();
                            History = Time + "->" + Environment.NewLine;
                            Hfile.Close();
               Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
               Hwriter = new StreamWriter(Hfile);
               Hwriter.Write(History);
               Hwriter.Close();
                        }

                    }
                }

                if (ActiveControl == textBox1)
                {
                    if (ctrlhook == false)
                    {
                        speech.Speak("", Cancell);
                        if (textBox1.SelectionStart == textBox1.Text.Length)
                        {
                            speech.Speak("Конец текста", Async);
                            return;
                        }
                        speech.Speak("<spell>" + textBox1.Text[textBox1.SelectionStart] + "</spell>", Async);

                        string t = textBox1.Text[textBox1.SelectionStart].ToString();
                        if (t == " ")
                            speech.Speak("Пробел", Async);

                    }
                }

                
            }
         
                #endregion letters

            #region words

            if ((e.Control && e.KeyCode == Keys.Up) || (e.Control && e.KeyCode == Keys.Down) || (e.Control && e.KeyCode == Keys.Left) || (e.Control && e.KeyCode == Keys.Right))
            {
                ctrlhook = true;
                if (ActiveControl == textBox2)
                reader.readword(textBox2);
                else
                    reader.readword(textBox1);
                speech.Speak("", Cancell);
                speech.Speak(reader.currword, Async);
            }
            #endregion words

           


            if (e.KeyCode == Keys.Enter
              || e.KeyCode == Keys.Back
              || e.KeyCode == Keys.OemQuestion
              || e.KeyCode == Keys.OemPipe
              || e.KeyCode == Keys.Escape)
            {
                Keyhook.hook = true;
            }

            ctrlhook = false;

            //rewrite result
            File.Delete(Application.StartupPath + "\\DictationText\\Result.txt");
            Tfile = new FileStream(Application.StartupPath + "\\DictationText\\Result.txt", FileMode.OpenOrCreate);
            Twriter = new StreamWriter(Tfile, UnicodeEncoding.Default);
            Twriter.Write(textBox2.Text.ToString());
            Twriter.Close();
            Tfile.Close();
            

        }
        
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
         

            Keyhook.Keyhook(e.KeyChar);
           
           Time = System.DateTime.Now.ToLongTimeString();
           if (e.KeyChar.ToString() == " ")
           {
               History = Time + " " + "Пробел" + Environment.NewLine;
               Hfile.Close();
               Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
               Hwriter = new StreamWriter(Hfile);
               Hwriter.Write(History);
               Hwriter.Close();
           }
           else
           {
              
                   History = Time + " " + e.KeyChar.ToString() + Environment.NewLine;
                   Hfile.Close();
                   Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
                   Hwriter = new StreamWriter(Hfile);
                   Hwriter.Write(History);
                   Hwriter.Close();
               
           }
           speech.Speak("", Cancell);
            speech.Speak(Keyhook.BuildSSML());
      }
  
        private void textBox2_Enter(object sender, EventArgs e)
        {
           
            speech.Speak ("",Cancell);
            Time = System.DateTime.Now.ToLongTimeString();
            History = Time+" Письмо" + Environment.NewLine;
            Hfile.Close();
            Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
            Hwriter = new StreamWriter(Hfile);
            Hwriter.Write(History);
            Hwriter.Close();
            speech.Speak("Ввод!", Async);
        }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e)
      {
           
          Hfile.Close();
          speech.Speak(readtxt.bye, Sync);
          
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

      private void textBox1_Enter(object sender, EventArgs e)
      {
          speech.Speak("", Cancell);
          Time = System.DateTime.Now.ToLongTimeString();
          History = Time + " Подсказка" + Environment.NewLine;
          Hfile.Close();
          Hfile = new FileStream(Application.StartupPath + "\\DictationText\\History.txt", FileMode.Append);
          Hwriter = new StreamWriter(Hfile);
          Hwriter.Write(History);
          Hwriter.Close();
          speech.Speak("Подсказка!", Async);
       
      }

   }


}
