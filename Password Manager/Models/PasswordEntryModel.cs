using System;

namespace Password_Manager.Models;

public class PasswordEntryModel
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier

    public string? ServiceName { get; set; }
    public string? Username { get; set; }
    public string? EncryptedPassword { get; set; }

    public override string ToString()
    {
        return ServiceName + " " + Username;
    }
    
}