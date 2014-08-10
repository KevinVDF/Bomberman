﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Map
    {
        [DataMember]
        public List<LivingObject> GridPositions;
        [DataMember]
        public string MapName;
        [DataMember]
        public int MapSize;
    }
}
