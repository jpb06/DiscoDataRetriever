using System;
using System.Collections.Generic;

namespace DiscoDataRetriever.Wrappers.Business
{
    public class Base
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public string SystemNickName { get; set; }
        public string FactionNN { get; set; }
        public List<MarketOpportunity> Market { get; set; }
        public string Description { get; set; }

        #region ctor
        public Base(string nickName, string name, string systemNickName, string factionNN, string description) 
        {
            this.NickName = nickName;
            this.Name = name;
            this.SystemNickName = systemNickName;
            this.FactionNN = factionNN;
            this.Description = description;
        }
        #endregion
    }
}
