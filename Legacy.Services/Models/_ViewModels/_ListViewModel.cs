using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels
{
    public class _ListViewModel<T>
    {
        [JsonProperty(PropertyName = "rows")]
        public List<T> Rows = new List<T>();
        [JsonProperty(PropertyName = "row_count")]
        public int RowCount { get { return Rows.Count; } }
        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = "";
        [JsonProperty(PropertyName = "is_success")]
        public bool IsSuccess { get { return Message == null || Message.Length < 1 || Message.ToUpper().IndexOf("ERROR") == -1; } }
    }
}
