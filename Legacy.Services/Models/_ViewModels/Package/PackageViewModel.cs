using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Legacy.Services.Models._ViewModels.Package
{
    public class PackageViewModel : _ItemViewModel
    {
        [JsonProperty(PropertyName = "package_id")]
        public int PackageId { get; set; } = 0;
        [JsonProperty(PropertyName = "package_name")]
        public string PackageName { get; set; } = "";
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";
        [JsonProperty(PropertyName = "billing_frequency")]
        public int BillingFrequency { get; set; } = 365;
        [JsonProperty(PropertyName = "billing_first_amount")]
        public decimal BillingFirstAmount { get; set; } = 0;
        [JsonProperty(PropertyName = "billing_first_date")]
        public string BillingFirstDate { get; set; } = "365";
        [JsonProperty(PropertyName = "hide_videos")]
        public bool HideVideos { get; set; } = false;
        [JsonProperty(PropertyName = "hide_local_phone")]
        public bool HideLocalPhone { get; set; } = false;
        [JsonProperty(PropertyName = "hide_toll_free_phone")]
        public bool HideTollFreePhone { get; set; } = false;
        [JsonProperty(PropertyName = "show_footer_baner")]
        public bool ShowFooterBanner { get; set; } = false;
        [JsonProperty(PropertyName = "send_activation_email")]
        public bool SendActivationEmail { get; set; } = true;
        [JsonProperty(PropertyName = "activation_email_delay_days")]
        public int ActivationEmailDelayDays { get; set; } = 0;
        [JsonProperty(PropertyName = "points_in_same_bucket")]
        public bool PointsInSameBucket { get; set; } = false;
        [JsonProperty(PropertyName = "condo_retail_add_price")]
        public decimal CondoRetailAddPrice { get; set; } = 0;
        [JsonProperty(PropertyName = "points_type")]
        public string PointsType { get; set; } = "Regular";
        [JsonProperty(PropertyName = "margin_discount")]
        public int MarginDiscount { get; set; } = 0;
        [JsonProperty(PropertyName = "below_retail_discount")]
        public int BelowRetailDiscount { get; set; } = 0;
        [JsonProperty(PropertyName = "condo_weeks")]
        public int CondoWeeks { get; set; } = 0;
        [JsonProperty(PropertyName = "is_unlimited_points")]
        public bool IsUnlimitedPoints { get; set; } = false;
        [JsonIgnore]
        public string SetBenefitsString
        {
            set
            {
                if(value != null && value.Length > 0)
                    BenefitsArray = value.Split(',').Select(Int32.Parse).ToList();
            }
        }
        [JsonProperty(PropertyName = "benefits_ids")]
        public List<int> BenefitsArray { get; set; } = new List<int>();
        [JsonProperty(PropertyName = "benefits")]
        public List<PackageBenefitViewModel> Benefits { get; set; } = new List<PackageBenefitViewModel>();
    }

    public class PackageBenefitViewModel
    {
        [JsonProperty(PropertyName = "benefit_id")]
        public int BenefitId { get; set; } = 0;
        [JsonProperty(PropertyName = "benefit_name")]
        public string BenefitName { get; set; } = "";
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";
        [JsonProperty(PropertyName = "short_decription")]
        public string ShortDescription { get; set; } = "";
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; } = "";
    }
}
