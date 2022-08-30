using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessDevelopment.Services.Models._ViewModels
{
    public class ReturnViewModel
    {
        [JsonProperty(PropertyName = "reference_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceId { get; set; } = null;
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = "";

        [JsonProperty(PropertyName = "is_success")]
        public bool IsSuccess { get { return String.IsNullOrEmpty(Message) || Message.Length < 1 || Message.ToUpper().IndexOf("ERROR") == -1; } }
    }
}
