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
<<<<<<< HEAD
        public int BombNumber { get; set; }
=======
        public int MaxBombCount { get; set; }
        [DataMember]
        public bool CanShootBomb { get; set; }


        public bool CompareId(LivingObject objectToCompare)
        {
            if (GetType() == objectToCompare.GetType())
                return Id == ((Player) objectToCompare).Id;
            return false;
        }
>>>>>>> origin/master
    }
}
