using EmpVerification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class EmploymentVerificationController : ControllerBase
{
    private readonly string _connectionString;

    public EmploymentVerificationController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("YourConnectionString");
    }

    [HttpPost]
    public IActionResult VerifyEmployment([FromBody] EmploymentVerificationRequest request)
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
                    command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = request.EmployeeId;
                    command.Parameters.Add("@CompanyName", SqlDbType.VarChar, 255).Value = request.CompanyName;
                    command.Parameters.Add("@VerificationCode", SqlDbType.VarChar, 255).Value = request.VerificationCode;
                    command.Parameters.Add("@VerificationResult", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@Qtypes", SqlDbType.Int).Value = request.Qtypes;

                    command.ExecuteNonQuery();

                    // Retrieve output parameter
                    bool verificationResult = (bool)command.Parameters["@VerificationResult"].Value;

                    return Ok(new { VerificationResult = verificationResult });
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpPost("insert")]
    public IActionResult InsertVerification([FromBody] EmploymentVerificationRequest request)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = @"INSERT INTO employment_verifications (employee_id, company_name, verification_code)
                               VALUES (@EmployeeId, @CompanyName, @VerificationCode)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Parameters
                    command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = request.EmployeeId;
                    command.Parameters.Add("@CompanyName", SqlDbType.VarChar, 255).Value = request.CompanyName;
                    command.Parameters.Add("@VerificationCode", SqlDbType.VarChar, 255).Value = request.VerificationCode;

                    command.ExecuteNonQuery();

                    return Ok("Verification inserted successfully");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
