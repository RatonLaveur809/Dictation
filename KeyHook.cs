using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace Dictation
{
    class KeyHook
    {
        public bool ctrlhook = false;
        public bool hook = false;
        public string Hkey="";
        string prevkey = null;

        public void Keyhook(char key)
        
        {
            
            byte b = Encoding.GetEncoding(1251).GetBytes(new char[] { key })[0];
            if ((b >= 192 && b <= 255)
                || b == 63
                || b == 44
                || b == 45
                || b == 46
                || b == 33
                || b == 58
                || b == 59
                || (b >= 48 && b <= 57)
                || b == 184
                || b == 32)

            {    
                Hkey = "";
                hook = true;
                if (key.ToString() == prevkey)
                {
                    Hkey = key.ToString();
                    prevkey = null;
                    return;
                }
                prevkey = key.ToString();
                
            }
            else
            {
            prevkey = "";
            Hkey = "";
                }
            
        }
        public String BuildSSML()
        {
            if (prevkey == " ")
            {
                StringBuilder space = new StringBuilder();
                space.Append("<speak xml:lang=''ru-RU'' version=''1.0''>");
                space.Append("<voice>" + "пробел" + "</voice></speak>");
                return space.ToString().Replace("''", '"'.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<speak xml:lang=''ru-RU'' version=''1.0''>");
                sb.Append("<voice><say-as interpret-as=''letters''>" + prevkey + "</say-as></voice></speak>");
                return sb.ToString().Replace("''", '"'.ToString());
            }
        }
    }
          
}

