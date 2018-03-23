using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Member
{
    public class MemberUpgradeViewModel: _ItemViewModel
    {
        [JsonProperty(PropertyName = "upgrade_audit_id"), Required]
        public int UpgradeAuditId { get; set; }
        [JsonProperty(PropertyName = "member_id"), Required]
        public int RSIId { get; set; }
        [JsonProperty(PropertyName = "old_rsi_org_id"), Required]
        public int OldRSIOrgId { get; set; }
        [JsonProperty(PropertyName = "old_package_id"), Required]
        public int OldPackageId { get; set; }
        [JsonProperty(PropertyName = "new_rsi_org_id"), Required]
        public int NewRSIOrgId { get; set; }
        [JsonProperty(PropertyName = "new_package_id"), Required]
        public int NewPackageId { get; set; }
        [JsonProperty(PropertyName = "upgrade_date"), Required]
        public DateTime UpgradeDate { get; set; }
        [JsonProperty(PropertyName = "upgrade_agent_id"), Required, StringLength(450)]
        public string UpgradeAgentId { get; set; }
        [JsonProperty(PropertyName = "upgrade_program"), Required, StringLength(50)]
        public string UpgradeProgram { get; set; }
        [JsonProperty(PropertyName = "condo_weeks"), Required]
        public int CondoWeeks { get; set; }
        [JsonProperty(PropertyName = "rci_weeks"), Required]
        public int RCIWeeks { get; set; }
        [JsonProperty(PropertyName = "points"), Required]
        public float Points { get; set; }
        [JsonProperty(PropertyName = "upgrade_price"), Required]
        public decimal UpgradePrice { get; set; }
        [JsonProperty(PropertyName = "down_payment"), Required]
        public decimal DownPayment { get; set; }
        [JsonProperty(PropertyName = "financed_amount"), Required]
        public decimal FinancedAmount { get; set; }
        [JsonProperty(PropertyName = "finance_term"), Required]
        public int FinanceTerm { get; set; }
    }
}
