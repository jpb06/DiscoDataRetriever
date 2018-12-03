using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoTradingDataRetrieval.Wrappers.Internal
{
    public class SystemPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public SystemPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
