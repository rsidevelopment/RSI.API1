using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels
{
    public class _SearchViewModel
    {
        [JsonProperty(PropertyName = "start_row_index", NullValueHandling = NullValueHandling.Ignore)]
        public int? StartRowIndex { get; set; } = null;
        [JsonProperty(PropertyName = "number_of_rows", NullValueHandling = NullValueHandling.Ignore)]
        public int? NumberOfRows { get; set; } = null;
        [JsonProperty(PropertyName = "sort_column", NullValueHandling = NullValueHandling.Ignore)]
        public string SortColumn { get; set; } = null;
        [JsonProperty(PropertyName = "sort_direction", NullValueHandling = NullValueHandling.Ignore)]
        public string SortDirection { get; set; } = null;
        [JsonProperty(PropertyName = "exact_match", NullValueHandling = NullValueHandling.Ignore)]
        public bool ExactMatch { get; set; } = true;

    }
}
