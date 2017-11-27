﻿/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Reports system information. It uses the info from Config. You need to register a retreiver.
    /// </summary>
    public static class PayloadReporter {
        private static Timer _reportTimer;
        private static IPayloadRetriever _retreiver;

        private static HttpClient _httpClient = new HttpClient();

        public static void RegisterRetreiverAndStartReporting(IPayloadRetriever retreiver) {
            _retreiver = retreiver;
            _reportTimer = new Timer(_reportTimer_Callback, null, 0, Config.GetInstance().reportEveryXMinutes * 60 * 1000);
        }
        async static void _reportTimer_Callback(object state) {
            try {
                if (_retreiver == null) return;

                ComponentGroup[] report = null;

                for (int i = 0; ;)
                    try {
                        report = _retreiver.Retrieve().ToArray();
                        break;
                    }
                    catch {
                        if (++i == 3) throw;
                        Task.Delay(i * 100).Wait();
                    }

                string json = JsonConvert.SerializeObject(report);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine(DateTime.Now.ToString("yyyy\"-\"MM\"-\"dd\" \"HH\":\"mm\":\"ss") + " - Reporting: " + json);
                Console.WriteLine();

                await _httpClient.PostAsync(Config.GetInstance().endpoint + "/api/report?apiKey=" + Config.GetInstance().apiKey, content);
            }
            catch (Exception ex) {
                ConsoleColor c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString("yyyy\"-\"MM\"-\"dd\" \"HH\":\"mm\":\"ss") + " - Failed:\n" + ex);
                Console.WriteLine();
                Console.ForegroundColor = c;
            }
        }
    }
}
