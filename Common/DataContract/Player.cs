using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Player : LivingObject
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public bool IsCreator { get; set; }
        [DataMember]
        public int BombPower { get; set; }
        [DataMember]
        public int BombNumber { get; set; }
        [DataMember]
        public bool CanShootBomb { get; set; }
    }
}
