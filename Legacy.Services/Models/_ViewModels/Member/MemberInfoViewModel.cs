using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Legacy.Services.Models._ViewModels.Member
{
    
    public class MemberInfoViewModel : _ItemViewModel
    {
        [JsonProperty(PropertyName = "primary_member")]
        public PersonViewModel PrimaryMember { get; set; } = new PersonViewModel();
        [JsonProperty(PropertyName = "secondary_member")]
        public PersonViewModel SecondaryMember { get; set; } = new PersonViewModel();
        [JsonProperty(PropertyName = "organization_info")]
        public OrganizationInfoViewModel OrganizationInfo { get; set; } = new OrganizationInfoViewModel();
        [JsonProperty(PropertyName = "package_info")]
        public PackageInfoViewModel PackageInfo { get; set; } = new PackageInfoViewModel();
        [JsonProperty(PropertyName = "membership_info")]
        public MembershipInfoViewModel MembershipInfo { get; set; } = new MembershipInfoViewModel();
        [JsonProperty(PropertyName = "family_members")]
        public List<FamilyMemberViewModel> FamilyMembers { get; set; } = new List<FamilyMemberViewModel>();
        [JsonIgnore]
        public string FamilyMemberString
        {
            get
            {
                string fms = string.Empty;
                for (int i = 0; i < FamilyMembers.Count; i++)
                {
                    var p = FamilyMembers[i];
                    if (i > 0) fms += ",";
                    fms += $"{p.FirstName} {p.LastName}";
                }

                return fms;
            }
        }
        [JsonProperty(PropertyName = "travel_info")]
        public List<TravelDetailViewModel> TravelInfo { get; set; } = new List<TravelDetailViewModel>();
        
    }
    public class ReferralViewModel
    {
        private string _homePhone = "", _mobilePhone = "";
        [JsonProperty(PropertyName = "referral_id")]
        public int ReferralId { get; set; } = 0;
        
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; } = "";
        [JsonProperty(PropertyName = "middle_name")]
        public string MiddleName { get; set; } = "";
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; } = "";
        [JsonProperty(PropertyName = "relationship")]
        public string Relationship { get; set; }
        [JsonProperty(PropertyName = "date_of_birth")]
        public DateTime? DateOfBirth { get; set; } = null;
        [JsonProperty(PropertyName = "email"), EmailAddress]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "mobile_phone")]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _mobilePhone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "home_phone")]
        public string HomePhone
        {
            get { return _homePhone; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _homePhone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
    }
    public class PersonViewModel
    {
        private string _mobilePhone = "", _homePhone = "";

        [JsonProperty(PropertyName = "member_id"), Required]
        public int RSIId { get; set; }
        [JsonProperty(PropertyName = "first_name"), StringLength(100), Required]
        public string FirstName { get; set; } = "";
        [JsonProperty(PropertyName = "middle_name"), StringLength(100)]
        public string MiddleName { get; set; } = "";
        [JsonIgnore]
        public string MiddleInitial
        {
            get
            {
                return (string.IsNullOrEmpty(MiddleName)) ? null : MiddleName.Substring(0,1);
            }
        }
        [JsonProperty(PropertyName = "last_name"), StringLength(255), Required]
        public string LastName { get; set; } = "";
        [JsonProperty(PropertyName = "address_1"), Required, StringLength(255)]
        public string Address1 { get; set; } = "";
        [JsonProperty(PropertyName = "address_2"), StringLength(255)]
        public string Address2 { get; set; } = "";
        [JsonProperty(PropertyName = "city"), StringLength(100), Required]
        public string City { get; set; } = "";
        [JsonProperty(PropertyName = "state"), StringLength(50), Required]
        public string State { get; set; } = "";
        [JsonProperty(PropertyName = "postal_code"), StringLength(50)]
        public string PostalCode { get; set; } = "";
        [JsonProperty(PropertyName = "country"), StringLength(50), Required]
        public string Country { get; set; } = "";
        [JsonProperty(PropertyName = "mobile_phone"), StringLength(50)]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _mobilePhone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "home_phone"), StringLength(50)]
        public string HomePhone
        {
            get { return _homePhone; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _homePhone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "email"), EmailAddress, Required, StringLength(100)]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "date_of_birth", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateOfBirth { get; set; } = null;
    }

    public class OrganizationInfoViewModel
    {
        [JsonProperty(PropertyName = "organization_id")]
        public int OrganizationId { get; set; }
        [JsonProperty(PropertyName = "organization_name")]
        public string OrganizationName { get; set; }
    }

    public class PackageInfoViewModel
    {
        [JsonProperty(PropertyName = "package_id")]
        public int PackageId { get; set; }
        [JsonProperty(PropertyName = "points")]
        public float Points { get; set; }
        [JsonProperty(PropertyName = "package_name")]
        public string PackageName { get; set; }
        [JsonProperty(PropertyName = "benefits")]
        public BenefitsInfoViewModel Benefits { get; set; } = new BenefitsInfoViewModel();
    }

    public class BenefitsInfoViewModel
    {
        [JsonProperty(PropertyName = "has_hotels")]
        public bool HasHotels { get; set; } = false;
        [JsonProperty(PropertyName = "has_condos")]
        public bool HasCondos { get; set; } = false;
        [JsonProperty(PropertyName = "has_brio_resorts")]
        public bool HasBrioResorts { get; set; } = false;
        [JsonProperty(PropertyName = "has_cruise_getaways")]
        public bool HasCruiseGetaways { get; set; } = false;
        [JsonProperty(PropertyName = "has_fantasy_getaways")]
        public bool HasFantasyGetaways { get; set; } = false;
        [JsonProperty(PropertyName = "has_staycation_getaways")]
        public bool HasStaycationGetaways { get; set; } = false;
        [JsonProperty(PropertyName = "has_condo_getaways")]
        public bool HasCondoGetaways { get; set; } = false;
        [JsonProperty(PropertyName = "has_leisure_hub")]
        public bool HasLeisureHub { get; set; } = false;
        [JsonProperty(PropertyName = "has_flights")]
        public bool HasFlights { get; set; } = false;
        [JsonProperty(PropertyName = "has_cars")]
        public bool HasCars { get; set; } = false;
    }

    public class MembershipInfoViewModel
    {
        [JsonProperty(PropertyName = "date_joined")]
        public DateTime ActivationDate { get; set; }
        [JsonProperty(PropertyName = "years_as_member")]
        public int YearsAsMember { get; set; }
        [JsonProperty(PropertyName = "condo_weeks")]
        public int CondoWeeks { get; set; } = 3;
        [JsonProperty(PropertyName = "dues_amount")]
        public string DuesAmount { get; set; }
        [JsonProperty(PropertyName = "last_dues_payment")]
        public DateTime? LastDuesPayment { get; set; }
    }

    public class FamilyMemberViewModel
    {
        private string _familyPrimaryPhone = "", _familyAltPhone = "";

        [JsonProperty(PropertyName = "family_member_id"), Required]
        public long FamilyMemberId { get; set; }
        [JsonProperty(PropertyName = "first_name"), StringLength(50), Required]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "middle_name"), StringLength(50)]
        public string MiddleName { get; set; } = "";
        [JsonProperty(PropertyName = "last_name"), StringLength(50), Required]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "email"), EmailAddress, StringLength(100)]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "relationship"), StringLength(50)]
        public string Relationship { get; set; }
        [JsonProperty(PropertyName = "date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; } = 0;
        [JsonProperty(PropertyName = "primary_phone"), StringLength(50)]
        public string PrimaryPhone
        {
            get { return _familyPrimaryPhone; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _familyPrimaryPhone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "primary_phone_type"), StringLength(25)]
        public string PrimaryPhoneType { get; set; }
        [JsonProperty(PropertyName = "alternative_phone"), StringLength(50)]
        public string AlternativePhone
        {
            get { return _familyAltPhone; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _familyAltPhone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "alternative_phone_type"), StringLength(25)]
        public string AlternativePhoneType { get; set; }
    }

    public class TravelDetailViewModel
    {
        [JsonProperty(PropertyName = "booking_reference")]
        public string BookingReference { get; set; }
        [JsonProperty(PropertyName = "date_booked")]
        public DateTime DateBooked { get; set; }
        [JsonProperty(PropertyName = "trip_type")]
        public string TripType { get; set; }
        [JsonProperty(PropertyName = "trip_category")]
        public string TripCategory { get; set; }
        [JsonProperty(PropertyName = "accommodation_name")]
        public string AccommodationName { get; set; }
        [JsonProperty(PropertyName = "accommodation_description")]
        public string AccommodationDescription { get; set; }
        [JsonProperty(PropertyName = "arrival_date")]
        public DateTime ArrivalDate { get; set; }
        [JsonProperty(PropertyName = "departure_date")]
        public DateTime DepartureDate { get; set; }
    }
}
