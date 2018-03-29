using Newtonsoft.Json;

namespace Legacy.Services.Models._ViewModels.Unit
{
    public class UnitListViewModel: _ItemViewModel
    {
        [JsonProperty(PropertyName = "unit_id")]
        public int UnitId { get; set; }
        string _originalUnitId;
        [JsonProperty(PropertyName = "original_unit_id")]
        public string OriginalUnitId
        {
            get
            {
                return _originalUnitId ?? string.Empty;
            }
            set
            {
                _originalUnitId = value;
            }
        }
        [JsonProperty(PropertyName = "owner_id")]
        public int? OwnerId { get; set; }
        [JsonProperty(PropertyName = "unit_name")]
        public string UnitName { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "image_url")]
        public string ImageURL { get; set; }
        [JsonProperty(PropertyName = "lowest_net_rate")]
        public decimal LowestNetRate { get; set; }
        [JsonProperty(PropertyName = "address")]
        public AddressViewModel Address { get; set; }
        [JsonIgnore]
        public int MaxRows { get; set; }
    }
}
