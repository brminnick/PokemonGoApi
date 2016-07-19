using System;
using System.Diagnostics;
using PokemonGoApi.Proto;
using Google.Protobuf;

namespace PokemonGoApi
{
    public class GoApi
    {
        private string m_accessToken, m_apiEndPoint;
        private double m_coordLatitude, m_coordLongitude, m_coordAltitude;
        private AuthenticationService.AuthenticationType m_authType;

        public GoApi(string accessToken, AuthenticationService.AuthenticationType authType)
        {
            m_coordLatitude = 0;
            m_coordLongitude = 0;
            m_coordAltitude = 0;

            m_authType = authType;

            m_accessToken = accessToken;
            m_apiEndPoint = "https://" + getApiEndPoint() + "/rpc";
            Debug.WriteLine($"[!] accessToken : {m_accessToken}\n[!] apiEndPoint : {m_apiEndPoint}");
        }
        ResponseEnvelop apiRequest(string apiEndPoint, RequestEnvelop requestEnvelop)
        {
            requestEnvelop.Direction = Direction.Request;
            requestEnvelop.RpcId = 7309341774315520108;

            requestEnvelop.Latitude = m_coordLatitude;
            requestEnvelop.Longitude = m_coordLongitude;
            requestEnvelop.Altitude = m_coordAltitude;

            requestEnvelop.Unknown12 = 989;
            requestEnvelop.Auth = new RequestEnvelop.Types.AuthInfo()
            {
                Provider = (m_authType == AuthenticationService.AuthenticationType.Google) ? "google" : "ptc",
                Token = new RequestEnvelop.Types.AuthInfo.Types.JWT()
                {
                    Contents = m_accessToken,
                    Unknown13 = 59
                }
            };

            var memoryStream = new System.IO.MemoryStream();
            var outStream = new CodedOutputStream(memoryStream, false);
            requestEnvelop.WriteTo(outStream);
            outStream.Flush();
            memoryStream.Position = 0;

            var memoryReader = new System.IO.BinaryReader(memoryStream);
            byte[] data = memoryReader.ReadBytes((int)memoryStream.Length);


            data = HttpHelper.Post(apiEndPoint, data);

            var responseEnvelop = new ResponseEnvelop();

            var messageParser = new MessageParser<ResponseEnvelop>(() => { return new ResponseEnvelop(); });
            responseEnvelop = messageParser.ParseFrom(data);

            return responseEnvelop;
        }
        string getApiEndPoint()
        {
            var requestEnvelop = new RequestEnvelop();
            requestEnvelop.Requests.Add(new Request() { Type = Method.GetPlayer });
            var responseEnvelop = apiRequest("https://pgorelease.nianticlabs.com/plfe/rpc", requestEnvelop);

            return responseEnvelop.ApiUrl;
        }
        public void setLocation(double latitude, double longitude, double altitude)
        {
            m_coordLatitude = latitude;
            m_coordLongitude = longitude;
            m_coordAltitude = altitude;
        }
        public ClientPlayerDetails getPlayerDetails()
        {
            var requestEnvelop = new RequestEnvelop();
            requestEnvelop.Requests.Add(new Request() { Type = Method.GetPlayer });

            var responseEnvelop = apiRequest(m_apiEndPoint, requestEnvelop);
            var messageParser = new MessageParser<ClientPlayerDetails>(() => { return new ClientPlayerDetails(); });

            return messageParser.ParseFrom(responseEnvelop.Payload[0].Data.ToByteArray());
        }
    }
}
