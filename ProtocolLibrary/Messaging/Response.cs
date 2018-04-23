using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    [ProtoInclude(2001, typeof (MoveResponse))]
    public class Response : Message {
        public Response() {
            Type = MessageTypes.Response;
        }
    }
}