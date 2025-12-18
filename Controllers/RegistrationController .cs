using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Zuwarak.Data;
using Zuwarak.Models;
using System.IO;
using System.Linq;

namespace Zuwarak.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AppDbContext _db;

        public RegistrationController(AppDbContext db)
        {
            _db = db;
        }

        
        public IActionResult Index()
        {
            return View(new RegisterVisit());
        }

        // POST – SAVE VISITOR
        [HttpPost]
        public IActionResult Index(RegisterVisit model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string imagePath = null;

            // رفع الصورة
            if (model.IDImage != null && model.IDImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.IDImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.IDImage.CopyTo(fileStream);
                }

                imagePath = "/uploads/" + uniqueFileName;
            }

            var visitor = new Visitor
            {
                FullName = model.FullName,
                NationalId = model.NationalId,
                Phone = model.Phone,
                Email = model.Email,
                VisitPurpose = model.VisitPurpose,
                TargetPerson = model.TargetPerson,
                VisitDate = DateTime.Now,
                IDImagePath = imagePath
            };

            _db.visitors.Add(visitor);
            _db.SaveChanges();

            return RedirectToAction("QRCode", new { id = visitor.Id });
        }

        // QR CODE PAGE
        public IActionResult QRCode(int id)
        {
            var visitor = _db.visitors.FirstOrDefault(v => v.Id == id);
            if (visitor == null)
                return NotFound();

            string qrText = $"VisitorID:{visitor.Id}";

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(qrData))
            using (var bitmap = qrCode.GetGraphic(20))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                var qrBytes = ms.ToArray();
                string qrBase64 = Convert.ToBase64String(qrBytes);
                ViewBag.QRCodeImage = $"data:image/png;base64,{qrBase64}";
            }

            return View(visitor);
        }
    }
}
