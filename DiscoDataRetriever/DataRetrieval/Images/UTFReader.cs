using DiscoDataRetriever.Wrappers.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscoDataRetriever.DataRetrieval.Images
{
    public class UTFReader : IDisposable
    {
        private BinaryReader reader;

        public UTFReader(string filePath) 
        {
            this.reader = new BinaryReader(File.Open(filePath, FileMode.Open), Encoding.UTF8);
        }

        public string GetImage(string destPath, string nickName) 
        {
            try
            {
                char[] buffer = new char[4];
                this.reader.Read(buffer, 0, 4);
                string ash = new string(buffer);

                int ver = this.reader.ReadInt32();

                if (!ash.Equals("UTF ") && ver == 257)
                    throw new Exception("Wrong file format");
 
                int nodeOffset = this.reader.ReadInt32();
                int nodeSize = this.reader.ReadInt32();
                  
                this.reader.BaseStream.Position = this.reader.BaseStream.Position + 8;

                int stringOffset = this.reader.ReadInt32();
                int stringSize = this.reader.ReadInt32();
                  
                this.reader.BaseStream.Position = this.reader.BaseStream.Position + 4;

                this.reader.BaseStream.Position = 36;
                int dataOffset = this.reader.ReadInt32();
                int dateSize = (int)(this.reader.BaseStream.Length - this.reader.BaseStream.Position);

                #region get nodes
                this.reader.BaseStream.Position = nodeOffset;

                List<PrimitiveNode> nodes = new List<PrimitiveNode>();
                while(this.reader.BaseStream.Position < nodeOffset+nodeSize)
                {
                    nodes.Add(this.GetNode(this.reader));
                }
                #endregion

                #region get strings
                foreach (PrimitiveNode node in nodes) 
                {
                    node.Nodestring = this.Getstring(this.reader, node, stringOffset);
                  //  node.PrintNode();
                }
                #endregion

                PrimitiveNode mipNode = (from node in nodes
                          where node.Nodestring.Equals("MIP0")
                          select node).FirstOrDefault();

                if (mipNode == null)
                    throw new Exception("Couldn't locate mip node");

                this.reader.BaseStream.Position = dataOffset + mipNode.ChildOffset;

                byte[] fileBytes = new byte[mipNode.Size];
                this.reader.Read(fileBytes, 0, fileBytes.Length);

                string fileName = this.getFileName(nodes, nodeOffset);
                if (fileName == "")
                    fileName = nickName;
                fileName += ".jpg";

                Bitmap image = TargaImage.LoadTargaImage(fileBytes);
                image.Save(destPath + fileName);

                return fileName;

                #region test
                /*
                string path = @"C:\Users\Yaoquizque\Desktop\FreelancerTrading\FilesFeed\commoditiesModels\TGA\";
                using (BinaryWriter bw = new BinaryWriter(File.Open(path + this.getFileName(nodes, nodeOffset), FileMode.Create)))
                {
                    bw.Write(fileBytes);
                }*/
                #endregion

            }
            finally
            {
                this.Dispose();
            }
        }

        private PrimitiveNode GetNode(BinaryReader br)
        {
            int initialPos = (int)br.BaseStream.Position;
            return new PrimitiveNode(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(),
                br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), initialPos);
        }

        private string Getstring(BinaryReader br, PrimitiveNode node, int stringOffset) 
        {
            string currstring = "";

            this.reader.BaseStream.Position = stringOffset + node.StringOffset;

            int currChar = 0;
            while ((currChar = this.reader.ReadByte()) != 0)
            {
                currstring += Convert.ToChar(currChar);
            }
            return currstring;
        }

        private string getFileName(List<PrimitiveNode> pNodes, int nodeOffset) 
        {
            PrimitiveNode tlNode = (from pnode in pNodes
                                   where pnode.Nodestring.Equals("Texture library")
                                   select pnode).FirstOrDefault();

            if (tlNode == null)
                throw new Exception("Couldn't find Texture Library node");

            PrimitiveNode tlSubNode = (from pnode in pNodes
                                       where pnode.InitialPos == nodeOffset + tlNode.ChildOffset
                                       select pnode).FirstOrDefault();

            if (tlNode == null)
                throw new Exception("Couldn't find Texture Library sub node");

            if (tlSubNode.PeerNextOffset == 0) 
            {
                string fileName = "";
                if (tlSubNode.Nodestring.EndsWith(".tga"))
                    fileName = tlSubNode.Nodestring.Remove(tlSubNode.Nodestring.LastIndexOf(".tga"), 4);

                return fileName; 
            }
            else
                throw new Exception("Multiple texture library sub nodes");
        }

        ~UTFReader() 
        {
            this.Dispose();
        }

        public void Dispose() 
        {
            if (this.reader != null)
            {
                this.reader.Close();
                this.reader.Dispose();
                this.reader = null;
            }
        }
    }
}