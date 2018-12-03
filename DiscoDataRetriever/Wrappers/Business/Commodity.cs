using System;

namespace DiscoDataRetriever.Wrappers.Business
{
    public class Commodity
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public string Infos { get; set; }
        public string FileName { get; set; }
        public int BasePrice { get; set; }

        #region ctor
        public Commodity(string nickName, string name, string infos, string filename, int basePrice) 
        {
            this.NickName = nickName;
            this.Name = name;
            this.Infos = infos;
            this.FileName = filename;
            this.BasePrice = basePrice;
        }
        #endregion
    }
}