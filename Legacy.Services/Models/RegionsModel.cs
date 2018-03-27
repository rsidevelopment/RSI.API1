using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("RCI_Regions")]
    public class RegionsModel
    {
        [Key]
        public int keyid { get; set; }
        public string regioncode { get; set; }
        public string regiondescription { get; set; }
    }
}
