using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("BrioClubLeads")]
    public class BrioClubLeadsModel
    {
        [Key]
        public int id { get; set; }
        public int rsiId { get; set; }
        public string clubReference { get; set; }
    }
}
