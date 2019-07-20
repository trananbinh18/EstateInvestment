using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models.DatabaseEntitiesModel
{
    public class Consultation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string Content { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
