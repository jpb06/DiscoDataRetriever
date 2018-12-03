using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives.Base;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated
{
    public class ResourceDirectory 
    {
        public uint Characteristics { get; set; }
        public uint TimeDateStamp { get; set; }
        public uint MajorVersion { get; set; }
        public uint MinorVersion { get; set; }
        public uint NumberOfNamedEntries { get; set; }
        public uint NumberOfIdEntries { get; set; }

        public ResourceDirectory()
        {
            this.Characteristics = 0;
            this.TimeDateStamp = 0;
            this.MajorVersion = 0;
            this.MinorVersion = 0;
            this.NumberOfNamedEntries = 0;
            this.NumberOfIdEntries = 0;
        }

        public ResourceDirectory(BinaryReader br) 
        {
            this.Characteristics = br.ReadUInt32();
            this.TimeDateStamp = br.ReadUInt32();
            this.MajorVersion = br.ReadUInt16();
            this.MinorVersion = br.ReadUInt16();
            this.NumberOfNamedEntries = br.ReadUInt16();
            this.NumberOfIdEntries = br.ReadUInt16();
        }
    }
}
