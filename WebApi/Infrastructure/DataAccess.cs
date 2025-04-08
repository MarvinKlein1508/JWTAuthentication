using Dapper;
using Microsoft.Data.SqlClient;
using WebApi.Models;

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

    public User? FindUserByEmail(string email)
    {
        string sql = "SELECT * FROM Users WHERE Email = @Email";
        var user = _connection.QueryFirstOrDefault<User>(sql, new { Email = email });
        return user;
    }
    public bool InsertRefreshToken(RefreshToken refreshToken, string email)
    {
        string sql = "INSERT INTO RefreshTokens (Token, CreatedDate, Expires, IsEnabled, Email) VALUES (@Token, @CreatedDate, @Expires, @IsEnabled, @Email)";
        var result = _connection.Execute(sql, new { Token = refreshToken.Token, CreatedDate = refreshToken.CreatedDate, Expires = refreshToken.Expires, IsEnabled = refreshToken.IsEnabled, Email = email });
        return result > 0;
    }

    public bool DisableUserTokenByEmail(string email)
    {
        string sql = "UPDATE RefreshTokens SET IsEnabled = 0 WHERE Email = @Email";
        var result = _connection.Execute(sql, new { Email = email });
        return result > 0;
    }   
    public bool DisableUserToken(string token)
    {
        string sql = "UPDATE RefreshTokens SET IsEnabled = 0 WHERE Token = @Token";
        var result = _connection.Execute(sql, new { Token = token });
        return result > 0;
    }

    public bool IsRefreshTokenValid(string token)
    {
        string sql = "SELECT COUNT(1) FROM RefreshTokens WHERE Token = @Token AND IsEnabled = 1 AND Expires >= CAST(GETDATE() AS DATE)";
        var result = _connection.ExecuteScalar<int>(sql, new { Token = token });
        return result > 0;
    }

    public User? FindUserByToken(string token)
    {
        string sql =
            """
            SELECT * FROM RefreshTokens RT
            JOIN Users U ON (U.Email = RT.Email)
            WHERE RT.Token = @Token
            """;
        
        var user = _connection.QueryFirstOrDefault<User>(sql, new { Token = token });

        return user;
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null!;
    }
}
