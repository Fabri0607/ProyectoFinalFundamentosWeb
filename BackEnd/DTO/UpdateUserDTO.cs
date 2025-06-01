namespace BackEnd.DTO
{
    public class UpdateUserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; } // Optional, only set if changing password
    }
}