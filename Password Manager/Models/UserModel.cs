namespace Password_Manager.Models

{
    public class UserModel
    {
        public required string UserName { get; set; }
        public required string MasterPassword { get; init; } // Keep this only in memory.
        public required string EntriesFilePath { get; init; }
    }
}
