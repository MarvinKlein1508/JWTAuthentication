using Dapper;
using Microsoft.Data.SqlClient;

namespace WebApi.Infrastructure;

public class DataAccess : IDisposable
{
    private SqlConnection _connection;
    public DataAccess(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")!;
        _connection = new SqlConnection(connectionString);
        _connection.Open();
    }

    public bool RegisterUser(string email, string password, string role)
    {
        var accountCount = _connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Users WHERE Email = @Email", new { Email = email });

        if (accountCount > 0)
        {
            return false; // User already exists
        }

        string sql = "INSERT INTO Users (Email, Password, Role) VALUES (@Email, @Password, @Role)";
        var result = _connection.Execute(sql, new { Email = email, Password = password, Role = role });

        return result > 0;
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null!;
    }
}
