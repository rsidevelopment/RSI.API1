using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Package
{
    public class PackageListViewModel
    {
        [JsonProperty(PropertyName = "package_id")]
        public int PackageId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";
        [JsonProperty(PropertyName = "condo_weeks")]
        public int CondoWeeks { get; set; }
        [JsonProperty(PropertyName = "rci_weeks")]
        public int RCIWeeks { get; set; }
        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }
    }
}
