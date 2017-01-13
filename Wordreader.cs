using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.IO;

namespace Dictation
{
    class Wordreader
    {
        public string currword = "";
        public string curroffer = "";
        public int currstart = 0;
        public int currend = 0;
        public int nextstart = 0;
        public List<string> offer = new List<string>();
        

        //Sets the current word in a string variable currword
        public void readword(TextBox ctrl)
        {
            char textstart = ctrl.Text[(ctrl.SelectionStart)];
            currword = "";
            if (char.IsPunctuation(textstart) == false && char.IsWhiteSpace(textstart) == false)
            {
                for (int i = ctrl.SelectionStart; i <= ctrl.Text.Length - 1; i++)
                {
                    if (char.IsPunctuation(ctrl.Text[i]) == false && char.IsWhiteSpace(ctrl.Text[i]) == false)
                    {
                        currword += ctrl.Text[i];
                    }
                    else break;
                }
            }
        }

        //Sets the current proposal in a string variable curroffer
        public void readoffer(TextBox ctrl, int start)
        {
            try
            {
                currstart = start;
                curroffer = "";
                int i = 0;
                for (i = currstart; i < ctrl.TextLength; i++)
                {
                    curroffer += ctrl.Text[i].ToString();
                    if (ctrl.Text[i] == '.' || ctrl.Text[i] == '!' || ctrl.Text[i] == '?')
                    {
                        if (char.IsWhiteSpace(ctrl.Text[i + 1]) || ctrl.Text[i + 1] == '\n')
                            break;
                    }
                }
                currend = i + 1;
                if (i == ctrl.TextLength)
                curroffer += " Конец текста";
                nextstart = currend;
            }
            catch (IndexOutOfRangeException) { curroffer = "Конец текста"; }
        }
        

    }
}
