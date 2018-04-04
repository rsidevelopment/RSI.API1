using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Member
{
    public class MemberListViewModel
    {
        [JsonProperty(PropertyName = "member_id")]
        public int MemberId { get; set; }
        [JsonProperty(PropertyName = "organization_id")]
        public int OrganizationId { get; set; }
        [JsonProperty(PropertyName = "organization_name")]
        public string OrganizationName { get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "middle_name")]
        public string MiddleName { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "phone_1")]
        public string Phone1 { get; set; }
        [JsonProperty(PropertyName = "phone_2")]
        public string Phone2 { get; set; }
        [JsonProperty(PropertyName = "package_id")]
        public int PackageId { get; set; } = 0;
        [JsonProperty(PropertyName = "package_name")]
        public string PackageName { get; set; }
    }
}
