using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    [ProtoInclude(1001, typeof (MoveRequest))]
    public class Request : Message {
        public Request() {
            Type = MessageTypes.Request;
        }

        [ProtoMember(1)]
        public RequesType RequesType { get; set; }
    }
}