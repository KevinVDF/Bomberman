using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    [KnownType(typeof(Wall))]
    [KnownType(typeof(Bomb))]
    [KnownType(typeof(Bonus))]
    [KnownType(typeof(Player))]
    public abstract class LivingObject
    {
        [DataMember]
        public Position Position { get; set; }
        [DataMember]
        public int ID { get; set; }

        public bool ComparePosition(LivingObject objectToCompare)
        {
            return Position.X == objectToCompare.Position.X &&
                 Position.Y == objectToCompare.Position.Y;
        }
    }
}
