using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Legacy.Services.Models
{
    [Table("AccessDevelopmentMemberAudit")]
    public class AccessDevelopmentMemberAuditModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccessDevelopmentId { get; set; }
        [Required]
        public int RSIId { get; set; }
        [Required, Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Required, StringLength(50)]
        public string Status { get; set; }
        [Required]
        public int ValidMemberCount { get; set; } = 0;
        [Required]
        public int InvalidMemberCount { get; set; } = 0;
        [Required, Column(TypeName = "datetime")]
        public DateTime ImportedDate { get; set; }
        [Required, StringLength(2500)]
        public string ShowImportLink { get; set; }
        [Required, StringLength(2500)]
        public string ValidMembersCSVLink { get; set; }
        [StringLength(2500)]
        public string InvalidMembersCSVLink { get; set; } = null;
        [Required]
        public string Message { get; set; } = "";
        [Required]
        public bool IsSuccess { get; set; }
        [Required, StringLength(100)]
        public string ProgramId { get; set; } = "200938";


    }
}
