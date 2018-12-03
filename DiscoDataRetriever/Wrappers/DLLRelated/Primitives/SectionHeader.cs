using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives.Base;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated
{
    public class SectionHeader
    {
        public string Name { get; set; }
        public int VirtualAddress { get; set; }
        public int SizeOfRawData { get; set; }
        public int PointerToRawData { get; set; }
        public int PointerToRelocations { get; set; }
        public int PointerToLinenumbers { get; set; }
        public int NumberOfRelocations { get; set; }
        public int NumberOfLinenumbers { get; set; }
        public int Characteristics { get; set; }

        public SectionHeader()
        {
            this.Name = null;
            this.VirtualAddress = -1;
            this.SizeOfRawData = -1;
            this.PointerToRawData = -1;
            this.PointerToRelocations = -1;
            this.PointerToLinenumbers = -1;
            this.NumberOfRelocations = -1;
            this.NumberOfLinenumbers = -1;
            this.Characteristics = -1;
        }
    }
}
