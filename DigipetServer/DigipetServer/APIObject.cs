using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.IO;

namespace DigipetServer
{
    [DataContract]
    public class Geometry
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<List<List<object>>> coordinates { get; set; }
    }

    [DataContract]
    public class Properties
    {
        [DataMember]
        public string kind { get; set; }
        [DataMember]
        public int area { get; set; }
        [DataMember]
        public int sort_rank { get; set; }
        [DataMember]
        public string kind_detail { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public double min_zoom { get; set; }
        [DataMember]
        public string landuse_kind { get; set; }
        [DataMember]
        public int scale_rank { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string addr_street { get; set; }
        [DataMember]
        public double height { get; set; }
        [DataMember]
        public int volume { get; set; }
        [DataMember]
        public string addr_housenumber { get; set; }
    }

    [DataContract]
    public class Feature
    {
        [DataMember]
        public Geometry geometry { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public Properties properties { get; set; }
    }

    [DataContract]
    public class Buildings
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<Feature> features { get; set; }
    }

    [DataContract]
    public class Geometry2
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<double> coordinates { get; set; }
    }

    [DataContract]
    public class Properties2
    {
        [DataMember]
        public string kind { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public List<string> subway_routes { get; set; }
        [DataMember]
        public int kind_tile_rank { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public double min_zoom { get; set; }
        [DataMember]
        public bool is_subway { get; set; }
        [DataMember]
        public object id { get; set; }
        [DataMember]
        public int tier { get; set; }
        [DataMember]
        public int area { get; set; }
    }

    [DataContract]
    public class Feature2
    {
        [DataMember]
        public Geometry2 geometry { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public Properties2 properties { get; set; }
    }

    [DataContract]
    public class Pois
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<Feature2> features { get; set; }
    }

    [DataContract]
    public class Geometry3
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<List<object>> coordinates { get; set; }
    }

    [DataContract]
    public class Properties3
    {
        [DataMember]
        public string kind { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int sort_rank { get; set; }
        [DataMember]
        public string landuse_kind { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public double min_zoom { get; set; }
        [DataMember]
        public string kind_detail { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public bool is_bus_route { get; set; }
        [DataMember]
        public List<string> all_networks { get; set; }
        [DataMember]
        public string shield_text { get; set; }
        [DataMember]
        public string @ref { get; set; }
        [DataMember]
        public List<string> all_shield_texts { get; set; }
        [DataMember]
        public string network { get; set; }
        [DataMember]
        public bool is_link { get; set; }
        [DataMember]
        public bool is_bridge { get; set; }
        [DataMember]
        public string foot { get; set; }
        [DataMember]
        public string reg_name { get; set; }
        [DataMember]
        public bool is_tunnel { get; set; }
        [DataMember]
        public string horse { get; set; }
        [DataMember]
        public string bicycle { get; set; }
        [DataMember]
        public string motor_vehicle { get; set; }
    }

    [DataContract]
    public class Feature3
    {
        [DataMember]
        public Geometry3 geometry { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public Properties3 properties { get; set; }
    }

    [DataContract]
    public class Roads
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<Feature3> features { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public Buildings buildings { get; set; }
        [DataMember]
        public Pois pois { get; set; }
        [DataMember]
        public Roads roads { get; set; }
    }

    public class APIObject
    {
        public async static Task<RootObject> GetMapData(string url)
        {
            var http = new HttpClient();
            var response = await http.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }

    // references : http://liliankasem.com/2015/10/17/api-calls-using-httpclient-and-deserializing-json-in-c/
}
