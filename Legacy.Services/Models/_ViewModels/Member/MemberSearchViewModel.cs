using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Member
{
    public class MemberSearchViewModel : _SearchViewModel
    {
        private string _phone = "";
        [JsonProperty(PropertyName = "catch_all")]
        public string CatchAll { get; set; } = "";
        [JsonProperty(PropertyName = "first_name")]
        public string FistName { get; set; } = "";
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; } = "";
        [JsonProperty(PropertyName = "email")]
        public string Email = "";
        [JsonProperty(PropertyName = "phone")]
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _phone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "organization_id")]
        public int? OrganizationId { get; set; } = null;
    }
}
