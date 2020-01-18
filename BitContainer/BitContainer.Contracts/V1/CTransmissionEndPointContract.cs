using System;
using System.Net;
using System.Runtime.Serialization;

namespace BitContainer.Contracts.V1
{
    [DataContract]
    public class CTransmissionEndPointContract
    {
        [DataMember]
        public Int16 Port { get; set; }

        public IPAddress Address { get; set; }
        [DataMember] private String _address;

        [OnSerializing]
        void PrepareForSerialization(StreamingContext sc)
        {
            _address = Address.ToString();
        }

        [OnDeserialized]
        void CompleteDeserialization(StreamingContext sc)
        {
            Address = IPAddress.Parse(_address);
        }

        public static CTransmissionEndPointContract Create(IPAddress address, Int16 port)
        {
            return new CTransmissionEndPointContract()
            {
                Address = address,
                Port = port
            };
        }
    }
}
