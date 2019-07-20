using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstateInvestmentWebApplication.Data;
using Microsoft.AspNetCore.Mvc;

namespace EstateInvestmentWebApplication.Controllers
{
    public class NumberUserAccessController : Controller
    {
        ApplicationDbContext _dbContext;
        public NumberUserAccessController(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        [Route("admin/thong-ke-truy-cap")]
        [HttpGet]
        public IActionResult NumberUserAccessChart()
        {
            return View();
        }

        [Route("admin/lay-danh-sach-thong-ke")]
        [HttpGet]
        public JsonResult GetListNumberUserAccess()
        {
            List<string> day = new List<string>();
            List<int> numberUser = new List<int>();

            var listNumberUserAccesses = _dbContext.NumberUserAccesses.OrderByDescending(x => x.Date).Take(7);
            foreach(var item in listNumberUserAccesses)
            {
                day.Insert(0, item.Date.ToString("d"));
                numberUser.Insert(0, item.UserNumber);
            }

            return Json(new { day = day, numberUser = numberUser });
             
        }


    }


}