using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Password_Manager.Models;

namespace Password_Manager.Services;

public static class FileService
{
    public static async Task<IEnumerable<PasswordEntryModel>?> LoadEntriesAsync(string path)
    {
        try
        {
            await using var fs = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<IEnumerable<PasswordEntryModel>>(fs);
        }
        catch(Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
        {
            return null;
        }
    }
}