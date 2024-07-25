namespace EmpVerification
{
    public class EmploymentVerificationRequest
    {
        public int EmployeeId { get; set; }
        public string CompanyName { get; set; }
        public string VerificationCode { get; set; }
        public int Qtypes { get; set; }
    }
}
