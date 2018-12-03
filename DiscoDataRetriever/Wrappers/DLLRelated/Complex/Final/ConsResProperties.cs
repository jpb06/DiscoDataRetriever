using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex.Final
{
    public enum ResType{ String, XML };

    public class ConsResProperties
    {
        public int Id { get; set; }
        public int LanguageID { get; set; }
        public int CodePage { get; set; }
        public string Source { get; set; }
        public string Data { get; set; }
        public ResType Type { get; set; }

        public ConsResProperties()
        {
            this.Id = -1;
            this.LanguageID = -1;
            this.CodePage = -1;
            this.Data = null;
            this.Type = ResType.String;
            this.Source = null;
        }

        public ConsResProperties(int id, int languageID, int codePage, string source, string data, ResType type)
        {
            this.Id = id;
            this.LanguageID = languageID;
            this.CodePage = codePage;
            this.Source = source;
            this.Data = data;
            this.Type = type;
        }
    }
}
