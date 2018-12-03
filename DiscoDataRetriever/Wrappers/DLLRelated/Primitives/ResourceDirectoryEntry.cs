using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives.Base;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated
{
    public class ResourceDirectoryEntry
    {
        public uint Name { get; set; }
        public uint OffsetToData { get; set; }

        public ResourceDirectoryEntry()
        {
            this.Name = 0;
            this.OffsetToData = 0;
        }

        public ResourceDirectoryEntry(uint name, uint offset)
        {
            this.Name = name;
            this.OffsetToData = offset;
        }

        public ResourceDirectoryEntry(BinaryReader br) 
        {
            this.Name = br.ReadUInt32();   
            this.OffsetToData = br.ReadUInt32();
        }
    }
}
