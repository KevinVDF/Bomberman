using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Wall : LivingObject
    {
        [DataMember]
        public WallType WallType { get; set; }
    }
    [DataContract]
    public enum WallType
    {
        [EnumMember]
        Destructible,
        [EnumMember]
        Undestructible
    }
}
