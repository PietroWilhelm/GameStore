namespace GameStore.Domain.Helpers;

public static class HashHelper
{
    public static string Hash(string newPassword, string salt)
    {
        // Append our generated salt value to the password and let BCrypt generate its own internal salt
        return BCrypt.Net.BCrypt.HashPassword(newPassword + salt);
    }
    
    public static bool Verify(string rawPassword, string salt, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(rawPassword + salt, hashedPassword);
    }
}