using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cult_Penguin
{
    public class RESTHandler
    {
        private static RESTHandler instance;

        private HttpClient httpClient;
        private string url;

        private RESTHandler()
        {
            httpClient = new HttpClient();
            url = "http://localhost:5000/Game/";
            httpClient.Timeout = TimeSpan.FromSeconds(1000);
        }

        public static RESTHandler Instance { get {
                if (instance == null) {
                    instance = new RESTHandler();
                }
                return instance;
            } 
        }

        public async Task CheckForUpdateAsync() {
            try
            {
                HttpResponseMessage responseGet = await httpClient.GetAsync(url);
                if (responseGet.IsSuccessStatusCode) {
                    string result = await responseGet.Content.ReadAsStringAsync();
                    int res = Convert.ToInt32(result);
                    if (res > GameWorld.Instance.LastUpdate) {
                        GameWorld.Instance.LastUpdate = res;
                        GameWorld.Instance.UpdateAvailable = true;
                    }

                }
            }
            catch (HttpRequestException e) {
                Console.WriteLine(e.ToString());
            }
            


        }

    }
}
