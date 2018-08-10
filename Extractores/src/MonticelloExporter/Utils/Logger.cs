using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Logger
    {
        public static void Write(string text, string filePath = "MonticelloExporter.log")
        {
            text = DateTime.Now + " | " + text;

            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                file.Close();

                var tw = new StreamWriter(filePath, true);

                tw.WriteLine(text);
                tw.Close();
            }
            else if (File.Exists(filePath))
            {
                using (var tw = new StreamWriter(filePath, true))
                {
                    tw.WriteLine(text);
                    tw.Close();
                }
            }
        }
    }
}
