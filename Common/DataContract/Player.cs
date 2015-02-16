using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Player : LivingObject
    {
        [DataMember]
        public int Id { get; set; }
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
<<<<<<< HEAD
<<<<<<< HEAD
        public int BombNumber { get; set; }
=======
        public int MaxBombCount { get; set; }
        [DataMember]
        public bool CanShootBomb { get; set; }
=======
        public int MaxBombCount { get; set; }
>>>>>>> parent of eeb1811... rewrite model objects
=======
        public int MaxBombCount { get; set; }
>>>>>>> parent of eeb1811... rewrite model objects


        public bool CompareId(LivingObject objectToCompare)
        {
            if (GetType() == objectToCompare.GetType())
                return Id == ((Player) objectToCompare).Id;
            return false;
        }
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> origin/master
=======
>>>>>>> parent of eeb1811... rewrite model objects
=======
>>>>>>> parent of eeb1811... rewrite model objects
    }
}
