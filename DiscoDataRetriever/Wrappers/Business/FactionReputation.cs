using System;

namespace DiscoTradingDataRetrieval.Wrappers.Business
{
    public class FactionReputation
    {
        public string NickName { get; set; }
        public double Value { get; set; }

        #region ctor
        public FactionReputation()
        {
            this.NickName = null;
            this.Value = -1;
        }

        public FactionReputation(string nickname, double rep)
        {
            this.NickName = nickname;
            this.Value = rep;
        } 
        #endregion
    }
}
