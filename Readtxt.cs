using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Dictation
{
    class Readtxt
    {
        public List<string> strings = new List<string>();
        public string Greetings;
        public string EndOfText;
        public string Saved;
        public string Help;
        public string bye;
        public string stop;
        public string reading;
        public string writing;
        public List<string> offer = new List<string>();
        public List<string> phrase = new List<string>();
        public int CurrentOffer = 0;
        public int CurrentPhrase = 0;
        // Constructor, loads Replics from the Replics.txt
        public Readtxt()
        {
            
        }
        // Reading lines from path
        public void readlines(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(file, Encoding.Default);
            while (reader.Peek() >= 0)
                strings.Add(reader.ReadLine());
            reader.Close();
            file.Close();
        }

        //Read offers from Text
        public void readoffers(string Text)
        {

            string str = string.Empty;
            for (int i = 0; i < Text.Length; i++)
            {
                
                    str += Text[i].ToString();
                if
                    (Text[i].ToString() == "."
                  || Text[i].ToString() == "!"
                  || Text[i].ToString() == "?")
                {
                    offer.Add(str);
                    str = string.Empty;
                }
            }
        }
        //reading phrases from file
        public void readphrases()
        {
            readlines("DictationText\\Replics.txt");

            #region greetings
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<greetings>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</greetings>")) { break; }
                        Greetings += strings[j];
                    }

                }
            }
            #endregion greetings
            #region End of Text
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<end of text>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</end of text>")) { break; }
                        EndOfText += strings[j];
                    }
                }
            }

            #endregion End of Text
            #region saved
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<saved>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</saved>")) { break; }
                        Saved += strings[j];
                    }
                }
            }
            #endregion saved
            #region help
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<help>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</help>")) { break; }
                        Help += strings[j];
                    }
                }
            }
            #endregion help
            #region bye
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<bye>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</bye>")) { break; }
                        bye += strings[j];
                    }
                }
            }
            #endregion bye;
            #region stop
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<stop>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</stop>")) { break; }
                        stop += strings[j];
                    }
                }
            }
            #endregion stop
            #region reading
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<reading>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</reading>")) { break; }
                        reading += strings[j];
                    }
                }
            }
            #endregion reading
            #region writing
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<writing>"))
                {
                    for (int j = i + 1; j < strings.Count; j++)
                    {
                        if (strings[j].Contains("</writing>")) { break; }
                        writing += strings[j];
                    }
                }
            }
            #endregion writing
            strings.Clear();
        }
    }
}
