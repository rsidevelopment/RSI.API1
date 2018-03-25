using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("units")]
    public class UnitModel
    {
        [Key]
        public int keyid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string contactname { get; set; }
        public string contactphone { get; set; }
        public string email { get; set; }
        public string checkin { get; set; }
        public string checkout { get; set; }
        public string rating { get; set; }
        public string origionalid { get; set; }
        public int? creatorid { get; set; }
        public int? ownerid { get; set; }
        public string url { get; set; }
        public string frontinfo { get; set; }
        public string info { get; set; }
        public string airport { get; set; }
        public double? distance { get; set; }
        public string mk { get; set; }
        public string currency { get; set; }
        public string regioncode { get; set; }
        public int? unittype { get; set; }
        public bool? restrictview { get; set; }
        public string inclusivetype { get; set; }
        public bool? realtime { get; set; }
        public string AdminLevelGroup { get; set; }
        public bool? active { get; set; }
        public DateTime? lastrciupdate { get; set; }
        public bool? bCallForAvailability { get; set; }
        public string catagory { get; set; }
        public int zone { get; set; }
        public int thumbID { get; set; }
        public string rsiCatagory { get; set; }
    }
}
