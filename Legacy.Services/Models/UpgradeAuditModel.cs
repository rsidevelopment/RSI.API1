using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("UpgradeAudits")]
    public class UpgradeAuditModel
    {
        [Key, Required]
        public int UpgradeAuditId { get; set; }
        [Required]
        public int RSIId { get; set; }
        [Required, Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Required]
        public int OldRSIOrgId { get; set; }
        [Required]
        public int OldPackageId { get; set; }
        [Required]
        public int NewRSIOrgId { get; set; }
        [Required]
        public int NewPackageId { get; set; }
        [Required, Column(TypeName = "datetime")]
        public DateTime UpgradeDate { get; set; }
        [Required, StringLength(450)]
        public string UpgradeAgentId { get; set; }
        [Required, StringLength(50)]
        public string UpgradeProgram { get; set; }
        [Required]
        public int CondoWeeks { get; set; }
        [Required]
        public int RCIWeeks { get; set; }
        [Required]
        public float Points { get; set; }
        [Required]
        public decimal UpgradePrice { get; set; }
        [Required]
        public decimal DownPayment { get; set; }
        [Required]
        public decimal FinancedAmount { get; set; }
        [Required]
        public int FinanceTerm { get; set; }
    }
}
