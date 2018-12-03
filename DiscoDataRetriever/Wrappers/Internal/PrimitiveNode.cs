using System;

namespace DiscoDataRetriever.Wrappers.Internal
{
    public class PrimitiveNode
    {
        public int PeerNextOffset { get; set; }
        public int StringOffset { get; set; }
        public int Flags { get; set; }
        public int Zero { get; set; }
        public int ChildOffset { get; set; }	// node segment if node, data segment if leaf
        public int AllocatedSize { get; set; }
        public int Size { get; set; }
        public int Size2 { get; set; }
        public int U1 { get; set; }
        public int U2 { get; set; }
        public int U3 { get; set; }
        public string Nodestring { get; set; }
        public int InitialPos { get; set; }

        public PrimitiveNode()
        {
            this.PeerNextOffset = -1;
            this.StringOffset = -1;
            this.Flags = -1;
            this.Zero = -1;
            this.ChildOffset = -1;
            this.AllocatedSize = -1;
            this.Size = -1;
            this.Size2 = -1;
            this.U1 = -1;
            this.U2 = -1;
            this.U3 = -1;
            this.Nodestring = null;
            this.InitialPos = -1;
        }

        public PrimitiveNode(int PeerNextOffset, int stringOffset, int Flags, int Zero, int ChildOffset,
            int AllocatedSize, int Size, int Size2, int U1, int U2, int U3, int InitialPos)
        {
            this.PeerNextOffset = PeerNextOffset;
            this.StringOffset = stringOffset;
            this.Flags = Flags;
            this.Zero = Zero;
            this.ChildOffset = ChildOffset;
            this.AllocatedSize = AllocatedSize;
            this.Size = Size;
            this.Size2 = Size2;
            this.U1 = U1;
            this.U2 = U2;
            this.U3 = U3;
            this.Nodestring = "";
            this.InitialPos = InitialPos;
        }

        public void PrintNode()
        {
            Console.WriteLine("peer next offset : " + this.PeerNextOffset);
            Console.WriteLine("string offset : " + this.StringOffset);
            Console.WriteLine("Flags : " + this.Flags);
            Console.WriteLine("Zero : " + this.Zero);
            Console.WriteLine("child offset : " + this.ChildOffset);
            Console.WriteLine("allocated Size : " + this.AllocatedSize);
            Console.WriteLine("Size : " + this.Size);
            Console.WriteLine("Size2 : " + this.Size2);
            Console.WriteLine("U1 : " + this.U1);
            Console.WriteLine("U2 : " + this.U2);
            Console.WriteLine("U3 : " + this.U3);
            Console.WriteLine("node string : " + this.Nodestring);
            Console.WriteLine("initial pos : " + this.InitialPos);
            Console.WriteLine("---------------------------------------------");
        }
    }
}
