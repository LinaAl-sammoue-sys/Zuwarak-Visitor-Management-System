using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zuwarak.Data;
using Zuwarak.Models;

namespace Zuwarak.Controllers
{
    [Authorize(Roles = "Security")]
    public class SecurityController : Controller
    {
        private readonly AppDbContext _db;
        public SecurityController(AppDbContext db)
        {
            _db = db;
        }

       
        // GET: /Security/Scan
        public IActionResult Scan()
        {
            return View();
        }

        // GET: /Security/GetVisitor?id=123
        [HttpGet]
        public async Task<JsonResult> GetVisitor(int id)
        {
            var v = await _db.visitors
                .Where(x => x.Id == id)
                .Select(x => new {
                    id = x.Id,
                    fullName = x.FullName,
                    visitDate = x.VisitDate,
                    visitPurpose = x.VisitPurpose,
                    isArrived = x.IsArrived,
                    isCheckout = x.IsCheckout
                }).FirstOrDefaultAsync();

            if (v == null)
                return Json(new { success = false, message = "Visitor not found" });

            return Json(new { success = true, data = v });
        }

        // POST: /Security/ConfirmArrival?id=123
        [HttpPost]
        public async Task<JsonResult> ConfirmArrival(int id)
        {
            var v = await _db.visitors.FindAsync(id);
            if (v == null) return Json(new { success = false, message = "Not found" });

            v.IsArrived = true;
            // ممكن تخزن وقت الوصول:
            v.ArrivalTime = DateTime.Now;

            _db.visitors.Update(v);
            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: /Security/ConfirmCheckout?id=123
        [HttpPost]
        public async Task<JsonResult> ConfirmCheckout(int id)
        {
            var v = await _db.visitors.FindAsync(id);
            if (v == null) return Json(new { success = false, message = "Not found" });

            // تأكد أن الزائر قد دخل قبل ما تسجّل الخروج
            if (!v.IsArrived) return Json(new { success = false, message = "Visitor hasn't arrived yet" });

            v.IsCheckout = true;
            v.CheckoutTime = DateTime.Now;

            _db.visitors.Update(v);
            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
