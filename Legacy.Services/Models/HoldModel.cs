using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("holds")]
    public class HoldModel
    {
        [Key]
        public int keyid { get; set; }
        [ForeignKey("unit")]
        public int? unitkeyid { get; set; }
        [ForeignKey("inventory")]
        public int? inventorykeyid { get; set; }
        [ForeignKey("user")]
        public int? creatorid { get; set; }
        public DateTime? creationdate { get; set; }
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
        public string bfname { get; set; }
        public string bmi { get; set; }
        public string blname { get; set; }
        public string baddress { get; set; }
        public string bcity { get; set; }
        public string bstate { get; set; }
        public string bzip { get; set; }
        public string bcountry { get; set; }
        public string bphone1 { get; set; }
        public string bext1 { get; set; }
        public string bphone2 { get; set; }
        public string bext2 { get; set; }
        public string bemail { get; set; }
        public string card { get; set; }
        public string mm { get; set; }
        public string yyyy { get; set; }
        public string cvv { get; set; }
        public int? org { get; set; }
        public string inventorytype { get; set; }
        public string refnum { get; set; }
        public string origionalid { get; set; }
        public int? ownerid { get; set; }
        public string bedrooms { get; set; }
        public string bathrooms { get; set; }
        public string kitchentype { get; set; }
        public string cost { get; set; }
        public string price { get; set; }
        public string strCurrency { get; set; }
        public string strUSPrice { get; set; }
        public string strCustomerPrice { get; set; }
        public string rewardsamt { get; set; }
        public string overage { get; set; }
        public string discount { get; set; }
        public string rsicost { get; set; }
        public bool? hw { get; set; }
        public int? specialinventory { get; set; }
        public bool? error { get; set; }
        public string errordesc { get; set; }
        public bool? realtime { get; set; }
        public int? numnights { get; set; }
        public decimal? customercostpernight { get; set; }
        public string retail { get; set; }
        public string dt1 { get; set; }
        public string dt2 { get; set; }
        public string dt3 { get; set; }
        public string tickets { get; set; }
        public bool? complete { get; set; }
        public string InventoryTable { get; set; }
        public bool? bSentWarning { get; set; }
        public bool? bSentHoldEmail { get; set; }
        public string strHoldSendXml { get; set; }
        public string strHoldReceiveXml { get; set; }
        public string strBookSendXml { get; set; }
        public string strBookReceiveXml { get; set; }
        public double? fRSIPrice { get; set; }
        public string strRSICurrency { get; set; }
        public double? fCustomerPrice { get; set; }
        public string strCustomerCurrency { get; set; }
        public double? fResortPrice { get; set; }
        public string strResortCurrency { get; set; }
        public long? ContactInfoKeyID { get; set; }
        public int agentID { get; set; }
        public bool? isBooking { get; set; }
        public bool isBookingSentToCB { get; set; }
    }
}
