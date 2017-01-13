using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Dictation
{
    class Phraser
    {
        public List<string> list = new List<string>();
        public void loadtext(string path)
        { 
            FileStream file = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(file, Encoding.UTF8);
            string[] text = reader.ReadToEnd().Replace("\r\n", "\n").Split('\n');
            reader.Close();
            file.Close();
            foreach (string str in text)
            {
                list.Add(str);
            }
        }

    }
}
