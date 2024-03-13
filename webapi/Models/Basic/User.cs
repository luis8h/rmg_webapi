namespace webapi.Models.Basic
{
    public class User
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Password { get; set; }
        public byte[]? PasswordHashed { get; set; }
        public byte[]? PasswordKey { get; set; }
        public string? Email { get; set; }
        public DateTime? SignupDate { get; set; }
    }
}
