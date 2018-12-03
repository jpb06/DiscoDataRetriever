using System;
using System.IO;

namespace DiscoTradingDataRetrieval.Wrappers.DLLRelated.Primitives
{
    public class Directorystring
    {
        public int Id { get; set; }
        public string Data { get; set; }

        public Directorystring()
        {
            this.Id = -1;
            this.Data = null;
        }

        public Directorystring(string data, int id)
        {
            this.Data = data;
            this.Id = id;
        }

        public Directorystring(BinaryReader br, int parentLength, int id)
        {
            int uselessLen = 0;
            do
            {
                uselessLen = br.ReadUInt16();
            }
            while (uselessLen == 0 || uselessLen == 16720);

            char[] buffer = new char[parentLength - 2];

            int i = 0;
            do
            {
                buffer[i] = Convert.ToChar(br.ReadUInt16());
                i++;
            }
            while (i < ((parentLength - 2) / 2));

            this.Data = new string(buffer);
            this.Id = id;
        }
    }
}
