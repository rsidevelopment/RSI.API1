using Newtonsoft.Json;

namespace Legacy.Services.Models._ViewModels.Unit
{
    public class AddressViewModel
    {
        [JsonProperty(PropertyName = "street_address")]
        public string StreetAddress { get; set; }
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "state_code")]
        public string StateCode { get; set; }
        [JsonProperty(PropertyName = "state_full_name")]
        public string StateFullName { get; set; }
        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }
        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }
        [JsonProperty(PropertyName = "country_full_name")]
        public string CountryFullName { get; set; }
        [JsonProperty(PropertyName = "region_code")]
        public string RegionCode { get; set; }
        [JsonProperty(PropertyName = "region_full_name")]
        public string RegionFullName { get; set; }
    }
}
