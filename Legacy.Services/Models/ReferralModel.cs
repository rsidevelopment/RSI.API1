using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("Referrals")]
    public class ReferralModel
    {
        private string _phone1 = "", _phone2 = "";

        [Key, Required]
        public int ReferralId { get; set; }
        [Required]
        public int RSIId { get; set; }
        [Required, StringLength(256)]
        public string FirstName { get; set; }
        [StringLength(256)]
        public string MiddleName { get; set; }
        [Required, StringLength(256)]
        public string LastName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Phone1
        {
            get { return _phone1; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _phone1 = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [StringLength(50)]
        public string Phone2
        {
            get { return _phone2; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    _phone2 = new String(value.Where(Char.IsDigit).ToArray());
                }
            }
        }
        [Required, Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [StringLength(100), Required]
        public string UpdateTimeStamp { get; set; }
        [StringLength(50), Required]
        public string Relationship { get; set; }
    }
}
