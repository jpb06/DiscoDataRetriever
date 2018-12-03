using System;

namespace DiscoTradingDataRetrieval.Wrappers.Internal
{
    public class InfoCardMap
    {
        public int First { get; set; }
        public int Second { get; set; }

        public InfoCardMap(string first, string second)
        {
            int f, s; 
            Int32.TryParse(first, out f);
            Int32.TryParse(second, out s);

            this.First = f;
            this.Second = s;
        }
    }
}
