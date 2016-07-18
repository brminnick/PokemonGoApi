using System;
using System.Diagnostics;

namespace PokemonGoApi
{
    public class GoApi
    {
        private string m_accessToken;
        private double m_coordLatitude, m_coordLongitude, m_coordAltitude;
        private string m_apiEndPoint;
        public GoApi(string accessToken)
        {
            m_coordLatitude = 0;
            m_coordLongitude = 0;
            m_coordAltitude = 0;

            m_accessToken = accessToken;
            m_apiEndPoint = "https://" + getApiEndPoint() + "/rpc";
            Debug.WriteLine("[!] accessToken : {0}\n[!] apiEndPoint : {1}", m_accessToken, m_apiEndPoint);
        }
        Proto.ResponseEnvelop apiRequest(string apiEndPoint, Proto.RequestEnvelop requestEnvelop)
        {
            requestEnvelop.Direction = Proto.Direction.Request;
            requestEnvelop.RpcId = 7309341774315520108;

            requestEnvelop.Latitude = m_coordLatitude;
            requestEnvelop.Longitude = m_coordLongitude;
            requestEnvelop.Altitude = m_coordAltitude;

            requestEnvelop.Unknown12 = 989;
            requestEnvelop.Auth = new Proto.RequestEnvelop.Types.AuthInfo();
            requestEnvelop.Auth.Provider = "ptc";
            requestEnvelop.Auth.Token = new Proto.RequestEnvelop.Types.AuthInfo.Types.JWT();
            requestEnvelop.Auth.Token.Contents = m_accessToken;
            requestEnvelop.Auth.Token.Unknown13 = 59;

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            Google.Protobuf.CodedOutputStream outStream = new Google.Protobuf.CodedOutputStream(memoryStream, false);
            requestEnvelop.WriteTo(outStream);
            outStream.Flush();
            memoryStream.Position = 0;

            var memoryReader = new System.IO.BinaryReader(memoryStream);
            byte[] data = memoryReader.ReadBytes((int)memoryStream.Length);


            data = HttpHelper.Post(apiEndPoint, data);

            Proto.ResponseEnvelop responseEnvelop = new Proto.ResponseEnvelop();

            Google.Protobuf.MessageParser<Proto.ResponseEnvelop> messageParser = new Google.Protobuf.MessageParser<Proto.ResponseEnvelop>(() => { return new Proto.ResponseEnvelop(); });
            responseEnvelop = messageParser.ParseFrom(data);

            return responseEnvelop;
        }
        string getApiEndPoint()
        {
            Proto.RequestEnvelop requestEnvelop = new Proto.RequestEnvelop();
            requestEnvelop.Requests.Add(new Proto.Request() { Type = Proto.Method.GetPlayer });
            Proto.ResponseEnvelop responseEnvelop = apiRequest("https://pgorelease.nianticlabs.com/plfe/rpc", requestEnvelop);

            return responseEnvelop.ApiUrl;
        }
        public void setLocation(double latitude, double longitude, double altitude)
        {
            m_coordLatitude = latitude;
            m_coordLongitude = longitude;
            m_coordAltitude = altitude;
        }
        public Proto.ClientPlayerDetails getPlayerDetails()
        {
            Proto.RequestEnvelop requestEnvelop = new Proto.RequestEnvelop();
            requestEnvelop.Requests.Add(new Proto.Request() { Type = Proto.Method.GetPlayer });

            Proto.ResponseEnvelop responseEnvelop = apiRequest(m_apiEndPoint, requestEnvelop);
            Google.Protobuf.MessageParser<Proto.ClientPlayerDetails> messageParser = new Google.Protobuf.MessageParser<Proto.ClientPlayerDetails>(() => { return new Proto.ClientPlayerDetails(); });

            return messageParser.ParseFrom(responseEnvelop.Payload[0].Data.ToByteArray());
        }
    }
}
