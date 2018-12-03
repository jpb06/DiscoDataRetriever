using DiscoTradingDataRetrieval.Wrappers.XMLRelated;
using System.Collections.Generic;
using System.Linq;

namespace DiscoDataRetriever.DataRetrieval.Ressources
{
    public class XMLParser
    {
        private string xml;
        private List<XmlElement> elements;

        public static string Parse(string rawXml)
        {
            string parsedData = "";
            List<string> lines = new List<string>();

            string workingHash = rawXml;
            bool treated = false;
            bool newPara = true;

            while (!treated)
            {
                // get the data segment
                int start = workingHash.IndexOf("<TEXT>") + 6;
                int end = workingHash.IndexOf("</TEXT>");
                string sectionToBeAdded = workingHash.Substring(start, end - start);

                // remove from the string the selected data segment
                workingHash = workingHash.Substring(workingHash.IndexOf("</TEXT>") + 7);

                if (!string.IsNullOrWhiteSpace(sectionToBeAdded) && !string.IsNullOrEmpty(sectionToBeAdded))
                {
                    if (newPara)
                        lines.Add(sectionToBeAdded);
                    else
                        lines[lines.Count - 1] = lines.ElementAt(lines.Count - 1) + sectionToBeAdded;
                }

                if (!workingHash.Contains("<TEXT>"))
                    treated = true;
                else
                {
                    if (workingHash.Substring(0, workingHash.IndexOf("</TEXT>")).Contains("<PARA/>"))
                        newPara = true;
                    else
                        newPara = false;
                }
            }

            foreach (string el in lines)
                parsedData += el + "|@";

            parsedData = parsedData.Substring(0, parsedData.Length - 2);
            return parsedData;
        }

        /*
        public XMLParser(string sXML)
        {
            if (!isXml(sXML))
                throw new Exception("The string "+ sXML +" isn't containing XML");
            
            this.xml = sXML.Substring(sXML.IndexOf("?>")+2);
            this.elements = new List<XmlElement>();

            string remainingXml = "";
            do
            {
                XmlElement el = new XmlElement(this.xml, out remainingXml);
                elements.Add(el);
            }
            while (remainingXml != "");

        }

        public static bool isXml(string sXML)
        {
            // if (sXML.StartsWith("<?xml version=\"1.0\" encoding=\"UTF-16\"?><RDL><PUSH/>") && sXML.EndsWith("<POP/></RDL>"))
            if (sXML.StartsWith("<?xml version=\"1.0\" encoding=\"UTF-16\"?>") )
                return true;
            else
                return false;
        }

        public static string ExtractInfo(string str)
        {
            int start = str.IndexOf("</TEXT><PARA/><TRA data=\"0\" mask=\"1\" def=\"-1\"/><JUST loc=\"left\"/><TEXT> </TEXT><PARA/><TEXT>") + 92;
            string ash = str.Substring(start, str.Length-(start));
            return ash;
        }
         */
    }
}

/*
<?xml version="1.0" encoding="UTF-16"?>
<RDL>
<PUSH/>
<TRA data="1" mask="1" def="-2"/><JUST loc="center"/>
<TEXT>ALUMINIUM</TEXT>
<PARA/>
<TRA data="0" mask="1" def="-1"/>
<JUST loc="left"/>
<TEXT> </TEXT>
<PARA/>
<TEXT>Aluminium is a silvery white, ductile metal classed in the boron family of chemical elements. Because aluminium is very chemically reactive, 
on planets it may only be commonly found combined with any of over 270 other minerals. In space, where Aluminium is formed by the action of gamma 
rays on lighter atomic elements, the metal may be found in its pure elemental form. Aluminium is very resistant to corrosion and possesses a low density, 
is nonmagnetic and nonsparking, and is a good thermal and electrical conductor. Aluminium is extremely important in the composition of various alloys 
essential to transportation, construction, and aerospace applications.</TEXT>
<PARA/>
<POP/>
</RDL>
*/
