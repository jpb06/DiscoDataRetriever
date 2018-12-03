using DiscoDataRetriever.Wrappers.Internal;
using System;
using System.Collections.Generic;

namespace DiscoTradingDataRetrieval.Wrappers.XMLRelated
{
    public class XmlElement
    {
        private string rawXML_TAG;
        private string rawXML_Inner;

        private string tag;
        private List<KeyValue> parameters;

        private List<XmlElement> childs;

        public XmlElement(string rawXml, out string remainingXml)
        {
            this.rawXML_TAG = rawXml.Substring(0, rawXml.IndexOf(">") + 1);
            this.tag = this.GetTag(this.rawXML_TAG);

            if (this.rawXML_TAG.Contains("/>"))
                this.rawXML_Inner = null;
            else
            {
                this.rawXML_Inner = rawXml.Substring(rawXml.IndexOf(">") + 1, rawXml.IndexOf("</" + this.tag + ">") - (this.tag.Length + 2));
                
            }

            remainingXml = rawXml.Substring(rawXml.IndexOf("</" + this.tag + ">") + (this.tag.Length + 3));
        }

        private string GetTag(string rawXML)
        {
            string theTag = rawXML.Substring(1);
            if (!rawXML.Contains("="))
                theTag = theTag.Substring(0, theTag.IndexOf(">"));
            else
                theTag = theTag.Substring(0, theTag.IndexOf(" "));

            return theTag;
        }
    }
}