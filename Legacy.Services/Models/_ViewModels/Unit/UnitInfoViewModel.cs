using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Unit
{
    public class UnitInfoViewModel: _ItemViewModel
    {
        [JsonProperty(PropertyName = "unit_id")]
        public int UnitId { get; set; }
        [JsonProperty(PropertyName = "owner_id")]
        public int? OwnerId { get; set; }
        [JsonProperty(PropertyName = "unit_name")]
        public string UnitName { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string ImageURL { get; set; }
        [JsonProperty(PropertyName = "address")]
        public AddressViewModel Address { get; set; }
    }
}
