
using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public enum ErrorType
    {
        [EnumMember]
        Connection,
        [EnumMember]
        Callback
    }
}
