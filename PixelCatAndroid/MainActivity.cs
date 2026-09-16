using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using PixelCatAndroid.Services;

namespace PixelCatAndroid
{
    /// <summary>
    /// 主 Activity - 应用入口，处理权限请求和启动服务
    /// </summary>
    [Activity(Label = "PixelCat", MainLauncher = true,
        Theme = "@android:style/Theme.Material.Light.NoActionBar",
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : Activity
    {
        private const int OVERLAY_PERMISSION_REQUEST = 1001;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 创建简单布局
            var layout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            layout.SetPadding(64, 64, 64, 64);

            // 标题
            var title = new TextView(this)
            {
                Text = "像素猫咪桌宠",
                TextSize = 28
            };
            title.SetPadding(0, 0, 0, 24);
            layout.AddView(title);

            // 说明文字
            var desc = new TextView(this)
            {
                Text = "一只可爱的像素猫咪会出现在你的屏幕上，可以拖动它，点击它会和你聊天哦！",
                TextSize = 14
            };
            desc.SetPadding(0, 0, 0, 32);
            layout.AddView(desc);

            // 状态文本
            var statusText = new TextView(this)
            {
                Text = "状态：未运行",
                TextSize = 16
            };
            statusText.SetPadding(0, 0, 0, 16);
            layout.AddView(statusText);

            // 启动桌宠按钮
            var btnStart = new Button(this)
            {
                Text = "启动桌宠"
            };
            btnStart.Click += async (s, e) =>
            {
                await StartOverlayService(statusText);
            };
            layout.AddView(btnStart);

            // 停止桌宠按钮
            var btnStop = new Button(this)
            {
                Text = "停止桌宠"
            };
            btnStop.Click += (s, e) =>
            {
                StopService(new Intent(this, typeof(OverlayService)));
                statusText.Text = "状态：已停止";
                Toast.MakeText(this, "桌宠已停止", ToastLength.Short)?.Show();
            };
            layout.AddView(btnStop);

            // 设置按钮
            var btnSettings = new Button(this)
            {
                Text = "设置 (配置 API)"
            };
            btnSettings.Click += (s, e) =>
            {
                StartActivity(typeof(Views.SettingsActivity));
            };
            layout.AddView(btnSettings);

            // 检查权限按钮
            var btnCheckPermission = new Button(this)
            {
                Text = "检查悬浮窗权限"
            };
            btnCheckPermission.Click += (s, e) =>
            {
                CheckOverlayPermission();
            };
            layout.AddView(btnCheckPermission);

            SetContentView(layout);
        }

        /// <summary>
        /// 检查并请求悬浮窗权限
        /// </summary>
        private void CheckOverlayPermission()
        {
            if (Android.Provider.Settings.CanDrawOverlays(this))
            {
                Toast.MakeText(this, "悬浮窗权限已授予", ToastLength.Short)?.Show();
            }
            else
            {
                var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission,
                    Android.Net.Uri.Parse($"package:{PackageName}"));
                StartActivityForResult(intent, OVERLAY_PERMISSION_REQUEST);
            }
        }

        /// <summary>
        /// 启动悬浮服务
        /// </summary>
        private async System.Threading.Tasks.Task StartOverlayService(Android.Widget.TextView statusText)
        {
            // 先检查权限
            if (!Android.Provider.Settings.CanDrawOverlays(this))
            {
                Toast.MakeText(this, "请先授予悬浮窗权限", ToastLength.Long)?.Show();
                CheckOverlayPermission();
                return;
            }

            // 启动前台服务
            var serviceIntent = new Intent(this, typeof(OverlayService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(serviceIntent);
            }
            else
            {
                StartService(serviceIntent);
            }

            statusText.Text = "状态：运行中";
            Toast.MakeText(this, "桌宠已启动！", ToastLength.Short)?.Show();

            await System.Threading.Tasks.Task.CompletedTask;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == OVERLAY_PERMISSION_REQUEST)
            {
                if (Android.Provider.Settings.CanDrawOverlays(this))
                {
                    Toast.MakeText(this, "悬浮窗权限已授予", ToastLength.Short)?.Show();
                }
                else
                {
                    Toast.MakeText(this, "悬浮窗权限被拒绝，桌宠无法启动", ToastLength.Long)?.Show();
                }
            }
        }

        public override void OnBackPressed()
        {
            // 按返回键不退出，而是最小化到后台
            MoveTaskToBack(true);
        }
    }
}
