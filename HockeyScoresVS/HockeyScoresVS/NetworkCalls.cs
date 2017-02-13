using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public static class NetworkCalls
    {
        public static async Task<JObject> ApiCallAsync(string uri)
        {
            string jsonFile = await GetJsonFromApiAsync(uri);
            return JObject.Parse(jsonFile.Substring(10, jsonFile.Length - 11));
        }

        public static async Task<string> GetJsonFromApiAsync(string uri)
        {
            WebRequest request = WebRequest.Create(uri);
            WebResponse response = await request.GetResponseAsync();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string jsonFile = await reader.ReadToEndAsync();
            reader.Close();
            response.Close();

            return jsonFile;
        }

        public static string GetJsonFromApi(string uri)
        {
            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string jsonFile = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return jsonFile;
        }
    }
}
