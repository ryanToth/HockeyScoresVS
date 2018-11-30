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
            JObject retVal = new JObject();

            try
            {
                retVal = JObject.Parse(jsonFile.Substring(10, jsonFile.Length - 11));
            }
            catch { }

            return retVal;
        }

        public static async Task<string> GetJsonFromApiAsync(string uri)
        {
            WebRequest request = WebRequest.Create(uri);
            WebResponse response;
            Stream dataStream = null;
            StreamReader reader = null;
            string jsonFile = "";

            try
            {
                response = await request.GetResponseAsync();

                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);

                jsonFile = await reader.ReadToEndAsync();
            }
            catch { }
            finally
            {
                if (dataStream != null)
                {
                    dataStream.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return jsonFile;
        }
    }
}
