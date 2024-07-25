using EmpVerification;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

public class EmploymentVerificationService : IEmploymentVerificationService
{
    private readonly string _connectionString;

    public EmploymentVerificationService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("YourConnectionString");
    }

    public bool VerifyEmployment(int employeeId, string companyName, string verificationCode, out bool verificationResult, int qtypes)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("VerifyEmployment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    command.Parameters.AddWithValue("@Qtypes", 0);
                    command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                    command.Parameters.Add("@CompanyName", SqlDbType.VarChar, 255).Value = companyName;
                    command.Parameters.Add("@VerificationCode", SqlDbType.VarChar, 255).Value = verificationCode;
                    command.Parameters.Add("@VerificationResult", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@Qtypes", SqlDbType.Int).Value = qtypes;

                    command.ExecuteNonQuery();

                    // Retrieve output parameter
                    verificationResult = (bool)command.Parameters["@VerificationResult"].Value;

                    return verificationResult;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (logging, etc.)
            throw new ApplicationException("Error verifying employment", ex);
        }
    }
    public async Task InsertVerificationAsync(EmploymentVerificationRequest request)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string sql = @"INSERT INTO employment_verifications (employee_id, company_name, verification_code)
                           VALUES (@EmployeeId, @CompanyName, @VerificationCode)";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@employeeId", request.EmployeeId);
            command.Parameters.AddWithValue("@companyName", request.CompanyName);
            command.Parameters.AddWithValue("@verificationCode", request.VerificationCode);

            await command.ExecuteNonQueryAsync();
        }
    }
}
