using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    public class MoveResponse : Response {
        [ProtoMember(1)]
        public bool Ok { get; set; }
    }
}