using Zuwarak.Models;

namespace Zuwarak.ViewModels
{
    public class DashboardViewModel
    {
        public int TodayVisits { get; set; }
        public int MonthlyVisits { get; set; }
        public int EmployeesCount { get; set; }

        public List<Visitor> LastVisits { get; set; }
    }
}
