using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("users")]
    public class MemberModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("keyid")]
        public int MemberId { get; set; }
        [StringLength(100), Column("username")]
        public string UserName { get; set; }

        [StringLength(100), Column("password")]
        public string Password { get; set; }
        //[Column("org")]
        public int? org { get; set; }

        [StringLength(50), Column("language")]
        public string Language { get; set; }
        [Column("admin")]
        public int? Admin { get; set; }
        [Column("userlevel")]
        public int? UserLevel { get; set; }
        [Column("guest")]
        public bool? Guest { get; set; }
        [Column("prompts")]
        public bool? Prompts { get; set; }
        [Column("creationdate")]
        public DateTime? CreationDate { get; set; }
        [Column("creatorid")]
        public int? CreatorId { get; set; }
        [Column("lastlogin")]
        public DateTime? LastLogin { get; set; }

        [StringLength(50), Column("loginip")]
        public string LoginIp { get; set; }

        [StringLength(100)]//, Column("fname")]
        public string fname { get; set; }

        [StringLength(2), Column("mi")]
        public string MiddleInitial { get; set; }

        [StringLength(255)]//, Column("lname")]
        public string lname { get; set; }

        [StringLength(255), Column("address")]
        public string Address { get; set; }

        [StringLength(100), Column("city")]
        public string City { get; set; }

        [StringLength(50), Column("state")]
        public string StateCode { get; set; }

        [StringLength(25), Column("zip")]
        public string PostalCode { get; set; }

        [StringLength(50), Column("country")]
        public string CountryCode { get; set; }

        [StringLength(25)]//, Column("phone1")]
        public string phone1 { get; set; }

        [StringLength(10), Column("ext1")]
        public string Extension1 { get; set; }

        [StringLength(25)]//, Column("phone2")]
        public string phone2 { get; set; }

        [StringLength(10), Column("ext2")]
        public string Extension2 { get; set; }

        [StringLength(100)]//, Column("email")]
        public string email { get; set; }
        [Column("active")]
        public bool? IsActive { get; set; }

        [Column("comments", TypeName = "text")]
        public string Comments { get; set; }
        [Column("addrep")]
        public bool? AddRep { get; set; }
        [Column("addprovider")]
        public bool? AddProvider { get; set; }
        [Column("rep")]
        public bool? Rep { get; set; }
        [Column("provider")]
        public bool? Provider { get; set; }
        [Column("renewaldate")]
        public DateTime? RenewalDate { get; set; }
        [Column("activationdate")]
        public DateTime? ActivationDate { get; set; }
        [Column("dr")]
        public bool? DR { get; set; }

        [StringLength(50), Column("dramt")]
        public string HotelRewards { get; set; }
        [Column("sentdr")]
        public bool? SentToDR { get; set; }
        [Column("drdate")]
        public DateTime? DateSentToDR { get; set; }

        [StringLength(255), Column("company")]
        public string Company { get; set; }

        [StringLength(50), Column("memberstatus")]
        public string MemberStatus { get; set; }

        [StringLength(50), Column("currency")]
        public string Currency { get; set; }

        [StringLength(100), Column("fname2")]
        public string FirstName2 { get; set; }

        [StringLength(255), Column("lname2")]
        public string LastName2 { get; set; }
        [Column("partner")]
        public bool? Partner { get; set; }

        [StringLength(50), Column("amountpaid")]
        public string AmountPaid { get; set; }

        [StringLength(100), Column("email2")]
        public string email2 { get; set; }
        [Column("viewonly")]
        public bool? ViewOnly { get; set; }
        [Column("multipleorgs")]
        public bool? MultipleOrgs { get; set; }

        [Column("orgnames", TypeName = "text")]
        public string OrganizationNames { get; set; }

        [StringLength(100), Column("siteblockreason")]
        public string SiteBlockReason { get; set; }
        [Column("returneddr")]
        public bool? ReturnedToDR { get; set; }
        [Column("drreturndate")]
        public DateTime? DateReturnedToDR { get; set; }
        [Column("bulknumber")]
        public bool? BulkNumber { get; set; }

        [StringLength(1), Column("mi2")]
        public string MiddleInitial2 { get; set; }
        [Column("template")]
        public bool? IsTemplate { get; set; }
        [Column("reject")]
        public bool? Reject { get; set; }

        [Column("rejectreason", TypeName = "text")]
        public string RejectReason { get; set; }
        [Column("checkinfo")]
        public bool? CheckInfo { get; set; }
        [Column("templateadd")]
        public DateTime? TemplateAddDate { get; set; }

        [Column("family", TypeName = "text")]
        public string Family { get; set; }

        [StringLength(50), Column("cruiserewards")]
        public string CruiseRewards { get; set; }

        [StringLength(50), Column("condorewards")]
        public string CondoRewards { get; set; }
        [Column("orgcompany")]
        public int? OrganizationCompany { get; set; }
        [Column("primarymember")]
        public int? PrimaryMember { get; set; }
        [Column("resortlookup")]
        public bool? ResortLookup { get; set; }

        [StringLength(50), Column("unitcost")]
        public string unitcost { get; set; }

        [StringLength(50), Column("approvalcode")]
        public string ApprovalCode { get; set; }
        [Column("minivacpackage")]
        public int? MiniVacPackage { get; set; }

        [StringLength(500), Column("minivacupgrades")]
        public string MiniVacUpgrades { get; set; }
        [Column("minivacs")]
        public bool? MiniVacs { get; set; }
        [Column("processdate")]
        public DateTime? SalesDate { get; set; }

        [StringLength(100), Column("internaltracking")]
        public string InternalTracking { get; set; }
        [Column("provideradd")]
        public bool? ProviderAdd { get; set; }
        [Column("provideredit")]
        public bool? ProviderEdit { get; set; }

        [Column("providernumbers", TypeName = "text")]
        public string ProviderNumbers { get; set; }

        [StringLength(50), Column("origionaldramt")]
        public string OrigionalHotelRewards { get; set; }

        [StringLength(50), Column("origionalcondoamt")]
        public string OrigionalCondoRewards { get; set; }

        [StringLength(50), Column("origionalcruiseamt")]
        public string OrigionalCruiseRewards { get; set; }
        [Column("tickets")]
        public bool? Tickets { get; set; }

        [Column("blockedreason", TypeName = "text")]
        public string BlockedReason { get; set; }

        [StringLength(50), Column("fax1")]
        public string Fax1 { get; set; }

        [StringLength(50), Column("faxext1")]
        public string FaxExtension1 { get; set; }

        [StringLength(50), Column("fax2")]
        public string Fax2 { get; set; }

        [StringLength(50), Column("faxext2")]
        public string FaxExtension2 { get; set; }

        [StringLength(50), Column("signupip")]
        public string SignupIP { get; set; }

        [StringLength(50), Column("firstlogin")]
        public string FirstLogin { get; set; }
        [Column("referralid")]
        public int? ReferralId { get; set; }
        [Column("renewalmsgdate")]
        public DateTime? RenewalMessageDate { get; set; }
        [Column("renewalsoftconfirm")]
        public bool? RenewalSoftConfirm { get; set; }

        [StringLength(255), Column("renewalerror")]
        public string RenewalError { get; set; }
        [Column("renewalsoftconfirmdate")]
        public DateTime? RenewalSoftConfirmDate { get; set; }
        [Column("financingoption")]
        public int? FinancingOption { get; set; }
        [Column("optout")]
        public bool? OptOut { get; set; }
        [Column("ndr")]
        public bool? NDR { get; set; }
        [Column("iRenewalLength")]
        public int? RenewalLength { get; set; }
        [Column("iRenewalTeamKeyId")]
        public int? RenewalTeamKeyId { get; set; }
        [Column("bRenewalPriceEdit")]
        public bool? RenewalPriceEdit { get; set; }
        [Column("bRenewalSkipBilling")]
        public bool? RenewalSkipBilling { get; set; }
        [Column("dtBirthDate", TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }
        [Column("bMilitary")]
        public bool? Military { get; set; }
        [Column("dtBirthDate2", TypeName = "datetime")]
        public DateTime? BirthDate2 { get; set; }
        [Column("bMilitary2")]
        public bool? Military2 { get; set; }

        [StringLength(25), Column("phone3")]
        public string Phone3 { get; set; }
        [Column("bTransferOffered")]
        public bool TransferOffered { get; set; }
        [Column("iTransferOfferedId")]
        public int TransferOfferedId { get; set; }
        [Column("dtTransferOfferedDate")]
        public DateTime? TransferOfferedDate { get; set; }
        [Column("dtInactiveDate")]
        public DateTime? InactiveDate { get; set; }

        [StringLength(100), Column("passwordQuestion")]
        public string PasswordQuestion { get; set; }
       
        [StringLength(255), Column("passwordAnswer")]
        public string PasswordAnswer { get; set; }

        [StringLength(50), Column("iContactContactID")]
        public string ContactContactId { get; set; }

        [StringLength(50), Column("iContactSubscriptionID")]
        public string ContactSubscriptionId { get; set; }

        [StringLength(50), Column("iContactSendID")]
        public string ContactSendId { get; set; }
        [Column("mailMonkeyUploadDate")]
        public DateTime? MailMonkeyUploadDate { get; set; }
        [Column("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(100), Column("mailMonkeyCode")]
        public string MailMonkeyCode { get; set; }

        [StringLength(1000), Column("mailMonkeyMessage")]
        public string MailMonkeyMessage { get; set; }

        [StringLength(100), Column("drUserID")]
        public string DRUserId { get; set; }

        [StringLength(100), Column("outsideRenewalUserID")]
        public string OutsideRenewalUserId { get; set; }
        [Column("membershipLengthInDays")]
        public int? MembershipLengthInDays { get; set; }

        [Column("membershipRenewalAmount", TypeName = "money")]
        public decimal? MembershipRenewalAmount { get; set; }
        [Column("compedAccount")]
        public bool? IsCompedAccount { get; set; }

        [StringLength(100), Column("soapCreatorReference")]
        public string SoapCreatorReference { get; set; }

        [StringLength(100), Column("soapCreationLocation")]
        public string SoapCreationLocation { get; set; }
        [Column("packageIDFK")]
        public int? PackageId { get; set; }

        [StringLength(255), Column("address2")]
        public string Address2 { get; set; }
        [Column("expiry_override_date")]
        public DateTime? ExpiryOverrideDate { get; set; }
        [Column("kllId")]
        public int? KLLId { get; set; }
        [Column("deleteMe")]
        public bool? DeleteMe { get; set; }
        [Column("keepMe")]
        public bool? KeepMe { get; set; }

    }
}
