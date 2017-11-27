using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace sizingservers.beholderv2.agent.linux {
    internal static class RetrieverProxy {
        private static string _thisDirectory;

        static RetrieverProxy() {
            _thisDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string tempPath = Path.Combine(_thisDirectory, "temp");
            while (File.Exists(tempPath)) {
                File.Delete(tempPath);
                tempPath += "_";
            }            
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <param name="inxiArgs">The inxi arguments e.g. -SCDMNm -xi -c 0. Inxi is a script to easily retrieve system info.</param>
        /// <returns></returns>
        public static string GetInfo(string inxiArgs) {
            string tempPath = Path.Combine(_thisDirectory, "temp");
            while (File.Exists(tempPath))
                tempPath += "_";

            //S(ystem)  --> host, kernel, desktop, distro
            //D(rives)  --> hdd total size, id, model, size in GB
            //M(achine) --> system( manufacturer), (system )product, (system )v(ersion), mobo( manufacturer), (mobo )model, (mobo )v(ersion), bios( manufacturer), (bios )v(ersion), (bios )date
            //N(etwork) --> card (name), driver
            //m(emory)  --> Device-# (name), size, speed, type (from dmidecode --> root privileges)

            //xi        --> extended info above,  N + ipv6 info
            //c 0       --> no color
            var startInfo = new ProcessStartInfo("inxi " + inxiArgs + " > '" + tempPath + "'") {
                UseShellExecute = true
            };
            var p = Process.Start(startInfo);
            p.WaitForExit();

            string output = string.Empty;
            using (var sr = new StreamReader(new FileStream(tempPath, FileMode.Open)))
                output = sr.ReadToEnd();

            File.Delete(tempPath);

            return output;
        }
    }
}
