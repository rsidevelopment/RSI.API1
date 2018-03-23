using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("translator")]
    public class TranslatorModel
    {
        [StringLength(50), Required, Column("language")]
        public string Language { get; set; }
        [StringLength(50), Required, Column("type")]
        public string Type { get; set; }
        [StringLength(100), Required, Column("ref")]
        public string Reference { get; set; }
        [Column("v")]
        public string Value { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("keyid")]
        public int Id { get; set; }
        [Column("xinfo")]
        public string XtraInfo { get; set; } = "";
        [Column("allowadd")]
        public bool AllowAdd { get; set; }
        [Column("allowdelete")]
        public bool AllowDelete { get; set; }

    }
}
