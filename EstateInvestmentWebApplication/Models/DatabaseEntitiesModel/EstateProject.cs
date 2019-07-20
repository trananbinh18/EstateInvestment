using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models.DatabaseEntitiesModel
{
    public class EstateProject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        [MaxLength(500, ErrorMessage = "Không được quá 500 ký tự")]
        public string ShortDescription { get; set; }

        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string Content { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Boolean Visible { get; set; }

        [ForeignKey("EstateCatalog")]
        public int CatalogId { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }

        public virtual EstateCatalog EstateCatalog { get;set; }

        public virtual IdentityUser IdentityUser { get; set; }
    }
}
