/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using System;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// 
    /// </summary>
    public static class LogToConsole {
        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        public static void Info(string s) { WriteEntry(s); }
        /// <summary>
        /// </summary>
        /// <param name="ex">The exception</param>
        public static void Error(Exception ex) {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteEntry(" - Failed:\n" + ex);
            Console.ForegroundColor = c;
        }
        private static void WriteEntry(string s) {
            Console.WriteLine(DateTime.Now.ToString("yyyy\"-\"MM\"-\"dd\" \"HH\":\"mm\":\"ss") + " - " + s);
            Console.WriteLine();
        }
    }
}
