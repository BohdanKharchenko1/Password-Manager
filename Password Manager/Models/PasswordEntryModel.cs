namespace Password_Manager.Models;

public class PasswordEntryModel
{
    public string? ServiceName { get; set; }
    public string? Username { get; set; }
    
    public string? Password { get; set; }
    
    public override string ToString()
    {
        return ServiceName + " " + Username;
    }
    
}