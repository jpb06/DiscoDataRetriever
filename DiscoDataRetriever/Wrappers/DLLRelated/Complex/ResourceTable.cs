using DiscoDataRetriever.DataRetrieval;
using DiscoDataRetriever.DataRetrieval.Ressources;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex.Final;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex
{
    public class ResourceTable
    {
        public ResType Type { get; set; }
        public ResourceDirectory Header { get; set; }
        public List<ResourceDirectoryEntry> Lvl1_Entries { get; set; }
        public List<ResourceDirectoryEntry> Lvl2_Language { get; set; }
        public List<ResourceDataEntry> Lvl3_Data { get; set; }
        public List<Resourcestrings> Lvl4_strings { get; set; }

        #region ctor
        public ResourceTable(int type)
        {
            if (type == 6)
                this.Type = ResType.String;
            else if (type == 23)
                this.Type = ResType.XML;
            this.Header = null;
            this.Lvl1_Entries = new List<ResourceDirectoryEntry>();
            this.Lvl2_Language = new List<ResourceDirectoryEntry>();
            this.Lvl3_Data = new List<ResourceDataEntry>();
            this.Lvl4_strings = new List<Resourcestrings>();
        }
        #endregion

        #region complex_getters
        public List<ConsResProperties> GetResources(string source)
        {
            List<ConsResProperties> resList = new List<ConsResProperties>();

            for (int i = 0; i < Lvl4_strings.Count; i++) 
            {
                for(int j = 0; j < Lvl4_strings.ElementAt(i).Strings.Count; j++)
                {
                    ConsResProperties crp = new ConsResProperties();
                    crp.Id = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Id;
                    crp.LanguageID = (int)Lvl2_Language.ElementAt(i).Name;
                    crp.CodePage = (int)Lvl3_Data.ElementAt(i).CodePage;
                    crp.Source = source;

                    if(Type == ResType.XML)
                        crp.Data = XMLParser.Parse(Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Data);
                    else if(Type == ResType.String)
                        crp.Data = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Data;
                    
                    crp.Type = Type;

                    resList.Add(crp);
                }
            }

            return resList;
        }

        public List<ConsResProperties> GetResourcesByType(ResType type)
        {
            List<ConsResProperties> resList = new List<ConsResProperties>();

            if (Type == type)
            {
                for (int i = 0; i < Lvl4_strings.Count; i++)
                {
                    for (int j = 0; j < Lvl4_strings.ElementAt(i).Strings.Count; j++)
                    {
                        ConsResProperties crp = new ConsResProperties();
                        crp.Id = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Id;
                        crp.LanguageID = (int)Lvl2_Language.ElementAt(i).Name;
                        crp.CodePage = (int)Lvl3_Data.ElementAt(i).CodePage;
                        crp.Data = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Data;
                        crp.Type = Type;

                        resList.Add(crp);
                    }
                }
            }

            return resList;
        } 

        public ConsResProperties GetResourceByID(int id)
        {
            for (int i = 0; i < Lvl4_strings.Count; i++)
            {
                for (int j = 0; j < Lvl4_strings.ElementAt(i).Strings.Count; j++)
                {
                    if (Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Id == id) 
                    {
                        ConsResProperties crp = new ConsResProperties();
                        crp.Id = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Id;
                        crp.LanguageID = (int)Lvl2_Language.ElementAt(i).Name;
                        crp.CodePage = (int)Lvl3_Data.ElementAt(i).CodePage;
                        crp.Data = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Data;
                        crp.Type = Type;
                        return crp;
                    }
                }
            }
            throw new Exception("Couldn't find the resource "+id);
        }

        public List<ConsResProperties> GetResourcesByData(string data)
        {
            List<ConsResProperties> resList = new List<ConsResProperties>();

            for (int i = 0; i < Lvl4_strings.Count; i++)
            {
                for (int j = 0; j < Lvl4_strings.ElementAt(i).Strings.Count; j++)
                {
                    if (Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Data.Contains(data))
                    {
                        ConsResProperties crp = new ConsResProperties();
                        crp.Id = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Id;
                        crp.LanguageID = (int)Lvl2_Language.ElementAt(i).Name;
                        crp.CodePage = (int)Lvl3_Data.ElementAt(i).CodePage;
                        crp.Data = Lvl4_strings.ElementAt(i).Strings.ElementAt(j).Data;
                        crp.Type = Type;
                        resList.Add(crp);
                    }
                }
            }

            if(resList.Count == 0)
                throw new Exception("No resources contain " + data);

            return resList;
        }
        #endregion

        #region Add
        public void AddHeader(BinaryReader br) 
        {
            this.Header = new ResourceDirectory(br);
        }

        public void AddEntry(BinaryReader br)
        {
            this.Lvl1_Entries.Add(new ResourceDirectoryEntry(br));
        }

        public void AddLanguageEntry(BinaryReader br) 
        {
            ResourceDirectory rd = new ResourceDirectory(br);
            for (int i = 0; i < rd.NumberOfIdEntries; i++)
            {
                this.Lvl2_Language.Add(new ResourceDirectoryEntry(br));
            }
        }

        public void AddDataEntry(BinaryReader br)
        {
            this.Lvl3_Data.Add(new ResourceDataEntry(br));
        }

        public void Addstrings(BinaryReader br, int offset, int length, int id)
        {
            this.Lvl4_strings.Add(new Resourcestrings(br, offset, length, Type, id));
        }
        #endregion
    }
}
