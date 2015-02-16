using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Map
    {
        [DataMember]
        public List<LivingObject> LivingObjects;
        [DataMember]
        public string MapName;
        [DataMember]
        public int MapSize;
    }
}
