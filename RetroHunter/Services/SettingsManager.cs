

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RetroHunter.Services;

public class RetroHunterSettings
{
    #region  RA Config
    public string RaName { get; set; } = string.Empty;

    public string RaApiKey { get; set; } = string.Empty;
    #endregion

    #region Tools config
    public string ChdmanExePath { get; set; } = string.Empty;

    public string DolphinToolExePath { get; set; } = string.Empty;
    #endregion

    public string LatestDbPath { get; set; } = "";

}

public class SettingsManager(ILogger<SettingsManager>? logger)
{
    public RetroHunterSettings Settings = new();

    public async Task Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                await using var stream = File.OpenRead(SettingsFilePath);
                Settings = await JsonSerializer.DeserializeAsync<RetroHunterSettings>(stream) ?? new RetroHunterSettings();
            }
        }
        catch (Exception ex)
        {
            logger?.LogError($"Failed to read settings: {ex.Message}");
        }

    }

    public async Task Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            await using var stream = File.OpenWrite(SettingsFilePath);
            await JsonSerializer.SerializeAsync(stream, Settings, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            logger?.LogError($"Failed to write settings: {ex.Message}");
        }
    }

    private static string SettingsFilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RetroHunter", "settings.json");

}