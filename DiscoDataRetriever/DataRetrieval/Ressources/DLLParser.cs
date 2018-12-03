using DiscoTradingDataRetrieval.Wrappers.DLLRelated;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex;
using DiscoTradingDataRetrieval.Wrappers.DLLRelated.Complex.Final;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscoDataRetriever.DataRetrieval.Ressources
{
    public class DLLParser : IDisposable
    {
        public BinaryReader Reader { get; set; }
        public string Source { get; set; }
        public List<ConsResProperties> Basestrings { get; set; }
        public List<ConsResProperties> Xmlstrings { get; set; }

        public DLLParser(string filePath, string source)
        {
            this.Source = source;
            this.Reader = new BinaryReader(File.Open(filePath, FileMode.Open), Encoding.UTF8);
            this.Basestrings = new List<ConsResProperties>();
            this.Xmlstrings = new List<ConsResProperties>();
        }

        public void GetResources()
        {
            // 64 bytes
            #region MZ-DOS
            // Magic number 
            char[] e_magic = this.Reader.ReadChars(2);
            // Bytes on last page of file
            byte[] e_cblp = this.Reader.ReadBytes(2);
            // Pages in file
            byte[] e_cp = this.Reader.ReadBytes(2);
            // Relocations
            byte[] e_crlc = this.Reader.ReadBytes(2);
            // Size of header in paragraphs
            byte[] e_cparhdr = this.Reader.ReadBytes(2);
            // Minimum extra paragraphs needed
            byte[] e_minalloc = this.Reader.ReadBytes(2);
            // Maximum extra paragraphs needed
            byte[] e_maxalloc = this.Reader.ReadBytes(2);
            // Initial (relative) SS value
            byte[] e_ss = this.Reader.ReadBytes(2);
            // Initial SP value
            byte[] e_sp = this.Reader.ReadBytes(2);
            // Checksum
            byte[] e_csum = this.Reader.ReadBytes(2);
            // Initial IP value
            byte[] e_ip = this.Reader.ReadBytes(2);
            // Initial (relative) CS value
            byte[] e_cs = this.Reader.ReadBytes(2);
            // File address of relocation table
            byte[] e_lfarlc = this.Reader.ReadBytes(2);
            // Overlay number
            byte[] e_ovno = this.Reader.ReadBytes(2);
            // Reserved words
            byte[] e_res = this.Reader.ReadBytes(8);
            // OEM identifier (for e_oeminfo)
            byte[] e_oemid = this.Reader.ReadBytes(2);
            // OEM information; e_oemid specific
            byte[] e_oeminfo = this.Reader.ReadBytes(2);
            // Reserved words
            byte[] e_res2 = this.Reader.ReadBytes(20);
            // File address of new exe header
            int e_lfanew = this.Reader.ReadInt32();
            #endregion

            #region DOS
            // e_lfanew - this.Reader.BaseStream.Position
            #endregion

            // 24 bytes
            #region PE_File_Signature
            this.Reader.BaseStream.Position = e_lfanew;

            byte[] signature = this.Reader.ReadBytes(4);
            byte[] machine = this.Reader.ReadBytes(2);
            int numberOfSections = this.Reader.ReadInt16();
            byte[] timeDateStamp = this.Reader.ReadBytes(4);
            byte[] pointerToSymbolTable = this.Reader.ReadBytes(4);
            byte[] numberOfSymbols = this.Reader.ReadBytes(4);
            int sizeOfOptionalHeader = this.Reader.ReadInt16();
            byte[] characteristics = this.Reader.ReadBytes(2);

            // optional
            this.Reader.BaseStream.Position += sizeOfOptionalHeader;
            #endregion

            #region sections
            List<SectionHeader> headers = new List<SectionHeader>();
            int i;
            for (i = 0; i < numberOfSections; i++)
            {
                SectionHeader header = new SectionHeader();

                char[] sName = this.Reader.ReadChars(8);
                header.Name = new string(sName).Replace("\0", string.Empty);
                header.VirtualAddress = this.Reader.ReadInt32();
                header.SizeOfRawData = this.Reader.ReadInt32();
                header.PointerToRawData = this.Reader.ReadInt32();
                header.PointerToRelocations = this.Reader.ReadInt32();
                header.PointerToLinenumbers = this.Reader.ReadInt32();
                header.NumberOfRelocations = this.Reader.ReadInt32();
                header.NumberOfLinenumbers = this.Reader.ReadInt32();
                header.Characteristics = this.Reader.ReadInt32();

                headers.Add(header);
            }
            #endregion

            #region get_to_rsrc
            int rscIndex = headers.IndexOf(headers.Where(hdr => hdr.Name == ".rsrc").FirstOrDefault());

            if (rscIndex > 0)
            {
                i = 0;
                this.Reader.BaseStream.Position = 0;
                while (i < rscIndex)
                {
                    SectionHeader header = headers.ElementAt(i);
                    this.Reader.BaseStream.Position += header.PointerToRawData + header.SizeOfRawData;
                    i++;
                }
            }
            else if (rscIndex == 0)
            {
                this.Reader.BaseStream.Position = headers.ElementAt(0).SizeOfRawData;
            }
            #endregion

            // start of rsrc
            ResourceDirectory baseRD = new ResourceDirectory(this.Reader);

            List<ResourceDirectoryEntry> baseEntries = new List<ResourceDirectoryEntry>();
            for (i = 0; i < baseRD.NumberOfIdEntries; i++)
            {
                baseEntries.Add(new ResourceDirectoryEntry(this.Reader));
            }

            List<ResourceTable> leaves = new List<ResourceTable>();
            for (i = 0; i < baseEntries.Count; i++)
            {
                ResourceTable cLeaf = new ResourceTable((int)baseEntries.ElementAt(i).Name);
                cLeaf.AddHeader(this.Reader);

                for (int j = 0; j < cLeaf.Header.NumberOfIdEntries; j++)
                {
                    cLeaf.AddEntry(this.Reader);
                }

                leaves.Add(cLeaf);
            }

            for (i = 0; i < leaves.Count; i++)
            {
                for (int j = 0; j < leaves.ElementAt(i).Header.NumberOfIdEntries; j++)
                {
                    leaves.ElementAt(i).AddLanguageEntry(this.Reader);
                }
            }

            for (i = 0; i < leaves.Count; i++)
            {
                for (int j = 0; j < leaves.ElementAt(i).Header.NumberOfIdEntries; j++)
                {
                    leaves.ElementAt(i).AddDataEntry(this.Reader);
                }
            }

            for (i = 0; i < leaves.Count; i++)
            {
                for (int j = 0; j < leaves.ElementAt(i).Lvl3_Data.Count; j++)
                {
                    leaves.ElementAt(i).Addstrings(this.Reader,
                                                   (int)leaves.ElementAt(i).Lvl3_Data.ElementAt(j).OffsetToData,
                                                   (int)leaves.ElementAt(i).Lvl3_Data.ElementAt(j).Size,
                                                   (int)leaves.ElementAt(i).Lvl1_Entries.ElementAt(j).Name);
                }
            }

            for (i = 0; i < leaves.Count; i++)
            {
                if (leaves.ElementAt(i).Type == ResType.String)
                    this.Basestrings.AddRange(leaves.ElementAt(i).GetResources(this.Source));
                else
                    this.Xmlstrings.AddRange(leaves.ElementAt(i).GetResources(this.Source));
            }
        }

        ~DLLParser()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.Reader != null)
            {
                this.Reader.Close();
                this.Reader.Dispose();
                this.Reader = null;
            }
        }
    }
}