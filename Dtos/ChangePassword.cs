namespace Learntendo_backend.Dtos
{
    public class ChangePassword
    {
        
            public int UserId { get; set; }
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        

    }
}
