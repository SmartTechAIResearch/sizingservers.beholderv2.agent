/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    /// <summary>
    /// Caches the ManagementScopes.
    /// </summary>
    internal static class RetrieverHelper {
        private static ConcurrentDictionary<string, ManagementScope> _connectedScopes = new ConcurrentDictionary<string, ManagementScope>();

        /// <summary>
        /// Determines whether this instance is vm.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is vm; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsVM() {
            ManagementObjectCollection col = GetWmiInfo("Select Model from Win32_ComputerSystem");
            foreach (ManagementObject mo in col)
                if (mo["Model"].ToString().ToLowerInvariant().Contains("virtual"))
                    return true;

            return false;
        }

        public static string GetBMCIP() {
            // https://michlstechblog.info/blog/windows-read-the-ip-address-of-a-bmc-board/
            // https://msdn.microsoft.com/en-us/library/cc146163.aspx

            /*
                void RequestResponse(
                  [in]  uint8  Command,
                  [out] uint8  CompletionCode,
                  [in]  uint8  Lun,
                  [in]  uint8  NetworkFunction,
                  [in]  uint8  RequestData[],
                  [out] uint32 ResponseDataSize,
                  [in]  uint32 RequestDataSize,
                  [in]  uint8  ResponderAddress,
                  [out] uint8  ResponseData
                );
            */

            byte bmcResponderAddress = 0x20;
            byte getLANInfoCmd = 0x02;
            byte getChannelInfoCmd = 0x42;
            byte defaultLUN = 0x00;
            //byte ipmiProtocolType = 0x01;
            byte __8023LANMediumType = 0x04;
            byte maxChannel = 0x0b;

            byte[] requestData = null;

            var cls = new ManagementClass("root\\wmi", "microsoft_ipmi", new ObjectGetOptions());

            try {
                if (cls.GetInstances().Count == 0)
                    return "BMC not found";
            }
            catch {
                return "BMC not found";
            }

            ManagementObject clsInstance = null;
            foreach (ManagementObject candidate in cls.GetInstances()) {
                clsInstance = candidate;
                break;
            }

            ManagementBaseObject inParameters = cls.GetMethodParameters("RequestResponse");
            inParameters.SetPropertyValue("Command", getChannelInfoCmd);
            inParameters.SetPropertyValue("Lun", defaultLUN);
            inParameters.SetPropertyValue("NetworkFunction", 0x06);
            inParameters.SetPropertyValue("ResponderAddress", bmcResponderAddress);

            //Get the first lan channel.
            byte lanChannel = 0;
            bool lanFound = false;
            for (; lanChannel <= maxChannel; lanChannel++) {
                requestData = new byte[] { lanChannel };

                inParameters.SetPropertyValue("RequestData", requestData);
                inParameters.SetPropertyValue("RequestDataSize", requestData.Length);

                ManagementBaseObject mo = clsInstance.InvokeMethod("RequestResponse", inParameters, new InvokeMethodOptions());
                if (((byte[])mo.GetPropertyValue("ResponseData"))[2] == __8023LANMediumType) {
                    lanFound = true;
                    break;
                }
            }
            if (!lanFound)
                return "BMC not connected to a LAN";

            //Get the IP
            inParameters.SetPropertyValue("Command", getLANInfoCmd);
            inParameters.SetPropertyValue("NetworkFunction", 0x0c);

            requestData = new byte[] { lanChannel, 3, 0, 0 };

            inParameters.SetPropertyValue("RequestData", requestData);
            inParameters.SetPropertyValue("RequestDataSize", requestData.Length);

            ManagementBaseObject o = clsInstance.InvokeMethod("RequestResponse", inParameters, new InvokeMethodOptions());
            var responseData = (byte[])o.GetPropertyValue("ResponseData");

            return responseData[2] + "." + responseData[3] + "." + responseData[4] + "." + responseData[5];
        }

        /// <summary>
        /// Caches the ManagementScopes.
        /// </summary>
        /// <param name="query">The WMI query.</param>
        /// <param name="scopeNameSpace">The scope name space.</param>
        /// <returns></returns>
        public static ManagementObjectCollection GetWmiInfo(string query, string scopeNameSpace = "root\\cimv2") {
            return new ManagementObjectSearcher(ConnectScope(scopeNameSpace), new ObjectQuery(query)).Get();
        }

        private static ManagementScope ConnectScope(string nameSpace = "root\\cimv2") {
            ManagementScope scope;
            if (_connectedScopes.TryGetValue(nameSpace, out scope)) {
                if (!scope.IsConnected) scope.Connect();
                return scope;
            }

            var options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.EnablePrivileges = false;
            options.Username = null;
            options.Password = null;
            var mpath = new ManagementPath(String.Format("\\\\{0}\\{1}", Environment.MachineName, nameSpace));
            scope = new ManagementScope(mpath, options);

            scope.Connect();

            _connectedScopes.GetOrAdd(nameSpace, scope);

            return scope;
        }
    }
}