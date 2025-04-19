namespace Password_Manager.Models

{
    public class UserModel
    {
        public required string UserName { get; set; }
        public required string MasterPassword { get; init; } 
        public required string EntriesFilePath { get; init; }
    }
}
