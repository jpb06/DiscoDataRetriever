using System;

namespace DiscoDataRetriever.Wrappers.Business
{
    public class MarketOpportunity
    {
        public bool BuyOnly { get; set; }
        public int Price { get; set; }
        public string CommodityNickname { get; set; }

        #region ctor
        public MarketOpportunity() 
        {
            this.BuyOnly = false;
            this.Price = -1;
            this.CommodityNickname = null;
        }

        public MarketOpportunity(bool buyOnly, Double price, string cNn)
        {
            this.BuyOnly = buyOnly;
            this.CommodityNickname = cNn;

            this.Price = (int)Math.Floor( price );
        }
        #endregion
    }
}