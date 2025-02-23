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

    public static async Task<bool> SaveEntryAsync(PasswordEntryModel? entry , string? path)
    {
        if (string.IsNullOrEmpty(path) || entry == null)
        {
            return false;
        }

        List<PasswordEntryModel> entries = new();

        try
        {
            if (File.Exists(path))
            {
                await using var fs = File.OpenRead(path);
                var existing_entries = await JsonSerializer.DeserializeAsync<List<PasswordEntryModel>>(fs);
                if (existing_entries != null)
                    entries = existing_entries;
            }

            entries.Add(entry);

            await using var writefs = File.Create(path);
            await JsonSerializer.SerializeAsync(writefs, entries);
            return true;


        }
        catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
        {
            return false;
        }
    }
}