using System.Text.Json;

namespace PixelCatAndroid.Models
{
    /// <summary>
    /// 聊天消息模型（用于 OpenAI API 请求/响应）
    /// </summary>
    public class ChatMessage
    {
        [System.Text.Json.Serialization.JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        [System.Text.Json.Serialization.JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// OpenAI Chat Completion 请求体
    /// </summary>
    public class ChatCompletionRequest
    {
        [System.Text.Json.Serialization.JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();

        [System.Text.Json.Serialization.JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 150;

        [System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.8;
    }

    /// <summary>
    /// OpenAI Chat Completion 响应体
    /// </summary>
    public class ChatCompletionResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("choices")]
        public List<ChatChoice> Choices { get; set; } = new();
    }

    /// <summary>
    /// 响应中的单个选择
    /// </summary>
    public class ChatChoice
    {
        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }
    }
}
