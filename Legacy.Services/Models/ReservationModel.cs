using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Legacy.Services.Models
{
    [Table("reservations")]
    public class ReservationModel
    {
        [Key]
        public int keyid { get; set; }
        public int? billingkeyid { get; set; }
        public int? unitkeyid { get; set; }
        public int? creatorid { get; set; }
        public DateTime? creationdate { get; set; }
        public int? ownerid { get; set; }
        public string fname { get; set; }
        public string mi { get; set; }
        public string lname { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phone1 { get; set; }
        public string ext1 { get; set; }
        public string phone2 { get; set; }
        public string ext2 { get; set; }
        public string email { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public int? los { get; set; }
        public string size { get; set; }
        public string sleeps { get; set; }
        public string cost { get; set; }
        public string price { get; set; }
        public string rewardsamt { get; set; }
        public string overage { get; set; }
        public string discount { get; set; }
        public int? org { get; set; }
        public string origionalid { get; set; }
        public bool? hw { get; set; }
        public int? specialinventory { get; set; }
        public string transactionnumber { get; set; }
        public string inventorytype { get; set; }
        public string refnum { get; set; }
        public string inventorytbl { get; set; }
        public int? program { get; set; }
        [Column("checked")]
        public bool? checked1 { get; set; }
        public bool? error { get; set; }
        public string comments { get; set; }
        public int? tickets { get; set; }
        public bool? realtime { get; set; }
        public int? resell { get; set; }
        public decimal? customercostpernight { get; set; }
        public string retail { get; set; }
        public string dt1 { get; set; }
        public string dt2 { get; set; }
        public string dt3 { get; set; }
        public string InventoryTable { get; set; }
        public long ContactInfoKeyID { get; set; }
        public int agentID { get; set; }
        public string xmlSend { get; set; }
        public string xmlReceive { get; set; }
    }
}