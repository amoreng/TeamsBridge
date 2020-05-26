using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TeamsBridge.Helpers
{
    public class GraphSettings
    {
        public string Host { get; set; }      
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string TenantID { get; set; }       
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class GraphUtils
    {
        /// <summary>
        /// Get Access Token on Behalf of A User using the RestAPI
        /// </summary>
        /// <param name="_host"></param>
        /// <param name="_tenantID"></param>
        /// <param name="_clientId"></param>
        /// <param name="_clientSecret"></param>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        public static bool DelegatedTokenRest(string _host, string _tenantID, string _clientId, string _userName, string _password, string _clientSecret,  out string token)
        {
            token = null;
            var client = new RestClient($"{_host }{_tenantID}");
            var request = new RestRequest("/oauth2/token", Method.POST);

            var encClientSec = HttpUtility.UrlEncode(_clientSecret);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=password&resource=https://graph.microsoft.com&client_id={_clientId}&username={_userName}&password={_password}&client_secret={encClientSec}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            //contains the token            
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                case System.Net.HttpStatusCode.Unauthorized:
                default:
                    {
                        return false;
                    }
                case System.Net.HttpStatusCode.OK:
                    string msg = response.Content;
                    {
                        dynamic val = JsonConvert.DeserializeObject(msg);
                        if (!String.IsNullOrEmpty(val.access_token.ToString()))
                        {
                            token = val.access_token;
                            return true;
                        }
                        else
                        {
                            token = null;
                            return false;
                        }
                    }
            }
        }
    }
}
