using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoTradingDataRetrieval.Wrappers.Business
{
    public enum ConnectionType{ Null, Jumphole, Jumpgate, Nomadgate, DysonAirlock }

    public class SystemConnections
    {
        public ConnectionType Type { get; set; }
        public string SourceNickname { get; set; }
        public string DestinationNickname { get; set; }

        public SystemConnections()
        {
            this.Type = ConnectionType.Null;
            this.SourceNickname = null;
            this.DestinationNickname = null;
        }

        public SystemConnections(ConnectionType type, string sourceNickName, string destNickName)
        {
            this.Type = type;
            this.SourceNickname = sourceNickName;
            this.DestinationNickname = destNickName;
        }
    }
}