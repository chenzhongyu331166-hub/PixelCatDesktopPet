using System;
using System.IO;
using System.Text.Json;

namespace PixelCatDesktop.Services;

/// <summary>
/// JSON 文件持久化设置
/// </summary>
public class SettingsService
{
    private static readonly string SettingsDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PixelCatDesktop");

    private static readonly string SettingsFile =
        Path.Combine(SettingsDir, "settings.json");

    public string ProviderName { get; set; } = "DeepSeek";
    public string BaseUrl { get; set; } = "https://api.deepseek.com/v1";
    public string Model { get; set; } = "deepseek-chat";
    public string ApiKey { get; set; } = string.Empty;
    public float ScaleFactor { get; set; } = 1.0f;

    public void Load()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                var data = JsonSerializer.Deserialize<SettingsData>(json);
                if (data != null)
                {
                    ProviderName = data.ProviderName;
                    BaseUrl = data.BaseUrl;
                    Model = data.Model;
                    ApiKey = data.ApiKey;
                    ScaleFactor = data.ScaleFactor;
                }
            }
        }
        catch
        {
            // 损坏的配置不应阻止启动
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(SettingsDir);
            var data = new SettingsData
            {
                ProviderName = ProviderName,
                BaseUrl = BaseUrl,
                Model = Model,
                ApiKey = ApiKey,
                ScaleFactor = ScaleFactor
            };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }
        catch
        {
            // 保存失败不应崩溃
        }
    }

    private class SettingsData
    {
        public string ProviderName { get; set; } = "DeepSeek";
        public string BaseUrl { get; set; } = "https://api.deepseek.com/v1";
        public string Model { get; set; } = "deepseek-chat";
        public string ApiKey { get; set; } = string.Empty;
        public float ScaleFactor { get; set; } = 1.0f;
    }
}
