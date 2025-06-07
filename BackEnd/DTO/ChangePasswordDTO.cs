namespace BackEnd.DTO
{
    public class ChangePasswordDTO
    {
        public string UserName { get; set; }
        public string TemporaryPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}