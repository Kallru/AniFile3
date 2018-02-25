using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class Log
    {
        // Console에 찍을지 말지 결정
        public static bool UseConsole { get; set; } = true;
        public static bool UseFile { get; set; } = false;

        private static void InternalWrite(string message)
        {
            message = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + " I " + message;

            if (UseConsole)
            {
                Console.WriteLine(message);
            }

            if (UseFile)
            {
                // blabla
            }
        }

        public static void WriteLine(string message) => InternalWrite(message);
        public static void WriteLine(string format, params object[] obj) => InternalWrite(string.Format(format, obj));

        public static void Error(string message) => WriteLine("ERROR\t" + message);
        public static void Error(string format, params object[] obj) => Error(string.Format(format, obj));

        public static void Warning(string message) => WriteLine("WARNING\t" + message);
        public static void Warning(string format, params object[] obj) => Warning(string.Format(format, obj));
    }
}
