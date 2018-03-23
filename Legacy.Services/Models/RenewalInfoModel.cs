using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("UsersMasterRenewalInfo")]
    public class RenewalInfoModel
    {
        [Key, Column("memberID")]
        public int RSIId { get; set; }
        [Column("recordType")]
        public string RecordType { get; set; }
        [Column("billingLocation")]
        public string BillingLocation { get; set; }
        [Column("renewalFee")]
        public string RenewalFee { get; set; }
        [Column("billingFrequency")]
        public string BillingFrequency { get; set; }
        [Column("autoDraftDiscount")]
        public decimal AutoDraftDiscount { get; set; }
        [Column("accountStatus")]
        public string AccountStatus { get; set; }
        [Column("dateOfNextBill")]
        public DateTime? DateOfNextBill { get; set; }
        [Column("hasAccess")]
        public bool? HasAccess { get; set; }
        [Column("dateOfLastPaid")]
        public DateTime? DateOfLastPaid { get; set; }

    }
}
