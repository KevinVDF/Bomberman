using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    [KnownType(typeof(Wall))]
    [KnownType(typeof(Bomb))]
    [KnownType(typeof(Bonus))]
    [KnownType(typeof(Player))]
    
    public class Game
    {
        [DataMember]
        public Map Map { get; set; }
        [DataMember]
        public GameStatus CurrentStatus { get; set; }
    }
}
