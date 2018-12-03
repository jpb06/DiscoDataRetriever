using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoTradingDataRetrieval.Wrappers.Internal
{
    public class MapSystemPos
    {
        public string SystemNickName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public MapSystemPos()
        {
            this.SystemNickName = null;
            this.X = -1;
            this.Y = -1;
        }

        public MapSystemPos(string nickName, int x, int y)
        {
            this.SystemNickName = nickName;
            this.X = x;
            this.Y = y;
        }
    }
}
