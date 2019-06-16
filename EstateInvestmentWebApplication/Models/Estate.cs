using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models
{
    public class Estate
    {
        [Key]
        public int EstateId { get; set; }

        [Required]
        public string Title { get; set; }

        public string PicturePath { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

    }
}
