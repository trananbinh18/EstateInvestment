using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models.ViewModels
{
    public class CreateEstateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        [MaxLength(500, ErrorMessage = "Không được quá 500 ký tự")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public string Content { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Bắt buộc nhập trường này")]
        public int CatalogId { get; set; }

    }
}
