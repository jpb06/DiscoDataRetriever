using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex.Final;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex
{
    public class Resourcestrings
    {
        public List<Directorystring> Strings { get; set; }

        public Resourcestrings()
        {
            this.Strings = new List<Directorystring>();
        }

        public Resourcestrings(BinaryReader br, int offset, int length, ResType type, int id)
        {
            br.BaseStream.Position = offset;
            int endPos = offset + length;

            this.Strings = new List<Directorystring>();

            if (type == ResType.String)
            {
                int index = 0;

                while (br.BaseStream.Position < endPos)
                {
                    int sLength = 0;

                    do
                    {
                        sLength = br.ReadUInt16();
                        if (sLength == 0)
                            index++;
                        if (br.BaseStream.Position >= endPos)
                            return;
                    }
                    while (sLength == 0 || sLength == 16720);

                    char[] buffer = new char[sLength];

                    int i = 0;
                    do
                    {
                        buffer[i] = Convert.ToChar(br.ReadUInt16());
                        i++;
                    }
                    while (i < sLength);

                    this.Strings.Add(new Directorystring(new string(buffer), (id - 1) * 16 + index));
                    index++;
                }
                if (index != 16)
                    throw new Exception("string bundle error (Must contain 16 strings");
            }
            else if (type == ResType.XML)
            {
                this.Strings.Add(new Directorystring(br, length, id));
            }
        }
    }
}
