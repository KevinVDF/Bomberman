using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Player : LivingObject
    {
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public int BombPower { get; set; }
        [DataMember]
        public int BombNumber { get; set; }
        [DataMember]
        public bool CanShootBomb { get; set; }
        [DataMember]
        public bool Alive { get; set; }
    }
}
