using System.Collections.Generic;
using ProtoBuf;

namespace ProtocolLibrary.Datastructures {
    [ProtoContract]
    public class User {
        [ProtoMember(1)] public List<int> Position;

        public User() {
            Position = new List<int>();
        }

        [ProtoMember(2)]
        public int Id { get; set; }
    }
}