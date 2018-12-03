using System;
using System.Linq;

namespace DiscoDataRetriever.Wrappers.Internal
{
    public class CommodityBlock
    {
        public string CommodityNn { get; set; }
        public bool BuyOnly { get; set; }
        public Double PriceModifier { get; set; }

        public CommodityBlock()
        {
            this.CommodityNn = null;
            this.BuyOnly = false;
            this.PriceModifier = -1;
        }

        public CommodityBlock(string commNn, bool buyOnly, int price)
        {
            this.CommodityNn = commNn;
            this.BuyOnly = buyOnly;
            this.PriceModifier = price;
        }

        public CommodityBlock(string line)
        {
            line = line.Substring(line.IndexOf("=") + 1).Replace(" ", string.Empty).ToLower();

            string[] el = line.Split(',');

            this.CommodityNn = el.ElementAt(0);
            string price = this.FormatModifier(el.ElementAt(5).Replace('.', ','));
            this.PriceModifier = Double.Parse(price);

            if (el.ElementAt(3).Equals("0") &&
               el.ElementAt(4).Equals("0") &&
               el.ElementAt(5).Equals("1"))
                this.BuyOnly = true;
            else if (el.ElementAt(3).Equals("150") &&
               el.ElementAt(4).Equals("500") &&
               el.ElementAt(5).Equals("0"))
                this.BuyOnly = false;
        }

        private string FormatModifier(string modifier)
        {
            if (modifier.Contains(','))
            {
                int commaCount = modifier.Count(c => c == ',');

                if (commaCount > 1)
                {
                    string first = modifier.Substring(0, modifier.IndexOf(','));
                    string second = modifier.Substring(modifier.IndexOf(',') + 1);

                    second = second.Substring(0, second.IndexOf(','));

                    return first + "," + second;
                }
                else
                {
                    char[] allowed = { '1', '2', '3', '4', '5', '6', '7', '8', '9', ',' };

                    string ash = modifier.Substring(0, modifier.LastIndexOfAny(allowed) + 1);
                    return ash;
                }
            }
            else
                return modifier;
        }
    }
}
