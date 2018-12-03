using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives.Base;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated
{
    public class ResourceDataEntry 
    {
        public uint OffsetToData { get; set; }
        public uint Size { get; set; }
        public uint CodePage { get; set; }
        public uint Reserved { get; set; }

        public ResourceDataEntry() 
        {
            this.OffsetToData = 0;
            this.Size = 0;
            this.CodePage = 0;
            this.Reserved = 0;
        }

        public ResourceDataEntry(BinaryReader br) 
        {
            this.OffsetToData = br.ReadUInt32();
            this.Size = br.ReadUInt32();
            this.CodePage = br.ReadUInt32();
            this.Reserved = br.ReadUInt32();
        }
    }
}
