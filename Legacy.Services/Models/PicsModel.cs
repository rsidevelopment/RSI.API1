using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    public class PicsModel
    {
        [Key]
        public int keyid { get; set; }
        [Column("pic")]
        public byte[] pic1 { get; set; }
        public string ptype { get; set; }
        public string ftype { get; set; }
        public string psize { get; set; }
        [Column("ref")]
        public string ref1 { get; set; }
        public int? id { get; set; }
        public bool? mainpic { get; set; }
        public int? recordkeyid { get; set; }
        public DateTime? lastrciupdate { get; set; }
    }
}
