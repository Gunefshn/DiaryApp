namespace DiaryApp;

public class User
{
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; } 
    public string SecurityQuestion { get; set; } 
    public string SecurityAnswer { get; set; } 
    public DateTime CreatedAt { get; set; }

    public string FileName => $"{Username}_records.txt";
}