using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("unitamenities")]
    public class AmenitiesModel
    {
        [Key]
        public int keyid { get; set; }
        public int? unitkeyid { get; set; }
        [Column("ref")]
        public string ref1 { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public double? distance { get; set; }
        public string mk { get; set; }
        public int? sorder { get; set; }
        public DateTime? lastrciupdate { get; set; }
    }
}
