using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoTradingDataRetrieval.Wrappers.Internal
{
    public class KeyDoubleValue
    {
        public string Key { get; set; }
        public string ValueA { get; set; }
        public string ValueB { get; set; }

        public KeyDoubleValue()
        {
            this.Key = null;
            this.ValueA = null;
            this.ValueB = null;
        }

        public KeyDoubleValue(string key, string val1, string val2)
        {
            this.Key = key;
            this.ValueA = val1;
            this.ValueB = val2;
        }
    }
}
