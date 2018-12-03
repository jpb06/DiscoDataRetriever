using System;
using System.Collections.Generic;

namespace DiscoTradingDataRetrieval.Wrappers.Business
{
    public class Faction
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public List<FactionReputation> RepList { get; set; }

        #region ctors
        public Faction()
        {
            this.NickName = null;
            this.Name = null;
            this.ShortName = null;
            this.Description = null;
            this.RepList = null;
        }

        public Faction(string nickname, string name, string sName, string desc, List<FactionReputation> rList)
        {
            this.NickName = nickname;
            this.Name = name;
            this.ShortName = sName;
            this.Description = desc;
            this.RepList = rList;
        }
        #endregion
    }
}
