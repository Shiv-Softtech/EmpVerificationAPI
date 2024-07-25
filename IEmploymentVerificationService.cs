using EmpVerification;

public interface IEmploymentVerificationService
{
    bool VerifyEmployment(int employeeId, string companyName, string verificationCode, out bool verificationResult, int qtypes);
    Task InsertVerificationAsync(EmploymentVerificationRequest request);
}
