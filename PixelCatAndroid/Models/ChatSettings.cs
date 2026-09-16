using Android.Content;
using Android.Preferences;

namespace PixelCatAndroid.Models
{
    /// <summary>
    /// AI 聊天设置，使用 SharedPreferences 持久化
    /// </summary>
    public class ChatSettings
    {
        private const string PREFS_NAME = "PixelCatSettings";
        private const string KEY_API_URL = "api_url";
        private const string KEY_API_KEY = "api_key";
        private const string KEY_MODEL_NAME = "model_name";
        private const string KEY_SYSTEM_PROMPT = "system_prompt";

        /// <summary>OpenAI 兼容 API 地址</summary>
        public string ApiUrl { get; set; } = "https://api.openai.com/v1/chat/completions";

        /// <summary>API 密钥</summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>模型名称</summary>
        public string ModelName { get; set; } = "gpt-3.5-turbo";

        /// <summary>系统提示词</summary>
        public string SystemPrompt { get; set; } = "你是一只可爱的像素猫咪桌宠。用简短、可爱的语气回答问题，偶尔卖萌。";

        /// <summary>
        /// 从 SharedPreferences 加载设置
        /// </summary>
        public static ChatSettings Load(Context context)
        {
            var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
            if (prefs == null)
                return new ChatSettings();

            return new ChatSettings
            {
                ApiUrl = prefs.GetString(KEY_API_URL, "https://api.openai.com/v1/chat/completions") ?? "https://api.openai.com/v1/chat/completions",
                ApiKey = prefs.GetString(KEY_API_KEY, string.Empty) ?? string.Empty,
                ModelName = prefs.GetString(KEY_MODEL_NAME, "gpt-3.5-turbo") ?? "gpt-3.5-turbo",
                SystemPrompt = prefs.GetString(KEY_SYSTEM_PROMPT, "你是一只可爱的像素猫咪桌宠。用简短、可爱的语气回答问题，偶尔卖萌。") ?? "你是一只可爱的像素猫咪桌宠。用简短、可爱的语气回答问题，偶尔卖萌。"
            };
        }

        /// <summary>
        /// 保存设置到 SharedPreferences
        /// </summary>
        public void Save(Context context)
        {
            var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
            if (prefs == null) return;

            var editor = prefs.Edit();
            editor.PutString(KEY_API_URL, ApiUrl);
            editor.PutString(KEY_API_KEY, ApiKey);
            editor.PutString(KEY_MODEL_NAME, ModelName);
            editor.PutString(KEY_SYSTEM_PROMPT, SystemPrompt);
            editor.Apply();
        }
    }
}
