using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using Newtonsoft.Json;

namespace DigipetServer
{
    class RouteManagement
    {

        private HttpClient httpClient;
        private string searchUrl;
        private string routeUrl;

        private float centerMercatorX;
        private float centerMercatorY;
        private float latitude;
        private float longitude;

        private List<Coordinate> finalRoute;

        private bool routingDone;

        public RouteManagement(float centerMercatorX, float centerMercatorY, float latitude, float longitude, HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.searchUrl = "https://api.opencagedata.com/geocode/v1/json?q={0}&key=2393494991af4eaf9866f28fbd3d739e";
            this.routeUrl = "http://osrm.pptik.id/route/v1/driving/{0}?alternatives=false&overview=full&steps=true";

            this.centerMercatorX = centerMercatorX;
            this.centerMercatorY = centerMercatorY;
            this.latitude = latitude;
            this.longitude = longitude;

            this.finalRoute = new List<Coordinate>();

            this.routingDone = false;
        }

        public void StartRouting(string routeDestination)
        {
            this.SearchRoute(routeDestination);
        }

        private async void SearchRoute(string routeDestination)
        {
            string destUpdate = routeDestination.Replace(" ", "%20");
            string url = string.Format(searchUrl, destUpdate);
            dynamic searchData = null;
            try
            {
                searchData = await this.ProcessData(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            
            if (searchData.results != null)
            {
                
                var coordinate = searchData.results[0].geometry;
                float latitudeSearch = (float)Convert.ToDouble(coordinate["lat"]);
                float longitudeSearch = (float)Convert.ToDouble(coordinate["lng"]);
                    
                string json = this.longitude+"%2C"+this.latitude+"%3B"+longitudeSearch+"%2C"+latitudeSearch;
                string url2 = string.Format(routeUrl, json);
                dynamic routeData = await this.ProcessData(url2);

                if (routeData != null)
                {
                    this.ProcessRouteData(routeData);
                }
                
            }

            this.routingDone = true;
        }

        private async Task<dynamic> ProcessData(string url)
        {
            var response = await this.httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(result);

            return data;
        }

        private void ProcessRouteData(dynamic routeData)
        {
            string shape = (string)routeData.routes[0].geometry;
            shape = shape.Replace("\\\\", "\\"); // remove the escaped character '\'

            PolylineDecoder pd = new PolylineDecoder();
            List<Coordinate> listCoor = pd.Decode(shape, 5);
            for (int i = 0; i < listCoor.Count; i++)
            {
                Coordinate coor = listCoor[i];
                float[] mercator = GeoConverter.GeoCoorToMercatorProjection((float)Convert.ToDouble(coor.latitude), (float)Convert.ToDouble(coor.longitude));
                float tempX = mercator[0] - this.centerMercatorX;
                float tempY = mercator[1] - this.centerMercatorY;

                Coordinate newCoor = new Coordinate();
                newCoor.latitude = tempX;
                newCoor.longitude = tempY;

                this.finalRoute.Add(newCoor);
            }
        }

        public List<Coordinate> GetFinalRoute()
        {
            return this.finalRoute;
        }

        public bool GetRoutingDone()
        {
            return this.routingDone;
        }

    }
}
