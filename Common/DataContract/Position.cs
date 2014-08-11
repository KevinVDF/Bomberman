using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Position
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
    }
}
