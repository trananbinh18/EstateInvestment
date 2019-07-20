using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace EstateInvestmentWebApplication.Controllers
{
    public class ConsultationController : Controller
    {
        ApplicationDbContext _dbContext;
        public ConsultationController(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        [HttpPost]
        public JsonResult CreateConsultation(Consultation model)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Consultations.Add(model);
                _dbContext.SaveChanges();

                return Json(new { status = "success", message = "Thông điệp của bạn đã được chuyển đi thành công. Xin cảm ơn." });
            }

            return Json(new { status = "fail", message = "Thông tin không đủ hoặc không hợp lệ" });
        }


        [HttpGet]
        [Route("admin/thong-tin-tu-van/{page?}")]
        public async Task<IActionResult> ListConsultation(int page = 1)
        {
            var listConsultation = _dbContext.Consultations.OrderByDescending(x => x.CreateDate);

            var model = await PagingList.CreateAsync(listConsultation,10,page);
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteConsultation([FromForm]int id)
        {
            var consultation = _dbContext.Consultations.Find(id);
            _dbContext.Remove(consultation);
            _dbContext.SaveChanges();

            return RedirectToAction("ListConsultation");
        }
            
    }

}