using System;

namespace Password_Manager.Models;

public class PasswordEntryModel
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier

    public required string ServiceName { get; set; }
    public string? Username { get; set; }
    public required string EncryptedPassword { get; set; }

    public override string ToString()
    {
        return ServiceName + " " + Username;
    }
    
}