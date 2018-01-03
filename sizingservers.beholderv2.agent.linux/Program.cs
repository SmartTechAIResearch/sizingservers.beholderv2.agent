/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using sizingservers.beholderv2.agent.shared;
using System;
using System.Threading;

namespace sizingservers.beholderv2.agent.linux {
    class Program {
        private static Mutex _namedMutex = new Mutex(true, "sizingservers.beholderv2.agent.linux");

        static void Main(string[] args) {
            if (!_namedMutex.WaitOne()) return;

            Console.WriteLine("SIZING SERVERS LAB LINUX BEHOLDER AGENT");
            Console.WriteLine("  Reporting every " + Config.GetInstance().reportEvery + " to " + Config.GetInstance().endpoint);
            Console.WriteLine();

            if (RetrieverHelper.IsVM()) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Beholder does not work for VMs!");
            }
            else {
                PayloadReporter.RegisterRetrieverAndStartReporting(PayloadRetriever.GetInstance());
            }
            Console.ReadLine();
        }
    }
}