/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Reports system information. It uses the info from Config. You need to register a retriever.
    /// </summary>
    public static class PayloadReporter {
        private static Timer _reportTimer;
        private static IPayloadRetriever _retriever;

        private static HttpClient _httpClient = new HttpClient();
        /// <summary>
        /// Registers the retriever and start reporting.
        /// </summary>
        /// <param name="retriever">The retriever.</param>
        public static void RegisterRetrieverAndStartReporting(IPayloadRetriever retriever) {
            _retriever = retriever;
            _reportTimer = new Timer(_reportTimer_Callback, null, 0, Config.GetInstance().reportEveryXMinutes * 60 * 1000);
        }
        async static void _reportTimer_Callback(object state) {
            while (true)
                try {
                    ComponentGroup[] report = _retriever.Retrieve().ToArray();

                    string json = JsonConvert.SerializeObject(report);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    Console.WriteLine(DateTime.Now.ToString("yyyy\"-\"MM\"-\"dd\" \"HH\":\"mm\":\"ss") + " - Reporting: " + json);
                    Console.WriteLine();

                    await _httpClient.PostAsync(Config.GetInstance().endpoint + "/api/report?apiKey=" + Config.GetInstance().apiKey, content);
                    break;
                }
                catch (Exception ex) {
                    ConsoleColor c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(DateTime.Now.ToString("yyyy\"-\"MM\"-\"dd\" \"HH\":\"mm\":\"ss") + " - Failed:\n" + ex);
                    Console.WriteLine();
                    Console.ForegroundColor = c;
                    Thread.Sleep(1000);
                }
        }
    }
}
