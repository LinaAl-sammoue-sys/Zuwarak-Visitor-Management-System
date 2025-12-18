using Microsoft.AspNetCore.Mvc;
using Zuwarak.Data;
using Zuwarak.Models;
using System.Linq;
using Zuwarak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.EntityFrameworkCore;



namespace Zuwarak.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
       
        public IActionResult Index()
        {
            return View();
        }

        // صفحة إضافة موظف
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1) إنشاء ApplicationUser
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            // 2) إنشاء المستخدم في Identity
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // 3) ربط المستخدم بالدور (Admin / Reception / Security)
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);

                // إذا فشل إضافة الدور، يمكن حذف المستخدم لتجنب البيانات غير المتناسقة
                await _userManager.DeleteAsync(user);

                return View(model);
            }

            // 4) إضافة بيانات الموظف في جدول Employees وربط IdentityUserId
            var employee = new Employee
            {
                FullName = model.FullName,
                Email = model.Email,
                UserId = user.Id,
                JobTitle = model.JobTitle,
                Department = model.Department,
                Phone = model.Phone,
                Role = model.Role

            };

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            return RedirectToAction("EmployeesList");
        }


        // عرض كل الموظفين
        public async Task<IActionResult> EmployeesList()
        {
            var employees = await _db.Employees.ToListAsync();
            return View(employees);
        }

        // تعديل بيانات موظف
        [HttpGet]
        public IActionResult EditEmployee(int id) {

            var emp = _db.Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound();
            return View(emp);


        }
        [HttpPost]
        public IActionResult EditEmployee(Employee model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            _db.Employees.Update(model);
            _db.SaveChanges();
            return RedirectToAction("EmployeesList");

     
        
        
        
        }
            //حذف موظف
            [HttpPost]
        public async Task< IActionResult> DeleteEmp(int id) {

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            //  حذف مستخدم Identity
            var user = await _userManager.FindByIdAsync(employee.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            //  حذف الموظف
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            return RedirectToAction("EmployeesList");

        }

        public IActionResult AllVisits(DateTime? fromDate, DateTime? toDate)
        {
            var visits = _db.visitors.AsQueryable();

            if (fromDate.HasValue)
                visits = visits.Where(v => v.VisitDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                visits = visits.Where(v => v.VisitDate.Date <= toDate.Value.Date);

            var list = visits
                        .OrderByDescending(v => v.VisitDate)
                        .ToList();

            return View(list);
        }

        public IActionResult Dashboard()
        {
            var today = DateTime.Today;

            var model = new DashboardViewModel
            {
                TodayVisits = _db.visitors
                    .Count(v => v.VisitDate.Date == today),

                MonthlyVisits = _db.visitors
                    .Count(v => v.VisitDate.Month == today.Month
                             && v.VisitDate.Year == today.Year),

                EmployeesCount = _db.Employees.Count(),

                LastVisits = _db.visitors
                    .OrderByDescending(v => v.VisitDate)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }


    }
}
