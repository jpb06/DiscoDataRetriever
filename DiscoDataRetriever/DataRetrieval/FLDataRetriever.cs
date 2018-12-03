using DiscoDataRetriever.DataRetrieval.Images;
using DiscoDataRetriever.DataRetrieval.Ressources;
using DiscoDataRetriever.Wrappers.Business;
using DiscoDataRetriever.Wrappers.Internal;
using DiscoTradingDataRetrieval.Wrappers.Business;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex.Final;
using DiscoTradingDataRetrieval.Wrappers.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscoDataRetriever.DataRetrieval
{
    public class FLDataRetriever
    {
        public string Path { get; set; }
        public List<Base> Bases { get; set; }
        public List<Commodity> Commodities { get; set; }
        public List<StarSystem> Systems { get; set; }
        public List<Faction> Factions { get; set; }
        public List<ConsResProperties> Dllstrings { get; set; }
        public List<ConsResProperties> DllXml { get; set; }
        public List<InfoCardMap> InfosMap { get; set; }
        public List<SystemConnections> Connections { get; set; }

        #region ctors
        public FLDataRetriever()
        {
            this.Path = null;
            this.Bases = null;
            this.Factions = null;
            this.Commodities = null;
            this.Systems = null;
            
            this.InfosMap = null;
            this.Connections = null;
        }

        public FLDataRetriever(string path)
        {
            this.Dllstrings = new List<ConsResProperties>();
            this.DllXml = new List<ConsResProperties>();

            this.Path = path;
            this.Bases = new List<Base>();
            this.Factions = new List<Faction>();
            this.Commodities = new List<Commodity>();
            this.Systems = new List<StarSystem>();
            this.Connections = new List<SystemConnections>();

            this.ParseRessourceFile(DllResource.nameresources);
            this.ParseRessourceFile(DllResource.infocards);
            this.ParseRessourceFile(DllResource.misctext);
            this.ParseRessourceFile(DllResource.Discovery);
            this.ParseRessourceFile(DllResource.DsyAddition);
        }

        private void ParseRessourceFile(DllResource ressource)
        {
            using (DLLParser parser = new DLLParser(Configuration.GetDllPath(ressource), ressource.ToString()))
            {
                parser.GetResources();
                this.Dllstrings.AddRange(parser.Basestrings);
                this.DllXml.AddRange(parser.Xmlstrings);
            }
        }
        #endregion

        public void FetchData()
        {
            this.GetInfoCardsMap();
            this.ParseFactions();
            this.ParseUniverse();
            this.OrganizeSystemsPositions();
            this.GetCommodities();
            this.GetMarket();
        }

        #region UniverseParsing
        private void ParseUniverse()
        {
            string subPath = @"DATA\UNIVERSE\universe.ini";
            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<KeyValue> dataBlock = new List<KeyValue>();
                    string line = reader.ReadLine();

                    if (!line.Equals("[Time]"))
                        throw new Exception("Wrong format for universe.ini");

                    while (!line.Equals("[Base]") && !line.Equals("[system]"))
                        line = reader.ReadLine();

                    dataBlock.Add(new KeyValue("InternalKeyType", line));

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            if (!line.Equals("[Base]") && !line.Equals("[system]"))
                            {
                                dataBlock.Add(new KeyValue(line, '='));
                            }
                            else if (line.Equals("[Base]") || line.Equals("[system]"))
                            {
                                this.CreateElement(dataBlock);
                                dataBlock.Clear();
                                dataBlock.Add(new KeyValue("InternalKeyType", line));
                            }
                        }
                    }

                    this.CreateElement(dataBlock);
                }
            }
            else
                throw new Exception("universe.ini doesn't exist.");
        }

        private void CreateElement(List<KeyValue> dataBlock)
        {
            if (dataBlock.ElementAt(0).Value.Equals("[Base]"))
            {
                int id = Convert.ToInt32((from db in dataBlock
                                          where db.Key.Equals("strid_name")
                                          select db.Value).FirstOrDefault());

                if (id != 0)
                {
                    string nickName = (from db in dataBlock
                                       where db.Key.Equals("nickname")
                                       select db.Value).FirstOrDefault().ToLower();

                    string SystemNN = (from db in dataBlock
                                       where db.Key.Equals("system")
                                       select db.Value).FirstOrDefault();

                    string hex = id.ToString("X");
                    int resID = Convert.ToInt32(hex.Substring(1), 16);

                    string name = (from rsrc in this.Dllstrings
                                   where rsrc.Id == resID &&
                                   rsrc.Type == ResType.String &&
                                   !rsrc.Data.Contains("\n") && !string.IsNullOrWhiteSpace(rsrc.Data) && !string.IsNullOrEmpty(rsrc.Data) && !rsrc.Data.StartsWith("I'll sell you the location of a")
                                   select rsrc.Data).FirstOrDefault();

                    List<string> details = this.GetBaseDetails(nickName, SystemNN);
                    if (details != null && name != null && details.Count > 1)
                    {
                        string infos = "";
                        if (details.ElementAt(1).Length > 1)
                        {
                            hex = Int32.Parse(details.ElementAt(1)).ToString("X");
                            resID = Convert.ToInt32(hex.Substring(1), 16);

                            ConsResProperties chunk = (from rsrc in this.DllXml
                                                       where rsrc.Id == resID &&
                                                       rsrc.Type == ResType.XML &&
                                                       (rsrc.Source == "infocards" || rsrc.Source == "Discovery")
                                                       select rsrc).DefaultIfEmpty(null).First();

                            if (chunk == null)
                                throw new Exception("First part of the Base " + nickName + " : " + name + " infocard couldn't be found.");

                            infos = chunk.Data + "|@|@";

                            int secondHash = (from infosMap in this.InfosMap
                                              where infosMap.First == Int32.Parse(details.ElementAt(1))
                                              select infosMap.Second).FirstOrDefault();
                            if (secondHash != 0)
                            {
                                hex = secondHash.ToString("X");
                                resID = Convert.ToInt32(hex.Substring(1), 16);
                                ConsResProperties secondChunk = (from rsrc in this.DllXml
                                                                 where rsrc.Id == resID &&
                                                                 rsrc.Type == ResType.XML &&
                                                                 (rsrc.Source == "infocards" || rsrc.Source == "Discovery" || rsrc.Source == "DsyAddition")
                                                                 select rsrc).DefaultIfEmpty(null).First();

                                if (secondChunk == null)
                                {
                                    hex = Int32.Parse(details.ElementAt(1)).ToString("X");
                                    resID = Convert.ToInt32(hex.Substring(1), 16);
                                    string secondPart = (from rsrc in this.DllXml
                                                         where rsrc.Id == (resID + 1) &&
                                                         rsrc.Type == ResType.XML &&
                                                         rsrc.Source == chunk.Source
                                                         select rsrc.Data).DefaultIfEmpty(String.Empty).First();

                                    //if (secondPart == null)
                                    //  throw new Exception("Second part of the Base " + nickName + " : " + name + " infocard couldn't be found.");

                                    infos += secondPart;
                                }
                                else
                                    infos += secondChunk.Data;
                            }
                        }

                        this.Bases.Add(new Base(nickName, name, SystemNN.ToLower(), details.ElementAt(0), infos));
                    }

                }
            }
            else if (dataBlock.ElementAt(0).Value.Equals("[system]"))
            {
                string nickName = (from db in dataBlock
                                   where db.Key.Equals("nickname")
                                   select db.Value).FirstOrDefault().ToLower();

                int id = Convert.ToInt32((from db in dataBlock
                                          where db.Key.Equals("strid_name")
                                          select db.Value).FirstOrDefault());

                string pos = (from db in dataBlock
                              where db.Key.Equals("pos")
                              select db.Value).FirstOrDefault();
                float x = float.Parse(pos.Substring(0, pos.IndexOf(",")).Trim(), CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(pos.Substring(pos.IndexOf(",") + 1).Trim(), CultureInfo.InvariantCulture.NumberFormat);

                string hex = id.ToString("X");
                int resID = Convert.ToInt32(hex.Substring(1), 16);

                string name = (from rsrc in this.Dllstrings
                               where rsrc.Id == resID &&
                               rsrc.Type == ResType.String &&
                               !rsrc.Data.Contains("\n")
                               select rsrc.Data).FirstOrDefault();

                this.Systems.Add(new StarSystem(nickName, name, x, y));
                this.GetSystemConnections(nickName);
            }
        }

        private List<string> GetBaseDetails(string baseNN, string system)
        {

            string subPath = @"DATA\UNIVERSE\SYSTEMS\" + system + @"\" + system + ".ini";
            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<KeyValue> dataBlock = new List<KeyValue>();
                    string line = reader.ReadLine();

                    while (!line.Equals("[Object]"))
                        line = reader.ReadLine();

                    dataBlock.Add(new KeyValue("InternalKeyType", line));

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            if (!line.StartsWith("[") && !line.EndsWith("]"))
                            {
                                dataBlock.Add(new KeyValue(line, '='));
                            }
                            else if (line.StartsWith("[") && line.EndsWith("]"))
                            {
                                if (dataBlock.Where(el => el.Key == "InternalKeyType").FirstOrDefault().Value == "[Object]")
                                {
                                    KeyValue idsName = dataBlock.Where(el => el.Key == "ids_name").FirstOrDefault();

                                    KeyValue baseN = dataBlock.Where(el => el.Key == "base").FirstOrDefault();
                                    if (baseN != null)
                                    {
                                        if (baseN.Value.ToLower() == baseNN)
                                        {
                                            KeyValue infos = dataBlock.Where(el => el.Key == "ids_info").FirstOrDefault();
                                            if (infos != null)
                                            {
                                                List<string> details = new List<string>();
                                                KeyValue reputation = dataBlock.Where(el => el.Key == "reputation").FirstOrDefault();
                                                if (reputation != null)
                                                {
                                                    details.Add(reputation.Value.ToLower());
                                                }

                                                details.Add(infos.Value);
                                                return details;
                                            }
                                        }
                                    }
                                }

                                dataBlock.Clear();
                                dataBlock.Add(new KeyValue("InternalKeyType", line));
                            }
                        }
                    }

                    if (dataBlock.Where(el => el.Key == "InternalKeyType").FirstOrDefault().Value == "[Object]")
                    {
                        KeyValue idsName = dataBlock.Where(el => el.Key == "ids_name").FirstOrDefault();
                        KeyValue baseN = dataBlock.Where(el => el.Key == "base").FirstOrDefault();

                        if (baseN != null)
                        {
                            if (baseN.Value.ToLower() == baseNN)
                            {
                                KeyValue infos = dataBlock.Where(el => el.Key == "ids_info").FirstOrDefault();
                                if (infos != null)
                                {
                                    List<string> details = new List<string>();
                                    details.Add(dataBlock.Where(el => el.Key == "reputation").FirstOrDefault().Value.ToLower());
                                    details.Add(infos.Value);
                                    return details;
                                }
                            }
                        }
                    }
                }
            }

            return null;

        }

        private void GetSystemConnections(string system)
        {
            string subPath = @"DATA\UNIVERSE\SYSTEMS\" + system + @"\" + system + ".ini";
            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<KeyValue> dataBlock = new List<KeyValue>();
                    string line = reader.ReadLine();

                    while (!line.Equals("[Object]"))
                        line = reader.ReadLine();

                    dataBlock.Add(new KeyValue("InternalKeyType", line));

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            if (!line.StartsWith("[") && !line.EndsWith("]"))
                            {
                                dataBlock.Add(new KeyValue(line, '='));
                            }
                            else if (line.StartsWith("[") && line.EndsWith("]"))
                            {
                                if (dataBlock.Where(el => el.Key == "InternalKeyType").FirstOrDefault().Value == "[Object]")
                                {
                                    KeyValue goTo = dataBlock.Where(el => el.Key == "goto").FirstOrDefault();
                                    if (goTo != null)
                                    {
                                        ConnectionType type = ConnectionType.Null;
                                        KeyValue archetype = dataBlock.Where(el => el.Key == "archetype").FirstOrDefault();
                                        if (archetype.Value.StartsWith("jumpgate"))
                                            type = ConnectionType.Jumpgate;
                                        else if (archetype.Value.StartsWith("jumphole"))
                                            type = ConnectionType.Jumphole;
                                        else if (archetype.Value.Contains("nomad"))
                                            type = ConnectionType.Nomadgate;
                                        else if (archetype.Value.Contains("dyson"))
                                            type = ConnectionType.DysonAirlock;

                                        string dest = goTo.Value.Substring(0, goTo.Value.IndexOf(","));

                                        this.Connections.Add(new SystemConnections(type, system, dest.ToLower()));
                                    }
                                }

                                dataBlock.Clear();
                                dataBlock.Add(new KeyValue("InternalKeyType", line));
                            }
                        }
                    }

                    if (dataBlock.Where(el => el.Key == "InternalKeyType").FirstOrDefault().Value == "[Object]")
                    {
                        KeyValue goTo = dataBlock.Where(el => el.Key == "goto").FirstOrDefault();
                        if (goTo != null)
                        {
                            ConnectionType type = ConnectionType.Null;
                            KeyValue archetype = dataBlock.Where(el => el.Key == "archetype").FirstOrDefault();
                            if (archetype.Value.StartsWith("jumpgate"))
                                type = ConnectionType.Jumpgate;
                            else if (archetype.Value.StartsWith("jumphole"))
                                type = ConnectionType.Jumphole;
                            else if (archetype.Value.Contains("nomad"))
                                type = ConnectionType.Nomadgate;
                            else if (archetype.Value.Contains("dyson"))
                                type = ConnectionType.DysonAirlock;

                            string dest = goTo.Value.Substring(0, goTo.Value.IndexOf(","));

                            this.Connections.Add(new SystemConnections(type, system, dest.ToLower()));
                        }
                    }
                }
            }
        }

        private void OrganizeSystemsPositions()
        {
            float maxY = this.Systems.Select(p => p.PosY).Max();
            float minX = this.Systems.Select(p => p.PosX).Min();

            foreach (StarSystem ss in this.Systems)
            {
                ss.PosX = ss.PosX + (-minX) + 1;
                ss.PosY = maxY - ss.PosY + 1;
            }
        }
        #endregion

        #region CommoditiesParsing
        private List<KeyDoubleValue> ParseEquip()
        {
            List<KeyDoubleValue> values = new List<KeyDoubleValue>();

            string subPath = @"DATA\EQUIPMENT\select_equip.ini";

            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<KeyValue> dataBlock = new List<KeyValue>();
                    string line = null;

                    string headerPattern = @"^\[[a-zA-Z]+\]$";

                    bool seek = false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            if (Regex.IsMatch(line, headerPattern))
                            {
                                if (line.Equals("[Commodity]"))
                                    seek = false;
                                else
                                    seek = true;

                                if (dataBlock.Count > 0)
                                {
                                    string nn = (from db in dataBlock
                                                 where db.Key.Equals("nickname")
                                                 select db.Value).FirstOrDefault().ToLower();

                                    string resNameID = (from db in dataBlock
                                                        where db.Key.Equals("ids_name")
                                                        select db.Value).FirstOrDefault();

                                    string resInfoID = (from db in dataBlock
                                                        where db.Key.Equals("ids_info")
                                                        select db.Value).FirstOrDefault();

                                    values.Add(new KeyDoubleValue(nn, resNameID, resInfoID));
                                    dataBlock.Clear();
                                }
                            }
                            else
                            {
                                if (!seek)
                                    dataBlock.Add(new KeyValue(line, '='));
                            }
                        }
                    }

                    return values;
                }
            }
            else
                throw new Exception("select_equip.ini doesn't exist.");
        }

        private void GetCommodities()
        {
            string subPath = @"DATA\EQUIPMENT\goods.ini";
            string outputPath = @"M:\WorkingSet\XenoCid\Anno_2k19\DiscoveryDataExtraction\Extracted\img\commodities\";


            List<KeyDoubleValue> commies = this.ParseEquip();

            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<KeyValue> dataBlock = new List<KeyValue>();
                    string line = reader.ReadLine();

                    if (!line.Equals("[Good]"))
                        throw new Exception("Wrong format for good.ini");

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            if (!line.Equals("[Good]"))
                            {
                                dataBlock.Add(new KeyValue(line, '='));
                            }
                            else if (line.Equals("[Good]"))
                            {
                                this.CreateCommodity(dataBlock, outputPath, commies);
                                dataBlock.Clear();
                            }
                        }
                    }

                    this.CreateCommodity(dataBlock, outputPath, commies);
                }
            }
            else
                throw new Exception("goods.ini doesn't exist.");
        }

        private void CreateCommodity(List<KeyValue> dataBlock, string outputPath, List<KeyDoubleValue> commodities)
        {
            string type = (from kvO in dataBlock
                           where kvO.Key.Equals("category")
                           select kvO.Value).FirstOrDefault();

            if (type.Equals("commodity"))
            {
                string nickName = (from db in dataBlock
                                   where db.Key.Equals("nickname")
                                   select db.Value).FirstOrDefault().ToLower();

                int nID = Convert.ToInt32((from cm in commodities
                                           where cm.Key.Equals(nickName)
                                           select cm.ValueA).FirstOrDefault());

                if (nID > 0)
                {
                    string nHex = nID.ToString("X");
                    int resNameID = Convert.ToInt32(nHex.Substring(1), 16);

                    string name = (from rsrc in this.Dllstrings
                                   where rsrc.Id == resNameID &&
                                   rsrc.Type == ResType.String
                                   select rsrc.Data).FirstOrDefault();

                    int iID = Convert.ToInt32((from cm in commodities
                                               where cm.Key.Equals(nickName)
                                               select cm.ValueB).FirstOrDefault());

                    string iHex = iID.ToString("X");
                    int resInfoID = Convert.ToInt32(iHex.Substring(1), 16);

                    string infos = (from rsrc in this.DllXml
                                    where rsrc.Id == resInfoID &&
                                    rsrc.Type == ResType.XML
                                    select rsrc.Data).FirstOrDefault();

                    if (!string.IsNullOrEmpty(infos))
                        infos = infos.Substring(infos.IndexOf("|@") + 2);

                    string utfPath = (from db in dataBlock
                                      where db.Key.Equals("item_icon")
                                      select db.Value).FirstOrDefault();

                    string basePrice = (from db in dataBlock
                                        where db.Key.Equals("price")
                                        select db.Value).FirstOrDefault();

                    string utfFilePath = this.Path + @"Data\" + utfPath;
                    string fileName = null;
                    if (File.Exists(utfFilePath))
                    {
                        using (UTFReader utfRdr = new UTFReader(utfFilePath))
                        {
                            fileName = utfRdr.GetImage(outputPath, nickName);
                        }
                    }
                    this.Commodities.Add(new Commodity(nickName, name, infos, fileName, Int32.Parse(basePrice)));
                }
            }
        }
        #endregion

        #region MarketParsing
        private void GetMarket()
        {
            string subPath = @"DATA\EQUIPMENT\market_commodities.ini";

            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<CommodityBlock> dataBlock = new List<CommodityBlock>();
                    string line = string.Empty;

                    do
                    {
                        line = reader.ReadLine();
                    }
                    while (line != null && !line.Equals("[BaseGood]"));

                    if (line == null)
                        throw new Exception("Wrong format for marketCommodities.ini");

                    KeyValue baseKey = new KeyValue(reader.ReadLine(), '=');
                    bool BaseRead = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            if (!line.Equals("[BaseGood]"))
                            {
                                if (!BaseRead)
                                {
                                    baseKey = new KeyValue(line, '=');
                                    BaseRead = true;
                                }
                                else
                                {
                                    dataBlock.Add(new CommodityBlock(line));
                                }
                            }
                            else if (line.Equals("[BaseGood]"))
                            {
                                this.CreateMarketOpportunity(dataBlock, baseKey);

                                BaseRead = false;
                                dataBlock.Clear();
                            }
                        }
                    }

                    this.CreateMarketOpportunity(dataBlock, baseKey);
                }
            }
            else
                throw new Exception("marketCommodities.ini doesn't exist.");
        }

        private void CreateMarketOpportunity(List<CommodityBlock> dataBlock, KeyValue baseKey)
        {
            Base currBase = (from ba in this.Bases
                             where ba.NickName.Equals(baseKey.Value.ToLower())
                             select ba).FirstOrDefault();

            if (currBase != null)
            {
                List<MarketOpportunity> opps = new List<MarketOpportunity>();

                foreach (CommodityBlock block in dataBlock)
                {
                    int comBasePrice = (from com in this.Commodities
                                        where com.NickName.Equals(block.CommodityNn)
                                        select com.BasePrice).FirstOrDefault();
                    opps.Add(new MarketOpportunity(block.BuyOnly, block.PriceModifier * comBasePrice, block.CommodityNn));
                }

                currBase.Market = opps;
            }
        }
        #endregion

        #region FactionsParsing
        private void ParseFactions()
        {
            string subPath = @"DATA\initialworld.ini";
            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<KeyValue> dataBlock = new List<KeyValue>();
                    string line = reader.ReadLine();

                    if (!line.Equals(";@#Group File"))
                        throw new Exception("Wrong format for initialworld.ini");

                    while (!line.Equals("[Group]"))
                        line = reader.ReadLine();

                    dataBlock.Add(new KeyValue("InternalKeyType", line));

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (!line.Equals("[Group]"))
                            {
                                dataBlock.Add(new KeyValue(line, '='));
                            }
                            else if (line.Equals("[Group]"))
                            {
                                this.CreateFaction(dataBlock);
                                dataBlock.Clear();
                                dataBlock.Add(new KeyValue("InternalKeyType", line));
                            }
                        }
                    }

                    this.CreateFaction(dataBlock);
                }
            }
            else
                throw new Exception("initialworld.ini doesn't exist.");
        }

        private void CreateFaction(List<KeyValue> dataBlock)
        {
            if (dataBlock.ElementAt(0).Value.Equals("[Group]"))
            {
                int idName = Convert.ToInt32((from db in dataBlock
                                              where db.Key.Equals("ids_name")
                                              select db.Value).FirstOrDefault());

                int idShortname = Convert.ToInt32((from db in dataBlock
                                                   where db.Key.Equals("ids_short_name")
                                                   select db.Value).FirstOrDefault());

                int idInfos = Convert.ToInt32((from db in dataBlock
                                               where db.Key.Equals("ids_info")
                                               select db.Value).FirstOrDefault());

                if (idName != 0 && idShortname != 0 && idInfos != 0)
                {
                    string nickName = (from db in dataBlock
                                       where db.Key.Equals("nickname")
                                       select db.Value).FirstOrDefault().ToLower();

                    string hex = idName.ToString("X");
                    int resID = Convert.ToInt32(hex.Substring(1), 16);

                    string name = (from rsrc in this.Dllstrings
                                   where rsrc.Id == resID &&
                                   rsrc.Type == ResType.String
                                   select rsrc.Data).FirstOrDefault();

                    hex = idShortname.ToString("X");
                    resID = Convert.ToInt32(hex.Substring(1), 16);

                    string shortName = (from rsrc in this.Dllstrings
                                        where rsrc.Id == resID &&
                                        rsrc.Type == ResType.String
                                        select rsrc.Data).FirstOrDefault();

                    string infos = "";
                    if (idInfos != 1)
                    {
                        hex = idInfos.ToString("X");
                        resID = Convert.ToInt32(hex.Substring(1), 16);

                        infos = (from rsrc in this.DllXml
                                 where rsrc.Id == resID &&
                                 rsrc.Type == ResType.XML
                                 select rsrc.Data).DefaultIfEmpty("").First();
                    }
                    this.Factions.Add(new Faction(nickName, name, shortName, infos, this.ParseReputations(dataBlock.Where(el => el.Key == "rep").ToList())));
                }
            }
        }

        private List<FactionReputation> ParseReputations(List<KeyValue> data)
        {
            List<FactionReputation> rep = new List<FactionReputation>();

            foreach (KeyValue unit in data)
            {
                string value = unit.Value.Substring(0, unit.Value.IndexOf(","));
                string faction = unit.Value.Substring(unit.Value.IndexOf(",") + 2);

                rep.Add(new FactionReputation(faction, Double.Parse(value.Replace(".", ","))));
            }

            return rep;
        }
        #endregion

        #region InfoCardsMapping
        private void GetInfoCardsMap()
        {
            string subPath = @"DATA\INTERFACE\infocardmap.ini";
            if (File.Exists(Path + subPath))
            {
                using (StreamReader reader = new StreamReader(Path + subPath))
                {
                    List<InfoCardMap> dataBlock = new List<InfoCardMap>();
                    string line = reader.ReadLine();

                    if (!line.Equals("[InfocardMapTable]"))
                        throw new Exception("Wrong format for infocardmap.ini");

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                        {
                            string first = line.Substring(line.IndexOf("=") + 1, line.IndexOf(",") - line.IndexOf("=") - 1).Replace(" ", "");
                            string second = line.Substring(line.IndexOf(",") + 1).Replace(" ", "");
                            dataBlock.Add(new InfoCardMap(first, second));
                        }
                    }
                    this.InfosMap = dataBlock;
                }
            }
            else
                throw new Exception("infocardmap.ini doesn't exist.");
        }
        #endregion
    }
}