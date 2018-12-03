using DiscoDataRetriever.DataRetrieval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FLDataRetriever fldr = new FLDataRetriever(@"D:\Games\Discovery_testdataextract\");
                fldr.FetchData();

                int ass = 0;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
