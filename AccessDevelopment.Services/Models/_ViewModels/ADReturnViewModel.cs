﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessDevelopment.Services.Models._ViewModels
{
    public class ADListReturnViewModel
    {
        //[JsonProperty(PropertyName = "info", NullValueHandling = NullValueHandling.Ignore)]
        //public ADInfo Info { get; set; } = null;
        [JsonProperty(PropertyName = "data")]
        public List<ADReturnViewModel> Items { get; set; } = new List<ADReturnViewModel>();
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = "";
        [JsonProperty(PropertyName = "is_success")]
        public bool IsSuccess { get { return !String.IsNullOrEmpty(Message) || Message.ToUpper().IndexOf("ERROR") == -1; } }
    }

    public class ADReturnViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; } = 0;
        [JsonProperty(PropertyName = "created_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreationDate { get; set; } = DateTime.Now;
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = "NA";
        [JsonProperty(PropertyName = "valid_members_count")]
        public int ValidMemberCount { get; set; } = 0;
        [JsonProperty(PropertyName = "invalid_members_count")]
        public int InvalidMemberCount { get; set; } = 0;
        [JsonProperty(PropertyName = "imported_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ImportedDate { get; set; } = DateTime.Now;
        [JsonProperty(PropertyName = "links")]
        public ADLinks Links { get; set; } = new ADLinks();
        [JsonProperty(PropertyName = "submitted_by", NullValueHandling = NullValueHandling.Ignore)]
        public string SubmittedBy { get; set; } = null;
    }

    public class ADLinks
    {
        [JsonProperty(PropertyName = "show_import")]
        public string ShowImportLink { get; set; } = "";
        [JsonProperty(PropertyName = "valid_members_csv")]
        public string ValidMembersCSVLink { get; set; } = "";
        [JsonProperty(PropertyName = "invalid_members_csv", NullValueHandling = NullValueHandling.Ignore)]
        public string InvalidMembersCSVLink { get; set; } = null;
    }

    public class ADInfo
    {
        [JsonProperty(PropertyName = "total_results")]
        public int TotalResults { get; set; }
        [JsonProperty(PropertyName = "current_page")]
        public int CurrentPage { get; set; }
        [JsonProperty(PropertyName = "total_pages")]
        public int TotalPages { get; set; }
        [JsonProperty(PropertyName = "results_per_page")]
        public int ResultsPerPage { get; set; }
    }
}
