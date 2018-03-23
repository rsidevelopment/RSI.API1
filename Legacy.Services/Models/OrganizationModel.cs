using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("organizations")]
    public class OrganizationModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None), Column("keyid")]
        public int OrganizationId { get; set; }

        [Column("organization")]
        [StringLength(255)]
        public string OrganizationName { get; set; }

        [StringLength(500), Column("email")]
        public string Email { get; set; }

        [StringLength(100), Column("url")]
        public string URL { get; set; }
        [Column("exp")]
        public bool? Expires { get; set; }
        [Column("months")]
        public int? Months { get; set; }
        [Column("startdate")]
        public DateTime? Startdate { get; set; }
        [Column("billingid")]
        public int? BillingId { get; set; }

        [StringLength(10), Column("billingamt")]
        public string BillingAmount { get; set; }

        [StringLength(50), Column("markup")]
        public string Markup { get; set; }
        [Column("family")]
        public int? FamilyNumber { get; set; }
        [Column("resort")]
        public bool? HasResorts { get; set; }
        [Column("cruise")]
        public bool? HasCruises { get; set; }
        [Column("car")]
        public bool? HasCars { get; set; }
        [Column("air")]
        public bool? HasAir { get; set; }
        [Column("golf")]
        public bool? HasGolf { get; set; }
        [Column("recreation")]
        public bool? HasRecreation { get; set; }
        [Column("specialrequest")]
        public bool? HasSpecialRequest { get; set; }
        [Column("dr")]
        public bool? HasHotels { get; set; }
        [Column("superinclusive")]
        public bool? HasSuperInclusive { get; set; }
        [Column("travelservice")]
        public bool? HasTravelServices { get; set; }
        [Column("profile")]
        public bool? HasProfile { get; set; }
        [Column("hw")]
        public bool? HasHotweeks { get; set; }
        [Column("getaways")]
        public bool? HasGetaways { get; set; }
        [Column("showmenu")]
        public bool? ShowMenu { get; set; }
        [Column("DrCompany")]
        public int? DRCompany { get; set; }
        [Column("iDrProgramId")]
        public int? DRProgramId { get; set; }

        [StringLength(50), Column("strDrProgramType")]
        public string DRProgramType { get; set; }

        [StringLength(25), Column("phone")]
        public string Phone { get; set; }
        [Column("trial")]
        public bool? IsTrial { get; set; }
        [Column("trialperiod")]
        public int? TrialPeriod { get; set; }
        [Column("additionalmembers")]
        public bool? AdditionalMembers { get; set; }
        [Column("minivacs")]
        public bool? HasMinivacs { get; set; }
        [Column("member")]
        public bool? IsMember { get; set; }
        [Column("owner")]
        public bool? IsOwner { get; set; }
        [Column("checkuserentry")]
        public bool? CheckUserEntry { get; set; }
        [Column("rewardsbased")]
        public int? RewardsBased { get; set; }
        [Column("signupemail")]
        public bool? SignupEmail { get; set; }

        [StringLength(1), Column("enrolltype")]
        public string EnrollType { get; set; }
        [Column("additionalmemberstogether")]
        public bool? AdditionalMembersTogether { get; set; }

        [StringLength(50), Column("orgmarkup")]
        public string OrganizationMarkup { get; set; }
        [Column("addedvalue")]
        public bool? AddedValue { get; set; }
        [Column("unlimited")]
        public bool? Unlimited { get; set; }
        [Column("editlevel")]
        public int? EditLevel { get; set; }
        [Column("restrictview")]
        public bool? RestrictView { get; set; }

        [Column("restrictviewresorts", TypeName = "text")]
        public string RestrictViewResorts { get; set; }

        [Column("programs", TypeName = "text")]
        public string Programs { get; set; }
        [Column("showfilter")]
        public bool? ShowFilter { get; set; }
        [Column("onlinesignup")]
        public bool? OnlineSignup { get; set; }

        [Column("summary", TypeName = "text")]
        public string Summary { get; set; }
        [Column("oneuse")]
        public bool? OneUse { get; set; }

        [StringLength(50), Column("renewalamount")]
        public string RenewalAmount { get; set; }
        [Column("numdaysrenewalmsg")]
        public int? NumdaysRenewalMessage { get; set; }

        [Column("renewalletter", TypeName = "text")]
        public string RenewalLetter { get; set; }

        [StringLength(255), Column("renewalurl")]
        public string RenewalUrl { get; set; }

        [StringLength(100), Column("renewalreferralip")]
        public string RenewalReferralIP { get; set; }

        public bool? QnA { get; set; }
        [Column("guestsignup")]
        public bool? GuestSignup { get; set; }
        [Column("showpdf")]
        public bool? ShowPDF { get; set; }
        [Column("premiumcondos")]
        public bool? PremiumCondos { get; set; }
        [Column("numLaway")]
        public int? LawayNumber { get; set; }
        [Column("monthsLaway")]
        public int? MonthsLaway { get; set; }

        [StringLength(50), Column("premiumLaway")]
        public string PremiumLaway { get; set; }

        [StringLength(50)]
        public string DRViewingStatus { get; set; }

        [StringLength(50)]
        public string NumBookings { get; set; }

        [StringLength(100), Column("stylesheet")]
        public string stylesheet { get; set; }

        [StringLength(50)]
        public string AdminLevelGroup { get; set; }

        public bool? ShowStatusScreen { get; set; }

        [StringLength(50)]
        public string LawayayWell { get; set; }

        [StringLength(50), Column("strRegistrationFee")]
        public string RegistrationFee { get; set; }

        [StringLength(3), Column("strRegistrationCurrency")]
        public string RegistrationCurrency { get; set; }

        [StringLength(50), Column("strPremiumRegistrationFee")]
        public string PremiumRegistrationFee { get; set; }

        [StringLength(50), Column("strPremiumRegistrationCurrency")]
        public string PremiumRegistrationCurrency { get; set; }

        [StringLength(100), Column("strOrganizationSignupEmail")]
        public string OrganizationSignupEmail { get; set; }

        [StringLength(100), Column("strOrganizationReservationEmail")]
        public string OrganizationReservationEmail { get; set; }

        [StringLength(100), Column("strOrganizationSpecialRequestEmail")]
        public string OrganizationSpecialRequestEmail { get; set; }
        [Column("showOneRewards")]
        public bool ShowOneRewards { get; set; }

        [Column("minimumPrice", TypeName = "money")]
        public decimal MinimumPrice { get; set; }
        [Column("autoActivateDays")]
        public int AutoActivateDays { get; set; }
        [Column("cruiseRewardsBackAmount")]
        public double CruiseRewardsBackAmount { get; set; }

        [Column("maximumRSICost", TypeName = "money")]
        public decimal MaximumRSICost { get; set; }

        [Column("rsiCostToRemoveMarkup", TypeName = "money")]
        public decimal RSICostToRemoveMarkup { get; set; }

        [Column("retailFixedMarkup", TypeName = "money")]
        public decimal RetailFixedMarkup { get; set; }

        [Required]
        [StringLength(50), Column("restrictRealTime")]
        public string RestrictRealTime { get; set; }
        [Column("destination_rewards_api")]
        public bool DestinationRewardsApi { get; set; }
    }
}
