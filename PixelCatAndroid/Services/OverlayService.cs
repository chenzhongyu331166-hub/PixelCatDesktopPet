using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Service.Notification;
using Android.Views;
using Android.Widget;
using PixelCatAndroid.Models;
using PixelCatAndroid.Views;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PixelCatAndroid.Services
{
    /// <summary>
    /// 前台服务 - 管理悬浮窗覆盖层
    /// 负责创建和维护桌面宠物的悬浮窗口
    /// </summary>
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeSpecialUse,
        Exported = false)]
    public class OverlayService : Service
    {
        private const string NOTIFICATION_CHANNEL_ID = "pixelcat_overlay";
        private const int NOTIFICATION_ID = 1001;

        private WindowManager? _windowManager;
        private SpriteView? _spriteView;
        private WindowManagerLayoutParams? _layoutParams;
        private HttpClient? _httpClient;
        private ChatSettings? _chatSettings;

        // 触摸状态
        private float _touchStartX;
        private float _touchStartY;
        private float _viewStartX;
        private float _viewStartY;
        private bool _isDragging;
        private long _touchStartTime;
        private const int TAP_THRESHOLD = 200; // 点击判定阈值（毫秒）
        private const int TAP_DISTANCE = 30;   // 点击判定距离（像素）

        // 对话历史
        private readonly List<ChatMessage> _chatHistory = new();
        private bool _isProcessingChat;

        public override void OnCreate()
        {
            base.OnCreate();
            _httpClient = new HttpClient();
            _chatSettings = ChatSettings.Load(this);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            CreateNotificationChannel();
            StartForeground(NOTIFICATION_ID, BuildNotification());

            // 初始化悬浮窗
            SetupOverlay();

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// 创建通知渠道（Android 8.0+）
        /// </summary>
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    NOTIFICATION_CHANNEL_ID,
                    "桌宠悬浮服务",
                    NotificationImportance.Low)
                {
                    Description = "桌面猫咪悬浮窗服务通知"
                };

                var manager = GetSystemService(NotificationService) as NotificationManager;
                manager?.CreateNotificationChannel(channel);
            }
        }

        /// <summary>
        /// 构建前台服务通知
        /// </summary>
        private Notification BuildNotification()
        {
            // 点击通知打开主界面
            var pendingIntent = PendingIntent.GetActivity(
                this, 0,
                new Intent(this, typeof(MainActivity)),
                PendingIntentFlags.Immutable);

            var builder = new Notification.Builder(this, NOTIFICATION_CHANNEL_ID)
                .SetContentTitle("像素猫咪桌宠")
                .SetContentText("桌宠正在运行中~ 点击打开设置")
                .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                .SetContentIntent(pendingIntent)
                .SetOngoing(true);

            return builder.Build()!;
        }

        /// <summary>
        /// 初始化悬浮窗
        /// </summary>
        private void SetupOverlay()
        {
            _windowManager = GetSystemService(WindowService) as IWindowManager;
            if (_windowManager == null) return;

            // 创建 SpriteView
            _spriteView = new SpriteView(this);

            // 配置悬浮窗参数
            _layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent,
                Build.VERSION.SdkInt >= BuildVersionCodes.O
                    ? WindowManagerTypes.ApplicationOverlay
                    : WindowManagerTypes.Phone,
                // 不拦截底层窗口的触摸事件，但需要接收自己的触摸
                WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutInScreen,
                Format.Translucent)
            {
                Gravity = GravityFlags.Left | GravityFlags.Top
            };

            // 初始位置（屏幕底部中央）
            var displayMetrics = Resources?.DisplayMetrics;
            if (displayMetrics != null)
            {
                _layoutParams.X = displayMetrics.WidthPixels / 2 - 64;
                _layoutParams.Y = displayMetrics.HeightPixels / 2 - 64;
                _spriteView.CatX = 0;
                _spriteView.CatY = 0;
            }

            // 设置触摸监听
            _spriteView.SetOnTouchListener(new OverlayTouchListener(this));

            // 添加到窗口
            _windowManager?.AddView(_spriteView, _layoutParams);
        }

        /// <summary>
        /// 处理猫咪点击事件 - 触发 AI 聊天
        /// </summary>
        public void OnCatTapped()
        {
            if (_isProcessingChat) return;

            // 显示思考状态
            _spriteView?.ShowBubble("让我想想...");

            // 随机打招呼或回复
            var greetings = new[]
            {
                "喵~你好呀！",
                "今天天气真好喵~",
                "要和我聊天吗？",
                "摸摸我的头喵~",
                "我有点饿了喵...",
                "一起玩吧！",
                "你好呀，主人~"
            };

            if (string.IsNullOrEmpty(_chatSettings?.ApiKey))
            {
                // 没有配置 API，使用预设回复
                var random = new Random();
                _spriteView?.ShowBubble(greetings[random.Next(greetings.Length)]);
                return;
            }

            // 使用 AI API 进行对话
            _ = ChatWithAI("你好！");
        }

        /// <summary>
        /// 调用 OpenAI 兼容 API 进行对话
        /// </summary>
        private async System.Threading.Tasks.Task ChatWithAI(string userMessage)
        {
            if (_httpClient == null || _chatSettings == null || string.IsNullOrEmpty(_chatSettings.ApiKey))
                return;

            _isProcessingChat = true;

            try
            {
                // 添加用户消息到历史
                _chatHistory.Add(new ChatMessage { Role = "user", Content = userMessage });

                // 构建请求
                var request = new ChatCompletionRequest
                {
                    Model = _chatSettings.ModelName,
                    Messages = new List<ChatMessage>
                    {
                        new() { Role = "system", Content = _chatSettings.SystemPrompt }
                    },
                    MaxTokens = 100,
                    Temperature = 0.8
                };

                // 添加对话历史（保留最近 10 条）
                var recentHistory = _chatHistory.TakeLast(10).ToList();
                request.Messages.AddRange(recentHistory);

                // 发送请求
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_chatSettings.ApiKey}");

                var response = await _httpClient.PostAsync(_chatSettings.ApiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var completion = JsonSerializer.Deserialize<ChatCompletionResponse>(responseBody);
                    var reply = completion?.Choices?.FirstOrDefault()?.Message?.Content ?? "喵...我有点累了";

                    // 显示回复
                    _spriteView?.ShowBubble(reply);

                    // 记录到历史
                    _chatHistory.Add(new ChatMessage { Role = "assistant", Content = reply });
                }
                else
                {
                    _spriteView?.ShowBubble("网络开小差了喵...");
                }
            }
            catch (HttpRequestException)
            {
                _spriteView?.ShowBubble("网络连接失败喵...");
            }
            catch (TaskCanceledException)
            {
                _spriteView?.ShowBubble("请求超时了喵...");
            }
            catch (Exception ex)
            {
                _spriteView?.ShowBubble($"出错了喵: {ex.Message}");
            }
            finally
            {
                _isProcessingChat = false;
            }
        }

        /// <summary>
        /// 更新悬浮窗位置
        /// </summary>
        public void UpdatePosition(float x, float y)
        {
            if (_layoutParams == null || _windowManager == null) return;

            _layoutParams.X = (int)x;
            _layoutParams.Y = (int)y;
            try
            {
                _windowManager.UpdateViewLayout(_spriteView, _layoutParams);
            }
            catch (WindowManagerBadTokenException)
            {
                // 窗口已失效，忽略
            }
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override void OnDestroy()
        {
            // 移除悬浮窗
            try
            {
                _windowManager?.RemoveView(_spriteView);
            }
            catch { /* 忽略 */ }

            _spriteView?.Dispose();
            _httpClient?.Dispose();
            base.OnDestroy();
        }

        /// <summary>
        /// 悬浮窗触摸事件处理
        /// </summary>
        private class OverlayTouchListener : Java.Lang.Object, IOnTouchListener
        {
            private readonly OverlayService _service;

            public OverlayTouchListener(OverlayService service)
            {
                _service = service;
            }

            public bool OnTouch(View? v, MotionEvent? e)
            {
                if (e == null || _service._layoutParams == null) return false;

                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        _service._touchStartX = e.RawX;
                        _service._touchStartY = e.RawY;
                        _service._viewStartX = _service._layoutParams.X;
                        _service._viewStartY = _service._layoutParams.Y;
                        _service._isDragging = false;
                        _service._touchStartTime = DateTime.UtcNow.Ticks;
                        return true;

                    case MotionEventActions.Move:
                        float dx = e.RawX - _service._touchStartX;
                        float dy = e.RawY - _service._touchStartY;

                        if (Math.Abs(dx) > _service.TAP_DISTANCE || Math.Abs(dy) > _service.TAP_DISTANCE)
                        {
                            _service._isDragging = true;
                        }

                        if (_service._isDragging)
                        {
                            _service.UpdatePosition(
                                _service._viewStartX + dx,
                                _service._viewStartY + dy);
                        }
                        return true;

                    case MotionEventActions.Up:
                        long elapsed = (DateTime.UtcNow.Ticks - _service._touchStartTime) / TimeSpan.TicksPerMillisecond;

                        if (!_service._isDragging && elapsed < _service.TAP_THRESHOLD)
                        {
                            // 点击事件 - 触发聊天
                            _service.OnCatTapped();
                        }
                        return true;
                }

                return false;
            }
        }
    }
}
