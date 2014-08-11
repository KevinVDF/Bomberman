using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Bomb : LivingObject
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int PlayerId { get; set; }
        [DataMember]
        public int Power { get; set; }
    }
}
