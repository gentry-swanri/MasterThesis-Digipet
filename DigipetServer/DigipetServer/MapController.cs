using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace DigipetServer
{
    class MapController
    {
        private string format = "json";                                                                                 // format data of mapzen vector tile
        private string mapzenApiKey = "mapzen-KhT9o6J";                                                                 // mapzen api key. get the api key by sign up to mapzen
        private string mapzenUrl = "http://tile.mapzen.com/mapzen/vector/v1/{0}/{1}/{2}/{3}.{4}?api_key={5}";           // mapzen vector tile url. 0 => layers, 1 => zoom level, 2 => x tile coordinate, 3 => y tile coordinate, 4 => vector tile data format, 5 => mapzen api key
        private int zoom = 18;

        private float centerMercatorX;
        private float centerMercatorY;
        private float posX;
        private float posY;
        private int tileX;
        private int tileY;

        private HttpClient http;
        //private dynamic completeMapData;
        private ListMapData listMapData;
        private bool mapReady;

        private bool firstAcess;

        public MapController()
        {
            this.centerMercatorX = float.MinValue;
            this.centerMercatorY = float.MinValue;
            this.posX = float.MinValue;
            this.posY = float.MinValue;
            this.tileX = int.MinValue;
            this.tileY = int.MinValue;

            this.http = new HttpClient();
            //this.completeMapData = null;
            this.listMapData = new ListMapData();
            this.mapReady = false;

            this.firstAcess = true;
        }

        public bool ConvertLocationAndCheck(float latitude, float longitude)
        {
            float[] mercator = GeoConverter.GeoCoorToMercatorProjection(latitude, longitude);
            float[] pixel = GeoConverter.MercatorProjectionToPixel(mercator, zoom);
            int[] tile = GeoConverter.PixelToTileCoordinate(pixel);

            if (firstAcess)
            {
                centerMercatorX = mercator[0];
                centerMercatorY = mercator[1];
                firstAcess = false;
            }

            this.posX = mercator[0] - centerMercatorX;
            this.posY = mercator[1] - centerMercatorY;

            if (tile[0] == tileX && tile[1] == tileY)
            {
                return false;
            }else
            {
                tileX = tile[0];
                tileY = tile[1];
                return true;
            }
        }

        public async void CreateMap()
        {
            // http://tile.mapzen.com/mapzen/vector/v1/buildings,roads,pois/14/9685/6207.json?api_key=mapzen-KhT9o6J (for testing purpose)
            //string url = string.Format(mapzenUrl, "buildings,roads,pois", 14, 9685, 6207, format, mapzenApiKey);

            this.mapReady = false;
            string url = string.Format(mapzenUrl, "buildings,roads,pois", zoom.ToString(), tileX.ToString(), tileY.ToString(), format, mapzenApiKey);
            dynamic mapData = await this.ProcessMapData(url);
            
            
            this.ConvertBuildingData(mapData.buildings);
            this.ConvertRoadData(mapData.roads);
            this.ConvertPOIData(mapData.pois);

            //this.completeMapData = mapData;

            this.mapReady = true;
        }

        private async Task<dynamic> ProcessMapData(string url)
        {
            var response = await this.http.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            dynamic mapData = JsonConvert.DeserializeObject(result);

            return mapData;
        }

        private void ConvertBuildingData(dynamic building)
        {
            if (this.listMapData.listBuildingData == null)
            {
                this.listMapData.listBuildingData = new List<BuildingData>();
            }

            for (int i=0; i<building.features.Count; i++)
            {
                var tempData = building.features[i];
                if (tempData.geometry.type == "Polygon")
                {
                    BuildingData buildingData = new BuildingData();
                    buildingData.listCoordinate = new List<Coordinate>();

                    for (int j=0; j<tempData.geometry.coordinates[0].Count; j++)
                    {
                        var coordinate = tempData.geometry.coordinates[0][j];
                        float[] mercator = GeoConverter.GeoCoorToMercatorProjection((float)Convert.ToDouble(coordinate[1]), (float)Convert.ToDouble(coordinate[0]));
                        float tempX = mercator[0] - this.centerMercatorX;
                        float tempY = mercator[1] - this.centerMercatorY;
                        //coordinate[1] = tempX;
                        //coordinate[0] = tempY;
                        Coordinate coor = new Coordinate();
                        coor.latitude = tempX;
                        coor.longitude = tempY;
                        buildingData.listCoordinate.Add(coor);
                    }

                    buildingData.buildingName = tempData.properties.name;
                    this.listMapData.listBuildingData.Add(buildingData);
                }
                if (tempData.geometry.type == "Point")
                {
                    BuildingData buildingData = new BuildingData();
                    buildingData.listCoordinate = new List<Coordinate>();

                    var coordinate = tempData.geometry.coordinates;
                    float[] mercator = GeoConverter.GeoCoorToMercatorProjection((float)Convert.ToDouble(coordinate[1]), (float)Convert.ToDouble(coordinate[0]));
                    float tempX = mercator[0] - this.centerMercatorX;
                    float tempY = mercator[1] - this.centerMercatorY;
                    //coordinate[1] = tempX;
                    //coordinate[0] = tempY;
                    Coordinate coor = new Coordinate();
                    coor.latitude = tempX;
                    coor.longitude = tempY;
                    buildingData.listCoordinate.Add(coor);

                    buildingData.buildingName = tempData.properties.name;
                    this.listMapData.listBuildingData.Add(buildingData);
                    
                }
            }
        }

        private void ConvertRoadData(dynamic road)
        {
            this.listMapData.listRoadData = new List<RoadData>();

            for (int i=0; i<road.features.Count; i++)
            {
                var tempData = road.features[i];
                if (tempData.geometry.type == "LineString")
                {
                    RoadData roadData = new RoadData();
                    roadData.listCoordinate = new List<Coordinate>();

                    for (int j = 0; j < tempData.geometry.coordinates.Count; j++)
                    {
                        var coordinate = tempData.geometry.coordinates[j];
                        float[] mercator = GeoConverter.GeoCoorToMercatorProjection((float)Convert.ToDouble(coordinate[1]), (float)Convert.ToDouble(coordinate[0]));
                        float tempX = mercator[0] - this.centerMercatorX;
                        float tempY = mercator[1] - this.centerMercatorY;
                        //coordinate[1] = tempX;
                        //coordinate[0] = tempY;
                        Coordinate coor = new Coordinate();
                        coor.latitude = tempX;
                        coor.longitude = tempY;
                        roadData.listCoordinate.Add(coor);
                    }

                    roadData.roadName = tempData.properties.name;
                    this.listMapData.listRoadData.Add(roadData);
                }
            }
        }

        private void ConvertPOIData(dynamic poi)
        {
            if (this.listMapData.listBuildingData == null)
            {
                this.listMapData.listBuildingData = new List<BuildingData>();
            }

            for (int i=0; i<poi.features.Count; i++)
            {
                var tempData = poi.features[i];

                BuildingData buildingData = new BuildingData();
                buildingData.listCoordinate = new List<Coordinate>();

                var coordinate = tempData.geometry.coordinates;
                float[] mercator = GeoConverter.GeoCoorToMercatorProjection((float)coordinate[1], (float)coordinate[0]);
                float tempX = mercator[0] - this.centerMercatorX;
                float tempY = mercator[1] - this.centerMercatorY;
                //coordinate[1] = tempX;
                //coordinate[0] = tempY;
                Coordinate coor = new Coordinate();
                coor.latitude = tempX;
                coor.longitude = tempY;
                buildingData.listCoordinate.Add(coor);

                buildingData.buildingName = tempData.properties.name;
                this.listMapData.listBuildingData.Add(buildingData);
            }
        }

        public List<Coordinate> StartRoute(float latitude, float longitude, string destination)
        {
            RouteManagement route = new RouteManagement(this.centerMercatorX, this.centerMercatorY, latitude, longitude, this.mapzenApiKey, this.http);
            route.StartRouting(destination);

            while (!route.GetRoutingDone()) ;

            return route.GetFinalRoute();
        }

        public void SetPosX(float posX)
        {
            this.posX = posX;
        }

        public float GetPosX()
        {
            return this.posX;
        }

        public void SetPosY(float posY)
        {
            this.posY = posY;
        }

        public float GetPosY()
        {
            return this.posY;
        }

        public void SetTileX(int tileX)
        {
            this.tileX = tileX;
        }

        public int GetTileX()
        {
            return this.tileX;
        }

        public void SetTileY(int tileY)
        {
            this.tileY = tileY;
        }

        public int GetTileY()
        {
            return this.tileY;
        }

        public void SetListMapData(ListMapData listMapData)
        {
            this.listMapData = listMapData;
            //this.completeMapData = completeMapData;
        }

        public dynamic GetListMapData()
        {
            return this.listMapData;
            //return this.completeMapData;
        }

        public void SetMapReady(bool mapReady)
        {
            this.mapReady = mapReady;
        }

        public bool GetMapReady()
        {
            return this.mapReady;
        }
    }
}
