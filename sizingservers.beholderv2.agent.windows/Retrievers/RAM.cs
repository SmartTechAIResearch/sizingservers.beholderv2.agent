using sizingservers.beholderv2.agent.shared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    internal class RAM : IPayloadRetriever {
        public static RAM _instance = new RAM();
        private static ConcurrentDictionary<long, string> _memoryTypes = new ConcurrentDictionary<long, string>();

        public static RAM GetInstance() { return _instance; }

        private RAM() { }
        static RAM() {
            //MemoryType --> number that matches a type specified in SMBIOS. Depending on the version of SMBIOS the type can be found or not (0 ==  unknown)
            //https://www.dmtf.org/standards/smbios
            //Latest specs at this moment https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.1.1.pdf

            _memoryTypes.TryAdd(0x00, "Unknown");
            _memoryTypes.TryAdd(0x01, "Other");
            _memoryTypes.TryAdd(0x02, "Unknown");
            _memoryTypes.TryAdd(0x03, "DRAM");
            _memoryTypes.TryAdd(0x04, "EDRAM");
            _memoryTypes.TryAdd(0x05, "VRAM");
            _memoryTypes.TryAdd(0x06, "SRAM");
            _memoryTypes.TryAdd(0x07, "RAM");
            _memoryTypes.TryAdd(0x08, "ROM");
            _memoryTypes.TryAdd(0x09, "FLASH");
            _memoryTypes.TryAdd(0x0A, "EEPROM");
            _memoryTypes.TryAdd(0x0B, "FEPROM");
            _memoryTypes.TryAdd(0x0C, "EPROM");
            _memoryTypes.TryAdd(0x0D, "CDRAM");
            _memoryTypes.TryAdd(0x0E, "3DRAM");
            _memoryTypes.TryAdd(0x0F, "SDRAM");
            _memoryTypes.TryAdd(0x10, "SGRAM");
            _memoryTypes.TryAdd(0x11, "RDRAM");
            _memoryTypes.TryAdd(0x12, "DDR");
            _memoryTypes.TryAdd(0x13, "DDR2");
            _memoryTypes.TryAdd(0x14, "DDR2 FB-DIMM");
            _memoryTypes.TryAdd(0x18, "DDR3");
            _memoryTypes.TryAdd(0x19, "FBD2");
            _memoryTypes.TryAdd(0x1A, "DDR4");
            _memoryTypes.TryAdd(0x1B, "LPDDR");
            _memoryTypes.TryAdd(0x1C, "LPDDR2");
            _memoryTypes.TryAdd(0x1D, "LPDDR3");
            _memoryTypes.TryAdd(0x1E, "LPDDR4");
        }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverProxy.GetWmiInfo("Select * from Win32_PhysicalMemory");
            foreach (ManagementObject mo in col) {
                string memoryType;
                if (!_memoryTypes.TryGetValue(long.Parse(mo["MemoryType"].ToString()), out memoryType))
                    memoryType = _memoryTypes[0x00];

                cgs.Add(new ComponentGroup("RAM",
                    new PayloadProperty[] {
                        new PayloadProperty("Size", long.Parse(mo["Capacity"].ToString()) / (1024*1024*1024), false, "GB"),
                        new PayloadProperty("Manufacturer", mo["Manufacturer"]),
                        new PayloadProperty("PartNumber", mo["PartNumber"]),
                        new PayloadProperty("SerialNumber", mo["SerialNumber"], true),
                        new PayloadProperty("Type", memoryType),
                        new PayloadProperty("Speed", mo["Speed"], false, "Mhz"),
                    })
                );
            }

            return cgs;
        }
    }
}
