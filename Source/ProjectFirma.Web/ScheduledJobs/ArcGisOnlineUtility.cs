using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.ScheduledJobs
{
    public class ArcGisOnlineUtility
    {
        

        public Task UpdateFeatures(int tenantID)
        {
            //HttpClient httpClient = new HttpClient();

            //if (tenantID == 2)
            //{
            //    if (!AddUserAuthToken(httpClient))
            //    {
            //        return;
            //    }
            //    var cwsArcGIS = new CWSArcGIS(_configuration, _logger, "cws");
            //    cwsArcGIS.UpdateFeatures(httpClient);
            //}
            //else if (tenantID == 1)
            //{
            //    if (!AddApplicationAuthToken(httpClient, _configuration["ARCGIS_METRO_CLIENT_ID"], _configuration["ARCGIS_METRO_CLIENT_SECRET"]))
            //    {
            //        return;
            //    }
            //    var metroArcGIS = new MetroArcGIS(_configuration, _logger, "metro");
            //    metroArcGIS.UpdateFeatures(httpClient);
            //}
            //else
            //{
            //    _logger.Warning("Not Implemented Yet");
            //}
            throw new NotImplementedException();
        }

        public T ProcessResponse<T>(HttpResponseMessage response)
        {
            using (var sr = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    T responseObject;
                    try
                    {
                        responseObject = serializer.Deserialize<T>(jsonTextReader);
                    }
                    catch (Exception ex)
                    {
                        ILog Logger = LogManager.GetLogger(typeof(ArcGisOnlineUtility));
                        Logger.Error($"Unable to deserialize response: {ex}");
                        throw ex;
                    }

                    return responseObject;
                }
            }
        }



        public bool AddApplicationAuthToken(HttpClient httpClient)
        {
            var url = FirmaWebConfiguration.ArcGisAuthUrl;

            var body = new Dictionary<string, string>();
            body.Add("client_id", FirmaWebConfiguration.ArcGisClientId);
            body.Add("client_secret", FirmaWebConfiguration.ArcGisClientSecret);
            body.Add("grant_type", "client_credentials");

            var htmlContent = new FormUrlEncodedContent(body);
            HttpResponseMessage response;

            ArcGISApplicationAuthorizationDto arcGISAuthorization;
            try
            {
                response = httpClient.PostAsync(url, htmlContent).Result;
                response.EnsureSuccessStatusCode();
                arcGISAuthorization = ProcessResponse<ArcGISApplicationAuthorizationDto>(response);
            }
            catch (Exception e)
            {
                ILog Logger = LogManager.GetLogger(typeof(ArcGisOnlineUtility));
                Logger.Error("ArcGIS: Unable to get token.", e);
                return false;
            }

            if (httpClient.DefaultRequestHeaders.Contains("X-Esri-Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("X-Esri-Authorization");
            }
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Esri-Authorization", $"Bearer {arcGISAuthorization.access_token}");
            return true;
        }


        public string GetDataImportAuthTokenFromUser()
        {
            using (var hc = new HttpClient())
            {
                var requestBody = new[] {
                    new KeyValuePair<string, string>("username", FirmaWebConfiguration.DataImportAuthUsername),
                    new KeyValuePair<string, string>("password", FirmaWebConfiguration.DataImportAuthPassword),
                    new KeyValuePair<string, string>("referer", "localhost"),
                    new KeyValuePair<string, string>("f", "json"),
                };
                var requestMsg = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(FirmaWebConfiguration.DataImportAuthUrl),
                    Content = new FormUrlEncodedContent(requestBody)
                };

                requestMsg.Headers.Accept.Clear();
                requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom"));

                var authResp = hc.SendAsync(requestMsg);
                var respRaw = authResp.Result.Content.ReadAsStringAsync().Result;

                /* response example.
                    {
                        "token":    "PBUEmJESrS9s6d8zSAc-6fXbV2eLRaSPyAYF843vTK6jyaWCRg9S6z5gwbSSzBCzE2l6JUJ1qJMoAH5YhYTH_fBMcDbYK-J8T-Pr_2h_r9El3C5E7yAa21VRiKZmzNJK",
                        "expires":  1668317334177,
                        "ssl":      true
                    }
                 */

                //Console.WriteLine($"\r\n-==Authentication Result==-\r\n{respRaw}");

                dynamic tmp = JsonConvert.DeserializeObject(respRaw);
                return Convert.ToString(tmp.token);
            }
        }






        public class ArcGISApplicationAuthorizationDto
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }
    }
}