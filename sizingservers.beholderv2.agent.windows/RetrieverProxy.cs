using System;
using System.Collections.Concurrent;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    /// <summary>
    /// Caches the ManagementScopes.
    /// </summary>
    internal static class RetrieverProxy {
        private static ConcurrentDictionary<string, ManagementScope> _connectedScopes = new ConcurrentDictionary<string, ManagementScope>();

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
