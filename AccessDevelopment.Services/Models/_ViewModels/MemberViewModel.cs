using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AccessDevelopment.Services.Models._ViewModels
{
    public class MemberViewModel
    {
        private string _firstName = "", _middleName = null, _lastName = "", _address1 = null, _address2 = null, _city = null, _state = null, _postalCode = null, _country = null
            , _phone = null, _email = "", _memberStatus = "OPEN";
        [JsonProperty(PropertyName = "record_identifier", NullValueHandling = NullValueHandling.Ignore)]
        public string RecordIdentifier { get; set; }
        [JsonProperty(PropertyName = "organization_customer_identifier"), Required]
        public string OrganizationCustomerIdentifier { get; set; } = "2002598";
        [JsonProperty(PropertyName = "program_customer_identifier"), Required]
        public string ProgramCustomerIdentifier { get; set; } = "200938";
        [JsonProperty(PropertyName = "member_customer_identifier"), Required]
        public int RSIId { get; set; }
        [JsonProperty(PropertyName = "member_status"), Required]
        public string MemberStatus
        {
            get { return _memberStatus; }
            set
            {
                if (value != null)
                {
                    string tmp = value.ToUpper().Trim();
                    if (tmp == "OPEN" || tmp == "SUSPEND")
                        _memberStatus = tmp;
                }

            } //OPEN or SUSPEND
        }
        [JsonProperty(PropertyName = "first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value != null)
                    _firstName = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "middle_name", NullValueHandling = NullValueHandling.Ignore)]
        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                if (value != null)
                    _middleName = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value != null)
                    _lastName = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "full_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FullName
        {
            get
            {
                string name = FirstName;

                if (!String.IsNullOrEmpty(MiddleName))
                    name += $" {MiddleName}";
                name += $" {LastName}";

                return name;
            }
        }
        [JsonProperty(PropertyName = "street_line1", NullValueHandling = NullValueHandling.Ignore)]
        public string Address1
        {
            get { return _address1; }
            set
            {
                if (value != null)
                    _address1 = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "street_line2", NullValueHandling = NullValueHandling.Ignore)]
        public string Address2
        {
            get { return _address2; }
            set
            {
                if (value != null)
                    _address2 = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "city", NullValueHandling = NullValueHandling.Ignore)]
        public string City
        {
            get { return _city; }
            set
            {
                if (value != null)
                    _city = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public string State
        {
            get { return _state; }
            set
            {
                if(value != null)
                    _state = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "postal_code", NullValueHandling = NullValueHandling.Ignore)]
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                if (value != null)
                    _postalCode = value.Trim();
            }
        }
        [JsonProperty(PropertyName = "country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country
        {
            get { return _country; }
            set
            {
                if (value != null && value.Length > 1)
                    _country = value.Trim() == "USA" ? "US" : value.Trim().Substring(0, 2);
                else if (value != null)
                    _country = value.Trim();


            }
        }
        [JsonProperty(PropertyName = "phone_number", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (value != null && value.Length > 1)
                {
                    _phone = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [JsonProperty(PropertyName = "email_address", NullValueHandling = NullValueHandling.Ignore), EmailAddress]
        public string Email
        {
            get { return _email; }
            set
            {
                if (value != null)
                    _email = value.Trim();
            }
        }
        

        
    }
}
