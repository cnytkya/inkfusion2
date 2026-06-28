namespace inkfusion.MVC.Utilities
{
    /// <summary>
    /// Utility class for password hashing and verification using BCrypt
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>The hashed password</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hash">The hashed password to compare against</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
