
using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public enum ErrorType
    {
        [EnumMember]
        Callback,
        [EnumMember]
        Connection,
        [EnumMember]
        Disconnection,
        [EnumMember]
        GameCreation,
    }
}
