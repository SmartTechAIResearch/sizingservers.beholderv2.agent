using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace sizingservers.beholderv2.agent.linux {
    internal static class RetrieverProxy {
        private static string _thisDirectory;

        /// <summary>
        /// Determines whether this instance is vm.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is vm; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsVM() {
            string output = GetBashStdOutput("dmesg | grep -i 'hypervisor detected:'");
            return output.Trim().Length != 0;
        }
        static RetrieverProxy() {
            _thisDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string tempPath = Path.Combine(_thisDirectory, "temp");
            while (File.Exists(tempPath)) {
                File.Delete(tempPath);
                tempPath += "_";
            }
        }
        /// <summary>
        /// Gets the name of the DNS domain.
        /// </summary>
        /// <returns></returns>
        public static string GetDnsDomainName() {
            return GetBashStdOutput("dnsdomainname");
        }
        /// <summary>
        /// Gets the cpu information.
        /// </summary>
        /// <returns></returns>
        public static CpuInfo GetCpuInfo() {
            string[] output = GetBashStdOutput("lscpu | egrep 'Socket(s):|Model name:'").Split('\n');

            return new CpuInfo() {
                Count = int.Parse(output[0].Split(':')[1].Trim()),
                Name = output[1].Split(':')[1].Trim()
            };
        }
        private static string GetBashStdOutput(string command) {
            var startInfo = new ProcessStartInfo("/bin/bash") {
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true

            };
            var p = Process.Start(startInfo);
            string s = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return s;
        }
        /// <summary>
        /// Gets the inxi key value pairs per category.
        /// </summary>
        /// <param name="inxiArgs">The inxi arguments e.g. -SCDMNm -xi. Do not use -c 0 (no color) since the color info is used to get values correctly. 
        /// Inxi is a script to easily retrieve system info. Install it using a packagemanager like apt(itude) or manually and puth in in PATH.</param>
        /// <returns>
        /// <para>Category e.g Memory, Keys, Values e.g. Speed in Mhz. A component (key) can have sub info. Since those keys are not unique in the output they are started with the component key. e.g. Device-1, Device-1 size, Device-1 speed</para>
        /// <para>Always use .GetValueOrDefault to ensure a value (null) is returned even if the key is not found.</para>
        /// </returns>
        public static Dictionary<string, Dictionary<string, string>> GetInxiInfo(string inxiArgs) {
            var d = new Dictionary<string, Dictionary<string, string>>();

            //ESC --> char(27)
            //ESC[1;34mSystem:   ESC[0;37m ESC[1;34mHost:ESC[0;37m WorkshopIoTFest ESC[1;34mKernel:ESC[0;37m 4.4.0-78-generic x86_64 (64 bit) ESC[1;34mDesktop:ESC[0;37m Unity 7.4.0ESC[0;37m
            //ESC[1;34m          ESC[0;37m ESC[1;34mDistro:ESC[0;37m Ubuntu 16.04 xenialESC[0;37m
            //ESC[0m
            //
            //Key between ESC[1;34m and ESC[0;37m. This text can be whitespace and should be ommited. 
            //Value between ESC[0;37m and ESC[1;34m or last delimiter can be ESC[0;37m\n at the end of a sentence.
            //\nESC[m0 at the end can be ommited.

            string startColor = (char)27 + "[1;34m";
            string stopColor = (char)27 + "[0;37m";
            string end = (char)27 + "[0m";

            string beginKey = "_._begin_._";
            string endKey = "_._end_._";

            string info = GetInxiOutput(inxiArgs).Replace(stopColor + "\n", "").Replace(startColor, beginKey).Replace(stopColor, endKey).Replace(end, beginKey).Trim();

            if (info.Contains(endKey + " " + endKey))
                throw new Exception("Error parsing inxi output. Run with root privileges?\noutput:" + info);

            Dictionary<string, string> currentKvps = null;
            string currentComponentKey = null;
            while (info.Length != 0) {
                string key = info.StartsWith(beginKey) ? GetStringBetween(info, beginKey, endKey, out info) : GetStringBetween(info, endKey, out info);
                string value = GetStringBetween(info, beginKey, out info);
                info = info.Trim();
                key = key.Replace(":", "").Trim();
                if (key.Length == 0) continue;
                value = value.Trim();

                if (value.Length == 0) {
                    currentKvps = new Dictionary<string, string>();
                    d.Add(key, currentKvps);
                }
                else {
                    if (char.IsUpper(key[0])) {
                        while (currentKvps.ContainsKey(key)) key += "_"; //Avoid multiple entries.
                        currentComponentKey = key;
                    }
                    else {
                        key = currentComponentKey + " " + key;
                    }
                    currentKvps.Add(key, value);
                }
            }

            return d;
        }
        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <param name="inxiArgs">The inxi arguments e.g. -SCDMNm -xi. Do not use -c 0 (no color) since the color info is used to get values correctly. 
        /// Inxi is a script to easily retrieve system info. Install it using a packagemanager like apt(itude) or manually and puth in in PATH.</param>
        /// <returns></returns>
        private static string GetInxiOutput(string inxiArgs) {
            string tempPath = Path.Combine(_thisDirectory, "temp");
            while (File.Exists(tempPath)) tempPath += "_";

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
            Process.Start(startInfo).WaitForExit();

            string output = string.Empty;
            using (var sr = new StreamReader(new FileStream(tempPath, FileMode.Open)))
                output = sr.ReadToEnd();

            File.Delete(tempPath);

            return output;
        }
        /// <summary>
        /// Gets the string between 0 and the index of end.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="end">The end.</param>
        /// <param name="trimmedInput">
        /// <para>input without the "string between" and the given delimiters. e.g. input: foo;b_ar begin: ; end: _ string between: b trimmedInput: fooar.</para>
        /// <para>Useful for when you want to get multiple string betweens from a text: use the trimmedInput as a new input to make text searches faster.</para>
        /// </param>
        /// <returns></returns>
        public static string GetStringBetween(string input, string end, out string trimmedInput) { return GetStringBetween(input, "", end, out trimmedInput); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="trimmedInput">
        /// <para>input without the "string between" and the given delimiters. e.g. input: foo;b_ar begin: ; end: _ string between: b trimmedInput: fooar.</para>
        /// <para>Useful for when you want to get multiple string betweens from a text: use the trimmedInput as a new input to make text searches faster.</para>
        /// </param>
        /// <returns></returns>
        public static string GetStringBetween(string input, string begin, string end, out string trimmedInput) {
            trimmedInput = input;

            int startIndex = begin.Length == 0 ? 0 : input.IndexOf(begin);
            int length = input.Substring(startIndex + begin.Length).IndexOf(end);

            if (startIndex == -1 || length == -1)
                return string.Empty;

            trimmedInput = trimmedInput.Substring(0, startIndex) + trimmedInput.Substring(startIndex + begin.Length + length + end.Length);

            return input.Substring(startIndex + begin.Length, length);
        }

        /// <summary>
        /// /
        /// </summary>
        public struct CpuInfo {
            public int Count { get; set; }
            public string Name { get; set; }
        }
    }
}
