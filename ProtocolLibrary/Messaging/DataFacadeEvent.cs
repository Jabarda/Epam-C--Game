﻿using System.Collections.Generic;
using ProtoBuf;
using ProtocolLibrary.Datastructures;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    public class DataFacadeEvent : Event {
        public DataFacadeEvent() {
            State = new List<User>();
        }

        [ProtoMember(1)]
        public List<User> State { get; set; }
    }
}