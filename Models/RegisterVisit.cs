namespace Zuwarak.Models
{
    public class RegisterVisit
    {
        
        public string? FullName { get; set; }
        public string? NationalId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? VisitPurpose { get; set; }
        public string? TargetPerson { get; set; } 
        public DateTime VisitDate { get; set; } = DateTime.Now;
        public IFormFile IDImage { get; set; }

    }
}   
