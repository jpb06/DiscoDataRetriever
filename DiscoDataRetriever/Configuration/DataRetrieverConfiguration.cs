using System.Collections.Generic;
using System.IO;

namespace DiscoDataRetriever
{
    public enum DllResource { nameresources, infocards, misctext, Discovery, DsyAddition };

    public static class Configuration
    {
        public const string BasePath = @"D:\Games\Discovery_testdataextract\";

        public static string GetDllPath(DllResource ressource)
        {
            return Path.Combine(BasePath, DllFiles[ressource]);
        }

        public static Dictionary<DllResource, string> DllFiles = new Dictionary<DllResource, string>()
        {
            { DllResource.nameresources,  @"EXE\nameresources.dll" },
            { DllResource.infocards,  @"EXE\infocards.dll" },
            { DllResource.misctext,  @"EXE\misctext.dll" },
            { DllResource.Discovery,  @"EXE\Discovery.dll" },
            { DllResource.DsyAddition, @"EXE\DsyAddition.dll" }
        };


    }
}
