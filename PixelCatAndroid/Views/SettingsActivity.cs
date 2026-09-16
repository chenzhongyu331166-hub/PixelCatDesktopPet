using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using PixelCatAndroid.Models;

namespace PixelCatAndroid.Views
{
    /// <summary>
    /// 设置页面 - 用于配置 API 地址和密钥
    /// </summary>
    [Activity(Label = "设置", Theme = "@android:style/Theme.Material.Light.NoActionBar")]
    public class SettingsActivity : Activity
    {
        private EditText _editApiUrl = null!;
        private EditText _editApiKey = null!;
        private EditText _editModel = null!;
        private EditText _editSystemPrompt = null!;
        private Button _btnSave = null!;
        private Button _btnStartService = null!;
        private Button _btnStopService = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 设置布局
            SetContentView(Android.Resource.Layout.ActivityListContent);

            // 使用代码创建简单布局
            var scrollView = new ScrollView(this);
            var layout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            layout.SetPadding(48, 48, 48, 48);

            // 标题
            var title = new TextView(this)
            {
                Text = "桌面猫咪设置",
                TextSize = 24
            };
            title.SetPadding(0, 0, 0, 32);
            layout.AddView(title);

            // API URL
            layout.AddView(CreateLabel("API 地址"));
            _editApiUrl = new EditText(this) { Hint = "https://api.openai.com/v1/chat/completions" };
            layout.AddView(_editApiUrl);

            // API Key
            layout.AddView(CreateLabel("API 密钥"));
            _editApiKey = new EditText(this)
            {
                Hint = "sk-...",
                InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword
            };
            layout.AddView(_editApiKey);

            // 模型名称
            layout.AddView(CreateLabel("模型名称"));
            _editModel = new EditText(this) { Text = "gpt-3.5-turbo" };
            layout.AddView(_editModel);

            // 系统提示词
            layout.AddView(CreateLabel("系统提示词"));
            _editSystemPrompt = new EditText(this)
            {
                Text = "你是一只可爱的像素猫咪桌宠。用简短、可爱的语气回答问题，偶尔卖萌。",
                InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextFlagMultiLine,
                MinLines = 3
            };
            layout.AddView(_editSystemPrompt);

            // 保存按钮
            _btnSave = new Button(this) { Text = "保存设置" };
            _btnSave.Click += OnSaveClicked;
            layout.AddView(_btnSave);

            // 启动服务按钮
            _btnStartService = new Button(this) { Text = "启动桌宠" };
            _btnStartService.Click += OnStartServiceClicked;
            layout.AddView(_btnStartService);

            // 停止服务按钮
            _btnStopService = new Button(this) { Text = "停止桌宠" };
            _btnStopService.Click += OnStopServiceClicked;
            layout.AddView(_btnStopService);

            scrollView.AddView(layout);
            SetContentView(scrollView);

            // 加载已有设置
            LoadSettings();
        }

        private TextView CreateLabel(string text)
        {
            var label = new TextView(this)
            {
                Text = text,
                TextSize = 16
            };
            label.SetPadding(0, 24, 0, 8);
            return label;
        }

        private void LoadSettings()
        {
            var settings = ChatSettings.Load(this);
            _editApiUrl.Text = settings.ApiUrl;
            _editApiKey.Text = settings.ApiKey;
            _editModel.Text = settings.ModelName;
            _editSystemPrompt.Text = settings.SystemPrompt;
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            var settings = new ChatSettings
            {
                ApiUrl = _editApiUrl.Text?.Trim() ?? "https://api.openai.com/v1/chat/completions",
                ApiKey = _editApiKey.Text?.Trim() ?? string.Empty,
                ModelName = _editModel.Text?.Trim() ?? "gpt-3.5-turbo",
                SystemPrompt = _editSystemPrompt.Text?.Trim() ?? "你是一只可爱的像素猫咪桌宠。"
            };
            settings.Save(this);
            Toast.MakeText(this, "设置已保存", ToastLength.Short)?.Show();
        }

        private void OnStartServiceClicked(object? sender, EventArgs e)
        {
            // 检查悬浮窗权限
            if (!Android.Provider.Settings.CanDrawOverlays(this))
            {
                Toast.MakeText(this, "请先授予悬浮窗权限", ToastLength.Long)?.Show();
                var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission,
                    Android.Net.Uri.Parse($"package:{PackageName}"));
                StartActivity(intent);
                return;
            }

            // 启动前台服务
            var serviceIntent = new Intent(this, typeof(Services.OverlayService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(serviceIntent);
            }
            else
            {
                StartService(serviceIntent);
            }

            Toast.MakeText(this, "桌宠已启动", ToastLength.Short)?.Show();
        }

        private void OnStopServiceClicked(object? sender, EventArgs e)
        {
            StopService(new Intent(this, typeof(Services.OverlayService)));
            Toast.MakeText(this, "桌宠已停止", ToastLength.Short)?.Show();
        }
    }
}
