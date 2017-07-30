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
        private string mapzenApiKey;

        private float centerMercatorX;
        private float centerMercatorY;
        private float latitude;
        private float longitude;

        private List<Coordinate> finalRoute;

        public RouteManagement(float centerMercatorX, float centerMercatorY, float latitude, float longitude, string mapzenKey)
        {
            this.httpClient = new HttpClient();
            this.searchUrl = "https://search.mapzen.com/v1/search?text={0}&api_key={1}";
            this.routeUrl = "https://valhalla.mapzen.com/route?json={0}&api_key={1}";
            this.mapzenApiKey = mapzenKey;
            //this.mapzenApiKey = "mapzen-KhT9o6J";

            this.centerMercatorX = centerMercatorX;
            this.centerMercatorY = centerMercatorY;
            this.latitude = latitude;
            this.longitude = longitude;

            this.finalRoute = new List<Coordinate>();
        }

        public void StartRouting(string routeDestination)
        {
            string shapeResult = this.SearchRoute(routeDestination);
        }

        private string SearchRoute(string routeDestination)
        {
            string destUpdate = routeDestination.Replace(" ", "%20");
            string url = string.Format(searchUrl, destUpdate, mapzenApiKey);
            dynamic searchData = this.ProcessData(url);
            if (searchData.features != null)
            {
                int pos = -1;
                for (int i = 0; i < searchData.features.Count; i++)
                {
                    string name = searchData.features[i].properties.name.str;
                    if (name.ToLower() == routeDestination.ToLower())
                    {
                        pos = i;
                        break;
                    }
                }

                if (pos != -1)
                {
                    var coordinate = searchData.features[0].geometry.coordinates;
                    float latitudeSearch = (float)Convert.ToDouble(coordinate[1]);
                    float longitudeSearch = (float)Convert.ToDouble(coordinate[0]);
                    
                    string json = "{\"locations\":[{\"lat\":" + latitude + ",\"lon\":" + longitude + ",\"type\":\"break\"},{\"lat\":" + latitudeSearch + ",\"lon\":" + longitudeSearch + ",\"type\":\"break\"}],\"costing\":\"auto\"}";
                    string url2 = string.Format(routeUrl, json, mapzenApiKey);
                    dynamic routeData = this.ProcessData(url2);

                    if (routeData.trip != null)
                    {
                        this.ProcessRouteData(routeData);
                    }
                }
            }

            return "";
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
            string shape = routeData.trip.legs[0].shape.str;
            shape = shape.Replace("\\\\", "\\"); // remove the escaped character '\'

            PolylineDecoder pd = new PolylineDecoder();
            List<Coordinate> listCoor = pd.Decode(shape, 6);
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

    }
}
