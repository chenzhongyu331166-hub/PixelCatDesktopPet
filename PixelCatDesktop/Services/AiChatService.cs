using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PixelCatDesktop.Services;

/// <summary>
/// OpenAI 兼容的 AI 聊天服务，支持 DeepSeek、通义千问、月之暗面等
/// </summary>
public class AiChatService
{
    private readonly HttpClient _httpClient = new();
    private string _baseUrl = "https://api.deepseek.com/v1";
    private string _model = "deepseek-chat";
    private string _apiKey = string.Empty;

    public void Configure(string baseUrl, string model, string apiKey)
    {
        _baseUrl = baseUrl;
        _model = model;
        _apiKey = apiKey;
    }

    public async Task<string> SendMessageAsync(string userMessage)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return "请先在设置中配置 API Key。";
        }

        try
        {
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "你是一只可爱的像素黑猫，住在用户的桌面上。你会用简短、可爱、带点调皮的语气回复。每次回复不超过两句话。" },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 200,
                temperature = 0.8
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType!.MediaType = "application/json";

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.GetArrayLength() > 0)
            {
                var message = choices[0].GetProperty("message");
                if (message.TryGetProperty("content", out var contentEl))
                {
                    return contentEl.GetString() ?? "喵~";
                }
            }

            return "喵？";
        }
        catch (Exception ex)
        {
            return $"网络出错了：{ex.Message}";
        }
    }
}
