using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels
{
    public class _ItemViewModel
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "is_success")]
        public bool IsSuccess
        {
            get { return Message == null || Message.ToUpper().IndexOf("ERROR") == -1; }
        }
    }
}
