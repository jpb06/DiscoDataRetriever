using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives.Base
{
    public class BasePEStructure
    {
        public int InternalOffset { get; set; }

        public BasePEStructure()
        {
            this.InternalOffset = -1;
        }

        public BasePEStructure(BinaryReader br)
        {
            this.InternalOffset = (int)br.BaseStream.Position;
        }
    }
}
