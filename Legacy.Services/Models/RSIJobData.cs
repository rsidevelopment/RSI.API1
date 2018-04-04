using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Legacy.Services.Models
{
    public class RSIJobData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int jobId { get; set; }
        public string data { get; set; }
        public int RSIId { get; set; }
        public string info { get; set; }
    }
}
