namespace UsersAndTasksAPI.Models.Responses
{
    /// <summary>
    /// Represents the response returned after successful authentication or registration
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT token for authenticated requests
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int ExpiresIn { get; set; }
        
        /// <summary>
        /// Type of the token (always "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
        
        /// <summary>
        /// Username of the authenticated user
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// User ID of the authenticated user
        /// </summary>
        public int UserId { get; set; }
    }
}
