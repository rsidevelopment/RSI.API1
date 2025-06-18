using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDevelopment.Services.Models._ViewModels
{
    public class ADCSVFileViewModel
    {
        public string RecordIdentifier { get; set; }
        public string RecordType { get; set; }
        public string OrganizationCustomerIdentifier { get; set; }
        public string ProgramCustomerIdentifier { get; set; }
        public string MemberCustomerIdentifier { get; set; }
        public string PreviousMemberCustomerIdentifier { get; set; }
        public string MemberStatus { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MembershipRenewalDate { get; set; }
        public string ProductIdentifier { get; set; }
        public string ProductTemplateField1 { get; set; }
        public string ProductTemplateField2 { get; set; }
        public string ProductTemplateField3 { get; set; }
        public string ProductTemplateField4 { get; set; }
        public string ProductTemplateField5 { get; set; }
        public string ProductRegistrationKey { get; set; }
        public string Message { get; set; } = "Not Implemented";
        public bool IsSuccess { get; set; } = false;
    }
}
