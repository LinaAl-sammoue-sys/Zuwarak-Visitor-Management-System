using Microsoft.AspNetCore.Mvc;

using System.Linq;
using Zuwarak.Models;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Zuwarak.Data;

namespace Zuwarak.Controllers
{
    [Authorize(Roles = "Reception")]
    public class ReceptionController : Controller
    {
        private readonly AppDbContext _db;

        public ReceptionController(AppDbContext db)
        {
            _db = db;
        }

        
        // Dashboard – عرض زوار اليوم
        public IActionResult Index(string search)
        {
            var today = DateTime.Today;

            var visitors = _db.visitors
                .Where(v => v.VisitDate.Date == today)
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                visitors = visitors
                    .Where(v => v.FullName.Contains(search) || v.NationalId.Contains(search))
                    .ToList();
            }

            return View(visitors);
        }

        // Confirm Arrival  
        public IActionResult ConfirmArrival(int id)
        {
            var visitor = _db.visitors.FirstOrDefault(v => v.Id == id);
            if (visitor == null) return NotFound();

            visitor.ArrivalTime = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Checkout – تسجيل الخروج  
        public IActionResult Checkout(int id)
        {
            var visitor = _db.visitors.FirstOrDefault(v => v.Id == id);
            if (visitor == null) return NotFound();

            visitor.CheckoutTime = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // توليد QR Code للداشبورد
        public string GenerateQR(string text)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(qrData))
            using (var bitmap = qrCode.GetGraphic(20))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                var bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
