using Newtonsoft.Json;

namespace Legacy.Services.Models._ViewModels.Unit
{
    public class Amenity
    {
        string _loc;
        [JsonProperty(PropertyName = "location")]
        public string Location
        {
            get { switch (_loc)
                {
                    case "IU": return "In Unit";
                    case "OP": return "On Property";
                    default: return "Off Property";
                }
            }
            set { _loc = value; }
        }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "distance")]
        public double? Distance { get; set; }
        string _mk;
        [JsonProperty(PropertyName = "distance_unit")]
        public string MK
        {
            get { return (_mk == "K") ? "Kilometers" : "Miles"; }
            set { _mk = value; }
        }
    }
}
