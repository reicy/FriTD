using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Manager.Utils
{
    public class CustomLogger
    {
        public static bool logToFile { get; set; }
        public static string filename { get; set; }

        public static void Log(string log = "", bool newLine = true)
        {
            if (newLine)
            {
                Console.WriteLine(log);
            }
            else
            {
                Console.Write(log);
            }
            if (logToFile)
            {
                File.AppendAllText(filename, log + (newLine ? "\n" : ""));
            }
        }

        public static void ClearActualLogFile()
        {
            if (File.Exists(filename)) File.Delete(filename);
        }
    }
}
