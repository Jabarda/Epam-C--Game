using System.Collections.Generic;
using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    public class MoveRequest : Request {
        public MoveRequest() {
            RequesType = RequesType.MoveRequest;
            Direction = new List<int>();
        }

        [ProtoMember(1)]
        public List<int> Direction { get; set; }
    }
}