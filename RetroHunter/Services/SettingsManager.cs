

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RetroHunter.Services;

public class RetroHunterSettings
{
    public string LatestDbPath { get; set; } = "";
}

public class SettingsManager(ILogger<SettingsManager> logger)
{
    private static string SettingsFilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RetroHunter", "settings.json");

    public async Task<RetroHunterSettings> Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                await using var stream = File.OpenRead(SettingsFilePath);
                return (await JsonSerializer.DeserializeAsync<RetroHunterSettings>(stream)) ?? new RetroHunterSettings();
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to load settings: {ex.Message}");

        }

        return new RetroHunterSettings();
    }

    public async Task Save(RetroHunterSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            await using var stream = File.OpenWrite(SettingsFilePath);
            await JsonSerializer.SerializeAsync(stream, settings, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to write settings: {ex.Message}");
        }
    }
}