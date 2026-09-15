using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Media;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace PixelCatDesktopPet;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new PetWindow());
    }
}

internal enum PetAnimation
{
    None,
    Jump,
    Squash,
    Shake,
    Lick,
    Spin,
    Stretch,
    Pounce
}

internal sealed class PetWindow : LayeredForm
{
    private static readonly string[] ClickPhrases =
    [
        "喵？你找我有事吗？",
        "今天的我，也是一团可爱的黑影。",
        "别戳啦，我会变成猫猫饼的！",
        "跳起来才有精神！",
        "左晃晃，右晃晃，烦恼全跑光。",
        "我可是桌面守卫喵。",
        "摸摸可以，工作也要继续哦。",
        "咕噜咕噜……电量充足！",
        "黑夜给了我黑色的毛，也给了我闪亮的眼。",
        "再点一下，说不定有惊喜。"
    ];

    private static readonly string[] ProactivePhrases =
    [
        "我巡逻了一圈，桌面一切正常！",
        "偷偷提醒你：喝口水，眼睛会感谢你。",
        "如果累了，就看看窗外，我马上来监督休息。",
        "今天的任务也一定能一点点完成。",
        "我在这里陪你，不着急，慢慢来。",
        "肩膀放松，背挺直，喵式健康检查通过！"
    ];

    private static readonly string[] WalkPhrases =
    [
        "巡逻时间到，我去那边看看。",
        "猫猫巡查中，请桌面保持可爱。",
        "换个位置，顺便活动一下爪子。"
    ];

    private static readonly string[] DragPhrases =
    [
        "这个位置不错，就在这里站岗吧！",
        "搬新家啦？我喜欢这里。",
        "好嘞，猫猫已就位。"
    ];

    private static readonly string[] ChatFallbacks =
    [
        "收到！虽然我只会简单聊天，但陪伴是认真的。",
        "喵呜，我听懂了一点点，也很想继续陪你。",
        "这句话我先收进猫猫小本本里了。",
        "不管怎样，先给你一个精神上的猫猫抱抱。"
    ];

    private static readonly string[] PettingPhrases =
    [
        "咕噜咕噜咕噜……继续，不要停。",
        "喵呜～就是这里，手法不错。",
        "被摸摸的感觉，值三个小鱼干。",
        "嗯……勉强认可你的按摩技术。",
        "再摸两下，我就要翻肚皮了。",
        "今天的人类格外温柔喵。",
        "舒服到尾巴都翘起来了！"
    ];

    private static readonly string[] PurrPhrases =
    [
        "咕噜……咕噜……咕噜……",
        "（发出小摩托一样的呼噜声）",
        "呼噜呼噜，猫猫引擎启动。"
    ];

    private static readonly string[] AnnoyedPhrases =
    [
        "喂！毛都要被你摸秃啦！",
        "猫猫的忍耐是有限度的喵！",
        "再摸，再摸我就……我就换个地方趴！",
        "（假装要咬你）嗷呜！"
    ];

    private static readonly string[] EatPhrases =
    [
        "开饭啦开饭啦！",
        "吧唧吧唧……今天的猫粮格外香。",
        "干饭时间，请勿打扰喵。",
        "先吃饱，才有力气巡逻桌面。"
    ];

    private static readonly string[] EatingBusyPhrases =
    [
        "等我吃完这一口……",
        "干饭猫，干饭魂，吃完再陪你玩。",
        "（嘴里塞满猫粮，说不出话）"
    ];

    private static readonly string[] SleepPhrases =
    [
        "猫猫困了，眯一会儿……",
        "眼皮好重，先睡为敬。",
        "我要进入充电模式了，晚安喵。"
    ];

    private static readonly string[] ZzzPhrases =
    [
        "Zzz……",
        "Zzz……Zzz……",
        "（呼呼大睡中）"
    ];

    private static readonly string[] WakePhrases =
    [
        "唔……天亮了吗？",
        "（打了个大大的哈欠）睡得真香。",
        "充电完毕，猫猫满血复活！"
    ];

    private static readonly string[] DisturbedSleepPhrases =
    [
        "唔……别吵……再睡五分钟……",
        "（翻了个身，继续睡）",
        "喵……梦里的小鱼干差点到手了……"
    ];

    private readonly Bitmap _idleSprite;
    private readonly Bitmap _lickSprite1;
    private readonly Bitmap _lickSprite2;
    private readonly Bitmap _lickSprite3;
    private readonly Bitmap _walkSprite1;
    private readonly Bitmap _walkSprite2;
    private readonly Bitmap _walkSprite3;
    private readonly Bitmap _walkSprite4;
    private readonly Bitmap _walkSprite5;
    private readonly Bitmap _walkSprite6;
    private readonly Bitmap _walkSprite7;
    private readonly Bitmap _walkSprite8;
    private readonly Bitmap _sleepSprite;
    private readonly Bitmap _eatSprite1;
    private readonly Bitmap _eatSprite2;
    private readonly Bitmap _eatSprite3;
    private readonly byte[] _idleAlpha;
    private readonly byte[] _lickAlpha1;
    private readonly byte[] _lickAlpha2;
    private readonly byte[] _lickAlpha3;
    private readonly byte[] _walkAlpha1;
    private readonly byte[] _walkAlpha2;
    private readonly byte[] _walkAlpha3;
    private readonly byte[] _walkAlpha4;
    private readonly byte[] _walkAlpha5;
    private readonly byte[] _walkAlpha6;
    private readonly byte[] _walkAlpha7;
    private readonly byte[] _walkAlpha8;
    private readonly byte[] _sleepAlpha;
    private readonly byte[] _eatAlpha1;
    private readonly byte[] _eatAlpha2;
    private readonly byte[] _eatAlpha3;
    private readonly int _spriteWidth;
    private readonly int _spriteHeight;
    private readonly float _initialScale;
    private readonly BubbleWindow _bubble;
    private readonly ContextMenuStrip _menu;
    private readonly List<ToolStripMenuItem> _sizeItems = [];
    private readonly List<ToolStripMenuItem> _intervalItems = [];
    private readonly ToolStripMenuItem _topMostItem;
    private readonly ToolStripMenuItem _autoMoveItem;
    private readonly ToolStripMenuItem _proactiveTalkItem;
    private readonly ToolStripMenuItem _breakReminderItem;
    private readonly ToolStripMenuItem _breakMenu;
    private readonly System.Windows.Forms.Timer _animationTimer;
    private readonly System.Windows.Forms.Timer _behaviorTimer;
    private readonly System.Windows.Forms.Timer _topMostTimer;
    private readonly Stopwatch _animationWatch = new();
    private readonly Stopwatch _walkWatch = new();
    private readonly Random _random = new();
    private readonly List<DateTime> _recentPettingTimes = [];
    private AiChatWindow? _aiChatWindow;

    private enum PetActivity
    {
        None,
        Eating,
        Sleeping
    }

    private float _scaleFactor = 0.42f;
    private int _petWidth;
    private int _petHeight;
    private int _sideMargin;
    private int _topMargin;
    private int _bottomMargin;
    private RectangleF _baseRect;
    private float[]? _hitInverse;
    private PetAnimation _currentAnimation = PetAnimation.None;
    private PetActivity _activity = PetActivity.None;
    private int _lastClickPhraseIndex = -1;
    private int _lastProactivePhraseIndex = -1;
    private int _lastWalkPhraseIndex = -1;
    private int _lastDragPhraseIndex = -1;
    private int _lastChatFallbackIndex = -1;
    private int _lastPettingPhraseIndex = -1;
    private int _lastPurrPhraseIndex = -1;
    private int _lastAnnoyedPhraseIndex = -1;
    private int _lastEatPhraseIndex = -1;
    private int _lastEatingBusyPhraseIndex = -1;
    private int _lastSleepPhraseIndex = -1;
    private int _lastZzzPhraseIndex = -1;
    private int _lastWakePhraseIndex = -1;
    private int _lastDisturbedSleepPhraseIndex = -1;
    private bool _mouseDown;
    private bool _dragging;
    private bool _updatingMenu;
    private bool _autoMoveEnabled = true;
    private bool _proactiveTalkEnabled = true;
    private bool _breakReminderEnabled = true;
    private bool _isWalking;
    private bool _isResting;
    private bool _facingLeft;
    private bool _welcomeShown;
    private int _breakIntervalMinutes = 45;
    private const int RestDurationMinutes = 5;
    private int _walkTargetX;
    private int _walkDurationMilliseconds;
    private float _walkBobOffsetY;
    private float _walkLateralOffset;
    private Point _walkStartLocation;
    private Point _dragCursorStart;
    private Point _dragWindowStart;
    private DateTime _sessionStartedAt = DateTime.Now;
    private DateTime _nextBreakAt = DateTime.Now.AddMinutes(45);
    private DateTime _restEndsAt;
    private DateTime _nextBehaviorAt = DateTime.Now.AddSeconds(3);
    private DateTime _nextProactiveTalkAt = DateTime.Now.AddMinutes(2);
    private DateTime _activityEndsAt;
    private DateTime _nextZzzAt;
    private TimeSpan _offWorkTime = new(17, 30, 0);
    private string _offWorkTimeText = "17:30";

    public PetWindow()
    {
        _idleSprite = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat.png");
        _lickSprite1 = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_lick_1.png");
        _lickSprite2 = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_lick_2.png");
        _lickSprite3 = BlendSprites(_lickSprite1, _lickSprite2, 0.5f);
        _walkSprite1 = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_walk_1.png");
        _walkSprite2 = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_walk_2.png");
        // 用两张行走帧插值出六个中间帧，组成八帧步态循环，动作更流畅。
        _walkSprite3 = BlendSprites(_walkSprite1, _walkSprite2, 1f / 7f);
        _walkSprite4 = BlendSprites(_walkSprite1, _walkSprite2, 2f / 7f);
        _walkSprite5 = BlendSprites(_walkSprite1, _walkSprite2, 3f / 7f);
        _walkSprite6 = BlendSprites(_walkSprite1, _walkSprite2, 4f / 7f);
        _walkSprite7 = BlendSprites(_walkSprite1, _walkSprite2, 5f / 7f);
        _walkSprite8 = BlendSprites(_walkSprite1, _walkSprite2, 6f / 7f);
        _sleepSprite = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_sleep.png");
        _eatSprite1 = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_eat_1.png");
        _eatSprite2 = LoadEmbeddedSprite("PixelCatDesktopPet.Assets.cat_eat_2.png");
        _eatSprite3 = BlendSprites(_eatSprite1, _eatSprite2, 0.5f);
        _spriteWidth = _idleSprite.Width;
        _spriteHeight = _idleSprite.Height;
        _idleAlpha = ExtractAlpha(_idleSprite);
        _lickAlpha1 = ExtractAlpha(_lickSprite1);
        _lickAlpha2 = ExtractAlpha(_lickSprite2);
        _lickAlpha3 = ExtractAlpha(_lickSprite3);
        _walkAlpha1 = ExtractAlpha(_walkSprite1);
        _walkAlpha2 = ExtractAlpha(_walkSprite2);
        _walkAlpha3 = ExtractAlpha(_walkSprite3);
        _walkAlpha4 = ExtractAlpha(_walkSprite4);
        _walkAlpha5 = ExtractAlpha(_walkSprite5);
        _walkAlpha6 = ExtractAlpha(_walkSprite6);
        _walkAlpha7 = ExtractAlpha(_walkSprite7);
        _walkAlpha8 = ExtractAlpha(_walkSprite8);
        _sleepAlpha = ExtractAlpha(_sleepSprite);
        _eatAlpha1 = ExtractAlpha(_eatSprite1);
        _eatAlpha2 = ExtractAlpha(_eatSprite2);
        _eatAlpha3 = ExtractAlpha(_eatSprite3);
        _initialScale = 240f / _spriteHeight;

        _bubble = new BubbleWindow();
        _menu = CreateMenu(out _topMostItem, out _autoMoveItem, out _proactiveTalkItem, out _breakReminderItem, out _breakMenu);
        _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _animationTimer.Tick += (_, _) => RenderFrame();
        _behaviorTimer = new System.Windows.Forms.Timer { Interval = 40 };
        _behaviorTimer.Tick += (_, _) => BehaviorTick();
        _topMostTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        _topMostTimer.Tick += (_, _) => ReassertTopMost();
        _nextProactiveTalkAt = DateTime.Now.AddMinutes(_random.Next(2, 5));

        UpdateDimensions();
        PositionAtBottomRight();
        TopMost = true;
        _behaviorTimer.Start();
        _topMostTimer.Start();
    }

    private static Bitmap LoadEmbeddedSprite(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new InvalidOperationException("内嵌角色素材缺失。");
        }

        using var loaded = Image.FromStream(stream);
        return new Bitmap(loaded);
    }

    private static byte[] ExtractAlpha(Bitmap bitmap)
    {
        var result = new byte[bitmap.Width * bitmap.Height];
        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            byte[] row = new byte[data.Stride];
            for (int y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(data.Scan0 + y * data.Stride, row, 0, data.Stride);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    result[y * bitmap.Width + x] = row[x * 4 + 3];
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return result;
    }

    private ContextMenuStrip CreateMenu(
        out ToolStripMenuItem topMostItem,
        out ToolStripMenuItem autoMoveItem,
        out ToolStripMenuItem proactiveTalkItem,
        out ToolStripMenuItem breakReminderItem,
        out ToolStripMenuItem breakMenu)
    {
        var menu = new ContextMenuStrip();
        var countdownItem = new ToolStripMenuItem("下班倒计时…");
        countdownItem.Click += (_, _) => ShowCountdownInfo();
        var setOffWorkItem = new ToolStripMenuItem("设置下班时间…");
        setOffWorkItem.Click += (_, _) => SetOffWorkTime();
        var aiChatItem = new ToolStripMenuItem("AI 聊天与记录…");
        aiChatItem.Click += (_, _) => OpenAiChatWindow();
        var chatItem = new ToolStripMenuItem("快速逗猫（本地）…");
        chatItem.Click += (_, _) => OpenChatDialog();
        var feedItem = new ToolStripMenuItem("喂饭");
        feedItem.Click += (_, _) => StartEating(DateTime.Now, true);
        var sleepItem = new ToolStripMenuItem("哄睡");
        sleepItem.Click += (_, _) => StartSleeping(DateTime.Now, true);

        var moveItem = new ToolStripMenuItem("自主移动")
        {
            CheckOnClick = true,
            Checked = true
        };
        moveItem.CheckedChanged += (_, _) =>
        {
            if (!_updatingMenu)
            {
                SetAutoMoveEnabled(moveItem.Checked);
            }
        };
        autoMoveItem = moveItem;

        var talkItem = new ToolStripMenuItem("主动说话")
        {
            CheckOnClick = true,
            Checked = true
        };
        talkItem.CheckedChanged += (_, _) =>
        {
            if (!_updatingMenu)
            {
                SetProactiveTalkEnabled(talkItem.Checked);
            }
        };
        proactiveTalkItem = talkItem;

        breakMenu = new ToolStripMenuItem("休息提醒");
        var reminderItem = new ToolStripMenuItem("开启休息提醒")
        {
            CheckOnClick = true,
            Checked = true
        };
        reminderItem.CheckedChanged += (_, _) =>
        {
            if (!_updatingMenu)
            {
                SetBreakReminderEnabled(reminderItem.Checked);
            }
        };
        breakReminderItem = reminderItem;
        breakMenu.DropDownItems.Add(breakReminderItem);
        breakMenu.DropDownItems.Add(new ToolStripSeparator());
        AddBreakIntervalItem(breakMenu, "每 30 分钟提醒", 30);
        AddBreakIntervalItem(breakMenu, "每 45 分钟提醒", 45);
        AddBreakIntervalItem(breakMenu, "每 60 分钟提醒", 60);
        AddBreakIntervalItem(breakMenu, "每 90 分钟提醒", 90);
        breakMenu.DropDownItems.Add(new ToolStripSeparator());
        var breakNowItem = new ToolStripMenuItem("立即开始 5 分钟休息");
        breakNowItem.Click += (_, _) => StartRestReminder(DateTime.Now, true);
        breakMenu.DropDownItems.Add(breakNowItem);

        var sizeMenu = new ToolStripMenuItem("调整大小");
        AddSizeItem(sizeMenu, "迷你（42%）", 0.42f);
        AddSizeItem(sizeMenu, "小巧（55%）", 0.55f);
        AddSizeItem(sizeMenu, "标准（85%）", 0.85f);
        AddSizeItem(sizeMenu, "醒目（120%）", 1.20f);
        AddSizeItem(sizeMenu, "大大猫（170%）", 1.70f);

        var autoStartItem = new ToolStripMenuItem("开机自启动")
        {
            CheckOnClick = true,
            Checked = IsAutoStartEnabled()
        };
        autoStartItem.CheckedChanged += (_, _) => SetAutoStartEnabled(autoStartItem.Checked);

        var alwaysTopItem = new ToolStripMenuItem("始终置顶")
        {
            CheckOnClick = true,
            Checked = true
        };
        alwaysTopItem.CheckedChanged += (_, _) =>
        {
            if (_updatingMenu)
            {
                return;
            }
            SetPetTopMost(alwaysTopItem.Checked);
        };
        topMostItem = alwaysTopItem;

        var exitItem = new ToolStripMenuItem("退出程序");
        exitItem.Click += (_, _) => Close();

        menu.Items.Add(aiChatItem);
        menu.Items.Add(chatItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(countdownItem);
        menu.Items.Add(setOffWorkItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(feedItem);
        menu.Items.Add(sleepItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(autoMoveItem);
        menu.Items.Add(proactiveTalkItem);
        menu.Items.Add(breakMenu);
        menu.Items.Add(sizeMenu);
        menu.Items.Add(topMostItem);
        menu.Items.Add(autoStartItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);
        menu.Opening += (_, _) => RefreshMenuChecks();
        return menu;
    }

    private void AddSizeItem(ToolStripMenuItem parent, string text, float factor)
    {
        var item = new ToolStripMenuItem(text) { Tag = factor };
        item.Click += (_, _) => SetScaleFactor(factor);
        parent.DropDownItems.Add(item);
        _sizeItems.Add(item);
    }

    private void AddBreakIntervalItem(ToolStripMenuItem parent, string text, int minutes)
    {
        var item = new ToolStripMenuItem(text) { Tag = minutes };
        item.Click += (_, _) => SetBreakInterval(minutes);
        parent.DropDownItems.Add(item);
        _intervalItems.Add(item);
    }

    private void RefreshMenuChecks()
    {
        _updatingMenu = true;
        try
        {
            foreach (ToolStripMenuItem item in _sizeItems)
            {
                float factor = item.Tag is float value ? value : 1.0f;
                item.Checked = Math.Abs(factor - _scaleFactor) < 0.03f;
            }
            foreach (ToolStripMenuItem item in _intervalItems)
            {
                int minutes = item.Tag is int value ? value : 45;
                item.Checked = minutes == _breakIntervalMinutes;
            }
            _topMostItem.Checked = TopMost;
            _autoMoveItem.Checked = _autoMoveEnabled;
            _proactiveTalkItem.Checked = _proactiveTalkEnabled;
            _breakReminderItem.Checked = _breakReminderEnabled;
            _breakMenu.Text = _breakReminderEnabled
                ? $"休息提醒（下次 {_nextBreakAt:HH:mm}）"
                : "休息提醒（已关闭）";
        }
        finally
        {
            _updatingMenu = false;
        }
    }

    private void SetPetTopMost(bool topMost)
    {
        if (TopMost != topMost)
        {
            TopMost = topMost;
        }
        _bubble.SyncTopMost(topMost);
        if (IsHandleCreated)
        {
            _bubble.ShowMessage(topMost ? "我会一直待在最上面喵。" : "我会低调一点，不挡住其他窗口。", GetPetScreenBounds(), TopMost, 3600);
        }
        RenderFrame();
    }

    private const string AutoStartValueName = "像素黑猫桌宠";

    private static string? AutoStartTarget()
    {
        return Process.GetCurrentProcess().MainModule?.FileName;
    }

    private static bool IsAutoStartEnabled()
    {
        string? target = AutoStartTarget();
        if (string.IsNullOrEmpty(target))
        {
            return false;
        }
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: false);
        string? value = key?.GetValue(AutoStartValueName) as string;
        return string.Equals(value, "\"" + target + "\"", StringComparison.OrdinalIgnoreCase);
    }

    private static void SetAutoStartEnabled(bool enabled)
    {
        string? target = AutoStartTarget();
        if (string.IsNullOrEmpty(target))
        {
            return;
        }
        using RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
        if (enabled)
        {
            key.SetValue(AutoStartValueName, "\"" + target + "\"");
        }
        else
        {
            key.DeleteValue(AutoStartValueName, throwOnMissingValue: false);
        }
    }

    private void SetAutoMoveEnabled(bool enabled)
    {
        _autoMoveEnabled = enabled;
        if (!enabled)
        {
            CancelWalk();
            _bubble.ShowMessage("我会乖乖待在原地，不乱跑。", GetPetScreenBounds(), TopMost, 3600);
        }
        else
        {
            _nextBehaviorAt = DateTime.Now.AddSeconds(1);
            _bubble.ShowMessage("收到，我开始自己巡逻啦。", GetPetScreenBounds(), TopMost, 3600);
        }
    }

    private void SetProactiveTalkEnabled(bool enabled)
    {
        _proactiveTalkEnabled = enabled;
        _nextProactiveTalkAt = DateTime.Now.AddMinutes(_random.Next(2, 5));
        _bubble.ShowMessage(enabled ? "我会偶尔主动找你说话。" : "我会安静一点，除非你戳我。", GetPetScreenBounds(), TopMost, 3600);
    }

    private void SetBreakReminderEnabled(bool enabled)
    {
        _breakReminderEnabled = enabled;
        _isResting = false;
        _sessionStartedAt = DateTime.Now;
        _nextBreakAt = DateTime.Now.AddMinutes(_breakIntervalMinutes);
        _bubble.ShowMessage(
            enabled ? $"休息提醒已开启，下次大约在 {_nextBreakAt:HH:mm}。" : "休息提醒已关闭，不过也别忘了照顾自己。",
            GetPetScreenBounds(),
            TopMost,
            5200);
    }

    private void SetBreakInterval(int minutes)
    {
        _breakIntervalMinutes = minutes;
        _isResting = false;
        _sessionStartedAt = DateTime.Now;
        _nextBreakAt = DateTime.Now.AddMinutes(minutes);
        _bubble.ShowMessage($"好的，我会每 {minutes} 分钟提醒你休息一次。", GetPetScreenBounds(), TopMost, 4600);
    }

    private void ShowCountdownInfo()
    {
        CancelWalk();
        string message = BuildCountdownMessage(DateTime.Now);
        _bubble.ShowMessage(message, GetPetScreenBounds(), TopMost, 9000);
        RenderFrame();
    }

    private void SetOffWorkTime()
    {
        using var dialog = new OffWorkTimeDialog(_offWorkTimeText);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string value = dialog.Value;
        if (TimeSpan.TryParse(value, out TimeSpan parsed))
        {
            _offWorkTime = parsed;
            _offWorkTimeText = value;
            _bubble.ShowMessage($"收到，下班时间设置为 {_offWorkTimeText} 喵。", GetPetScreenBounds(), TopMost, 4000);
        }
        else
        {
            _bubble.ShowMessage("这个时间格式我没看懂，请用 HH:mm 的格式再试试喵。", GetPetScreenBounds(), TopMost, 4000);
        }
        RenderFrame();
    }

    private string BuildCountdownMessage(DateTime now)
    {
        var (holidayDays, holidayName) = GetNextHolidayInfo(now);
        string holidayLine = holidayDays is null
            ? "下一个节假日……暂时还没找到喵。"
            : holidayDays == 0
                ? $"今天就是{holidayName}啦，好好享受假期喵！"
                : $"离{holidayName}还有 {holidayDays} 天。";

        bool isWeekend = now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        if (isWeekend)
        {
            return $"现在是 {now:HH:mm}，周末啦！离{holidayName}还有 {holidayDays} 天，好好休息喵。";
        }

        DateTime offTimeToday = now.Date + _offWorkTime;
        string weekendLine;
        if (now >= offTimeToday)
        {
            weekendLine = now.DayOfWeek == DayOfWeek.Friday
                ? "周末马上开始啦！"
                : $"离周末还有 {6 - (int)now.DayOfWeek} 天，坚持住喵。";
            return $"已经下班啦！现在是 {now:HH:mm}，今天辛苦了。{weekendLine} {holidayLine}";
        }

        TimeSpan remaining = offTimeToday - now;
        int hours = (int)remaining.TotalHours;
        int minutes = remaining.Minutes;
        string remainingText = hours > 0 ? $"{hours}小时{minutes}分钟" : $"{minutes}分钟";
        weekendLine = now.DayOfWeek == DayOfWeek.Friday
            ? "今天是周五，撑到下班就放假喵！"
            : $"周末还要等 {6 - (int)now.DayOfWeek} 天。";
        return $"现在 {now:HH:mm}，离下班（{_offWorkTimeText}）还有 {remainingText}。{weekendLine} {holidayLine}";
    }

    private static (int? Days, string Name) GetNextHolidayInfo(DateTime now)
    {
        var fixedHolidays = new[]
        {
            (1, 1, "元旦"),
            (5, 1, "劳动节"),
            (10, 1, "国庆节")
        };

        for (int offset = 0; offset <= 1; offset++)
        {
            int year = now.Year + offset;
            foreach ((int month, int day, string name) in fixedHolidays)
            {
                DateTime date;
                try
                {
                    date = new DateTime(year, month, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }

                if (date.Date < now.Date)
                {
                    continue;
                }

                return (date.Date == now.Date ? 0 : (date - now).Days, name);
            }
        }

        return (null, string.Empty);
    }

    private void PositionAtBottomRight()
    {
        Rectangle workArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        Location = new Point(
            Math.Max(workArea.Left, workArea.Right - Width - 48),
            Math.Max(workArea.Top, workArea.Bottom - Height - 24));
    }

    private Point GetPetAnchorScreen()
    {
        return new Point(
            Location.X + (int)Math.Round(_baseRect.X + _baseRect.Width / 2f),
            Location.Y + (int)Math.Round(_baseRect.Bottom));
    }

    private Rectangle GetPetScreenBounds()
    {
        return new Rectangle(
            Location.X + (int)Math.Round(_baseRect.X),
            Location.Y + (int)Math.Round(_baseRect.Y),
            _petWidth,
            _petHeight);
    }

    private void SetScaleFactor(float factor)
    {
        factor = Math.Clamp(factor, 0.35f, 3.0f);
        if (Math.Abs(factor - _scaleFactor) < 0.001f)
        {
            return;
        }

        Point anchor = GetPetAnchorScreen();
        CancelWalk();
        _scaleFactor = factor;
        UpdateDimensions();
        Location = new Point(
            anchor.X - (int)Math.Round(_baseRect.X + _baseRect.Width / 2f),
            anchor.Y - (int)Math.Round(_baseRect.Bottom));
        _bubble.HideBubble();
        RenderFrame();
    }

    private void UpdateDimensions()
    {
        float pixelScale = _initialScale * _scaleFactor;
        _petWidth = Math.Max(24, (int)Math.Round(_spriteWidth * pixelScale));
        _petHeight = Math.Max(24, (int)Math.Round(_spriteHeight * pixelScale));
        _sideMargin = Math.Max(24, (int)Math.Ceiling(_petWidth * 0.20f));
        _topMargin = Math.Max(44, (int)Math.Ceiling(_petHeight * 0.38f));
        _bottomMargin = Math.Max(8, (int)Math.Ceiling(_petHeight * 0.05f));
        _baseRect = new RectangleF(_sideMargin, _topMargin, _petWidth, _petHeight);
        Size = new Size(
            _petWidth + _sideMargin * 2,
            _petHeight + _topMargin + _bottomMargin);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RenderFrame();
        if (!_welcomeShown)
        {
            _welcomeShown = true;
            _nextBehaviorAt = DateTime.Now.AddSeconds(7);
            _bubble.ShowMessage(
                $"你好呀，现在是 {DateTime.Now:HH:mm}。我会陪你工作，也会提醒你按时休息。",
                GetPetScreenBounds(),
                TopMost,
                6500);
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RenderFrame();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            CancelWalk();
            _currentAnimation = PetAnimation.None;
            _animationTimer.Stop();
            _mouseDown = true;
            _dragging = false;
            _dragCursorStart = Cursor.Position;
            _dragWindowStart = Location;
            Capture = true;
            RenderFrame();
        }
        else if (e.Button == MouseButtons.Right)
        {
            CancelWalk();
            RefreshMenuChecks();
            _menu.Show(this, e.Location);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_mouseDown || (e.Button & MouseButtons.Left) == 0)
        {
            return;
        }

        Point cursor = Cursor.Position;
        int deltaX = cursor.X - _dragCursorStart.X;
        int deltaY = cursor.Y - _dragCursorStart.Y;
        if (!_dragging && Math.Abs(deltaX) + Math.Abs(deltaY) > 5)
        {
            _dragging = true;
            _bubble.HideBubble();
        }

        if (_dragging)
        {
            Location = new Point(_dragWindowStart.X + deltaX, _dragWindowStart.Y + deltaY);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button != MouseButtons.Left || !_mouseDown)
        {
            return;
        }

        bool wasDragging = _dragging;
        int movedDistance = Math.Abs(Location.X - _dragWindowStart.X) + Math.Abs(Location.Y - _dragWindowStart.Y);
        _mouseDown = false;
        _dragging = false;
        Capture = false;
        _nextBehaviorAt = DateTime.Now.AddSeconds(_random.Next(3, 7));
        if (!wasDragging)
        {
            TriggerNextInteraction();
        }
        else
        {
            if (_activity != PetActivity.None && movedDistance > 60)
            {
                // 吃饭/睡觉时被抱走，活动被打断
                EndActivity(DateTime.Now, false);
                _bubble.ShowMessage(
                    _random.NextDouble() < 0.5 ? "喵？突然被抱起来了！" : "好吧，换个地方也一样。",
                    GetPetScreenBounds(), TopMost, 3000);
            }
            else if (movedDistance > 60 && _random.NextDouble() < 0.55)
            {
                _bubble.ShowMessage(GetRandomPhrase(DragPhrases, ref _lastDragPhraseIndex), GetPetScreenBounds(), TopMost, 3400);
            }
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        float multiplier = e.Delta > 0 ? 1.08f : 1f / 1.08f;
        SetScaleFactor(_scaleFactor * multiplier);
    }

    private void TriggerNextInteraction()
    {
        DateTime now = DateTime.Now;

        if (_activity == PetActivity.Sleeping)
        {
            // 睡觉时被摸：大概率嘟囔着继续睡，小概率被吵醒
            if (_random.NextDouble() < 0.30)
            {
                EndActivity(now, false);
                _currentAnimation = PetAnimation.Shake;
                _animationWatch.Restart();
                _animationTimer.Start();
                _bubble.ShowMessage("喵？！……好吧好吧，我醒了。", GetPetScreenBounds(), TopMost, 3600);
            }
            else
            {
                _animationTimer.Start();
                _bubble.ShowMessage(GetRandomPhrase(DisturbedSleepPhrases, ref _lastDisturbedSleepPhraseIndex), GetPetScreenBounds(), TopMost, 3000);
            }
            RenderFrame();
            return;
        }

        if (_activity == PetActivity.Eating)
        {
            _animationTimer.Start();
            _bubble.ShowMessage(GetRandomPhrase(EatingBusyPhrases, ref _lastEatingBusyPhraseIndex), GetPetScreenBounds(), TopMost, 3000);
            RenderFrame();
            return;
        }

        // 连续快速摸头会被嫌弃
        _recentPettingTimes.Add(now);
        _recentPettingTimes.RemoveAll(t => (now - t).TotalSeconds > 3.5);
        if (_recentPettingTimes.Count >= 4)
        {
            _recentPettingTimes.Clear();
            _currentAnimation = PetAnimation.Shake;
            _animationWatch.Restart();
            _animationTimer.Start();
            _bubble.ShowMessage(GetRandomPhrase(AnnoyedPhrases, ref _lastAnnoyedPhraseIndex), GetPetScreenBounds(), TopMost, 3400);
            RenderFrame();
            return;
        }

        // 猫咪对摸头的随机反应：享受/呼噜/回蹭/开心跳/理毛/抖一抖
        double roll = _random.NextDouble();
        string phrase;
        PetAnimation reaction;
        if (roll < 0.28)
        {
            reaction = PetAnimation.Squash;
            phrase = GetRandomPhrase(PettingPhrases, ref _lastPettingPhraseIndex);
        }
        else if (roll < 0.45)
        {
            reaction = PetAnimation.Squash;
            phrase = GetRandomPhrase(PurrPhrases, ref _lastPurrPhraseIndex);
        }
        else if (roll < 0.58)
        {
            reaction = PetAnimation.Lick;
            phrase = GetRandomPhrase(ClickPhrases, ref _lastClickPhraseIndex);
        }
        else if (roll < 0.72)
        {
            reaction = PetAnimation.Jump;
            phrase = GetRandomPhrase(PettingPhrases, ref _lastPettingPhraseIndex);
        }
        else if (roll < 0.84)
        {
            reaction = PetAnimation.Spin;
            phrase = GetRandomPhrase(PettingPhrases, ref _lastPettingPhraseIndex);
        }
        else if (roll < 0.94)
        {
            reaction = PetAnimation.Stretch;
            phrase = GetRandomPhrase(PettingPhrases, ref _lastPettingPhraseIndex);
        }
        else
        {
            reaction = PetAnimation.Shake;
            phrase = GetRandomPhrase(ClickPhrases, ref _lastClickPhraseIndex);
        }

        _currentAnimation = reaction;
        _animationWatch.Restart();
        _animationTimer.Start();
        _bubble.ShowMessage(phrase, GetPetScreenBounds(), TopMost);
        RenderFrame();
    }

    private string GetRandomPhrase(string[] phrases, ref int lastIndex)
    {
        if (phrases.Length == 1)
        {
            return phrases[0];
        }

        int index;
        do
        {
            index = _random.Next(phrases.Length);
        } while (index == lastIndex);
        lastIndex = index;
        return phrases[index];
    }

    private static Bitmap BlendSprites(Bitmap from, Bitmap to, float weight)
    {
        int width = Math.Min(from.Width, to.Width);
        int height = Math.Min(from.Height, to.Height);
        var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        // weight 表示 to 混合进结果的比例：先画 from（不透明度 weight），再叠 to（不透明度 1-weight）。
        // SourceOver 逐像素合成后，结果近似 from*weight + to*(1-weight)。
        float fromOpacity = Math.Clamp(weight, 0f, 1f);
        float toOpacity = Math.Clamp(1f - weight, 0f, 1f);
        using var fromAttributes = new ImageAttributes();
        fromAttributes.SetColorMatrix(CreateColorMatrix(fromOpacity));
        using var toAttributes = new ImageAttributes();
        toAttributes.SetColorMatrix(CreateColorMatrix(toOpacity));
        graphics.DrawImage(from, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, fromAttributes);
        graphics.DrawImage(to, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, toAttributes);
        return result;
    }

    private static ColorMatrix CreateColorMatrix(float alpha)
    {
        return new ColorMatrix(new float[][]
        {
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, alpha, 0],
            [0, 0, 0, 0, 1]
        });
    }

    private void StartAnimation(PetAnimation animation)
    {
        CancelWalk();
        _activity = PetActivity.None;
        _currentAnimation = animation;
        _animationWatch.Restart();
        _animationTimer.Start();
    }

    private void BehaviorTick()
    {
        if (IsDisposed || !Visible)
        {
            return;
        }

        DateTime now = DateTime.Now;
        CheckBreakReminder(now);
        if (_mouseDown)
        {
            return;
        }

        if (_isWalking)
        {
            UpdateWalk(now);
            return;
        }

        if (_activity != PetActivity.None)
        {
            UpdateActivity(now);
            return;
        }

        if (_autoMoveEnabled && !_isResting && _currentAnimation == PetAnimation.None && now >= _nextBehaviorAt)
        {
            StartIdleBehavior(now);
        }

        if (_proactiveTalkEnabled && !_isResting && now >= _nextProactiveTalkAt)
        {
            if (_bubble.Visible)
            {
                _nextProactiveTalkAt = now.AddSeconds(35);
            }
            else
            {
                SpeakProactively(now);
            }
        }
    }

    private enum IdleBehavior
    {
        Walk,
        Groom,
        Play,
        Eat,
        Sleep,
        Sit
    }

    private void StartIdleBehavior(DateTime now)
    {
        // 按真实猫咪的作息规律，给不同时段的行为分配不同概率
        // 权重顺序：巡逻 / 理毛 / 玩耍 / 吃饭 / 睡觉 / 发呆
        double[] weights = now.Hour switch
        {
            >= 6 and < 10 => [0.50, 0.20, 0.14, 0.09, 0.02, 0.05],   // 早晨活跃
            >= 10 and < 14 => [0.34, 0.15, 0.10, 0.25, 0.10, 0.06],  // 中午干饭+小憩
            >= 14 and < 18 => [0.28, 0.18, 0.10, 0.08, 0.28, 0.08],  // 下午犯困
            >= 18 and < 22 => [0.42, 0.14, 0.22, 0.13, 0.04, 0.05],  // 傍晚兴奋
            >= 22 or < 2 => [0.20, 0.12, 0.05, 0.06, 0.50, 0.07],    // 夜里犯困
            _ => [0.08, 0.05, 0.02, 0.02, 0.78, 0.05]                // 凌晨熟睡
        };

        double roll = _random.NextDouble();
        double acc = 0;
        IdleBehavior picked = IdleBehavior.Walk;
        for (int i = 0; i < weights.Length; i++)
        {
            acc += weights[i];
            if (roll < acc)
            {
                picked = (IdleBehavior)i;
                break;
            }
        }

        switch (picked)
        {
            case IdleBehavior.Walk:
                StartWalk();
                break;
            case IdleBehavior.Groom:
                StartAnimation(PetAnimation.Lick);
                _nextBehaviorAt = now.AddSeconds(_random.Next(5, 10));
                break;
            case IdleBehavior.Play:
                StartAnimation(_random.Next(7) switch
                {
                    0 => PetAnimation.Jump,
                    1 => PetAnimation.Squash,
                    2 => PetAnimation.Spin,
                    3 => PetAnimation.Stretch,
                    4 => PetAnimation.Pounce,
                    5 => PetAnimation.Lick,
                    _ => PetAnimation.Shake
                });
                _nextBehaviorAt = now.AddSeconds(_random.Next(4, 9));
                break;
            case IdleBehavior.Eat:
                StartEating(now, false);
                break;
            case IdleBehavior.Sleep:
                StartSleeping(now, false);
                break;
            default:
                _nextBehaviorAt = now.AddSeconds(_random.Next(4, 9));
                break;
        }
    }

    private void StartEating(DateTime now, bool manual)
    {
        CancelWalk();
        _activity = PetActivity.Eating;
        _activityEndsAt = now.AddSeconds(_random.Next(24, 42));
        _currentAnimation = PetAnimation.None;
        _animationWatch.Restart();
        _animationTimer.Start();
        _bubble.HideBubble();
        if (manual || _random.NextDouble() < 0.55)
        {
            _bubble.ShowMessage(
                manual ? "哇，加餐！吧唧吧唧……" : GetRandomPhrase(EatPhrases, ref _lastEatPhraseIndex),
                GetPetScreenBounds(), TopMost, 3400);
        }
        RenderFrame();
    }

    private void StartSleeping(DateTime now, bool manual)
    {
        CancelWalk();
        _activity = PetActivity.Sleeping;
        bool lateNight = now.Hour is >= 22 or < 6;
        _activityEndsAt = now.AddSeconds(lateNight ? _random.Next(240, 480) : _random.Next(90, 220));
        _nextZzzAt = now.AddSeconds(_random.Next(6, 10));
        _currentAnimation = PetAnimation.None;
        _animationWatch.Restart();
        _animationTimer.Start();
        _bubble.HideBubble();
        if (manual || _random.NextDouble() < 0.55)
        {
            _bubble.ShowMessage(
                manual ? "被哄睡了……晚安喵。" : GetRandomPhrase(SleepPhrases, ref _lastSleepPhraseIndex),
                GetPetScreenBounds(), TopMost, 3400);
        }
        RenderFrame();
    }

    private void UpdateActivity(DateTime now)
    {
        if (now >= _activityEndsAt)
        {
            EndActivity(now, true);
            return;
        }

        if (_activity == PetActivity.Sleeping && now >= _nextZzzAt)
        {
            _nextZzzAt = now.AddSeconds(_random.Next(9, 16));
            if (_random.NextDouble() < 0.55 && !_bubble.Visible)
            {
                _bubble.ShowMessage(GetRandomPhrase(ZzzPhrases, ref _lastZzzPhraseIndex), GetPetScreenBounds(), TopMost, 2600);
            }
        }
    }

    private void EndActivity(DateTime now, bool natural)
    {
        PetActivity ended = _activity;
        _activity = PetActivity.None;
        _animationTimer.Stop();
        _nextBehaviorAt = now.AddSeconds(_random.Next(4, 9));
        if (natural && ended == PetActivity.Sleeping)
        {
            _currentAnimation = PetAnimation.Squash;  // 睡醒伸个懒腰
            _animationWatch.Restart();
            _animationTimer.Start();
            if (_random.NextDouble() < 0.6)
            {
                _bubble.ShowMessage(GetRandomPhrase(WakePhrases, ref _lastWakePhraseIndex), GetPetScreenBounds(), TopMost, 3400);
            }
        }
        else if (natural && ended == PetActivity.Eating && _random.NextDouble() < 0.5)
        {
            _currentAnimation = PetAnimation.Lick;    // 吃饱舔舔嘴
            _animationWatch.Restart();
            _animationTimer.Start();
            _bubble.ShowMessage("吃饱了，满足喵。", GetPetScreenBounds(), TopMost, 3200);
        }
        RenderFrame();
    }

    private void StartWalk()
    {
        Rectangle workArea = Screen.FromHandle(Handle).WorkingArea;
        int minX = workArea.Left + 8;
        int maxX = workArea.Right - Width - 8;
        if (maxX <= minX + 40)
        {
            _nextBehaviorAt = DateTime.Now.AddSeconds(5);
            return;
        }

        int direction;
        if (Location.X - minX < 150)
        {
            direction = 1;
        }
        else if (maxX - Location.X < 150)
        {
            direction = -1;
        }
        else
        {
            direction = _random.Next(2) == 0 ? -1 : 1;
        }

        int distance = _random.Next(130, 421);
        int targetX = Math.Clamp(Location.X + direction * distance, minX, maxX);
        if (Math.Abs(targetX - Location.X) < 45)
        {
            direction *= -1;
            targetX = Math.Clamp(Location.X + direction * distance, minX, maxX);
        }
        if (Math.Abs(targetX - Location.X) < 30)
        {
            _nextBehaviorAt = DateTime.Now.AddSeconds(4);
            return;
        }

        _bubble.HideBubble();
        _walkStartLocation = Location;
        _walkTargetX = targetX;
        _facingLeft = targetX < Location.X;
        float pixelsPerSecond = _random.Next(52, 80);
        _walkDurationMilliseconds = Math.Max(700, (int)(Math.Abs(targetX - Location.X) / pixelsPerSecond * 1000));
        _isWalking = true;
        _walkWatch.Restart();
    }

    private void UpdateWalk(DateTime now)
    {
        float t = Math.Clamp((float)(_walkWatch.Elapsed.TotalMilliseconds / _walkDurationMilliseconds), 0f, 1f);
        float eased = EaseInOutSine(t);
        int x = (int)Math.Round(Lerp(_walkStartLocation.X, _walkTargetX, eased));
        Location = new Point(x, _walkStartLocation.Y);
        // 走路时保持悬浮在最上层，避免被其他窗口盖住
        ReassertTopMost();
        // 四足交替步态：每 175ms 一个半拍，四帧循环，四肢轮流抬放
        float stepPhase = (float)((_walkWatch.Elapsed.TotalMilliseconds / 175.0) % 1.0);
        _walkBobOffsetY = (float)Math.Sin(stepPhase * Math.PI) * 1.8f;
        _walkLateralOffset = (float)Math.Sin(stepPhase * Math.PI * 2.0) * 1.4f;
        RenderFrame();

        if (t >= 1f)
        {
            _isWalking = false;
            _walkBobOffsetY = 0f;
            _walkLateralOffset = 0f;
            ReassertTopMost();
            _nextBehaviorAt = now.AddSeconds(_random.Next(3, 8));
            RenderFrame();
            if (_random.NextDouble() < 0.18)
            {
                _bubble.ShowMessage(GetRandomPhrase(WalkPhrases, ref _lastWalkPhraseIndex), GetPetScreenBounds(), TopMost, 3200);
            }
        }
    }

    private void ReassertTopMost()
    {
        if (!TopMost || !IsHandleCreated)
        {
            return;
        }
        if (!Visible)
        {
            Show();
        }
        const int hwndTopmost = -1;
        const int swpNoMove = 0x0002;
        const int swpNoSize = 0x0001;
        const int swpNoActivate = 0x0010;
        const int swpShowWindow = 0x0040;
        NativeMethods.SetWindowPos(
            Handle,
            (IntPtr)hwndTopmost,
            0, 0, 0, 0,
            swpNoMove | swpNoSize | swpNoActivate | swpShowWindow);
    }

    private void CancelWalk()
    {
        if (_isWalking)
        {
            _isWalking = false;
            _walkBobOffsetY = 0f;
            if (IsHandleCreated)
            {
                RenderFrame();
            }
        }
        if (!_isResting)
        {
            _nextBehaviorAt = DateTime.Now.AddSeconds(3);
        }
    }

    private void CheckBreakReminder(DateTime now)
    {
        if (!_breakReminderEnabled && !_isResting)
        {
            return;
        }

        if (!_isResting && now >= _nextBreakAt)
        {
            StartRestReminder(now, false);
        }
        else if (_isResting && now >= _restEndsAt)
        {
            FinishRest(now);
        }
    }

    private void StartRestReminder(DateTime now, bool manuallyStarted)
    {
        CancelWalk();
        _isResting = true;
        _restEndsAt = now.AddMinutes(RestDurationMinutes);
        int workedMinutes = Math.Max(1, (int)Math.Round((now - _sessionStartedAt).TotalMinutes));
        string message = manuallyStarted
            ? $"现在 {now:HH:mm}。主动休息是很棒的决定，放松 {RestDurationMinutes} 分钟吧！"
            : $"现在 {now:HH:mm}，你已经连续使用电脑约 {workedMinutes} 分钟啦。休息 {RestDurationMinutes} 分钟，看看远处、活动一下肩膀吧！";
        StartAnimation(PetAnimation.Jump);
        _bubble.ShowMessage(message, GetPetScreenBounds(), TopMost, 12000);
        SystemSounds.Asterisk.Play();
    }

    private void FinishRest(DateTime now)
    {
        _isResting = false;
        _sessionStartedAt = now;
        _nextBreakAt = now.AddMinutes(_breakIntervalMinutes);
        _nextBehaviorAt = now.AddSeconds(8);
        _nextProactiveTalkAt = now.AddMinutes(_random.Next(2, 5));
        StartAnimation(PetAnimation.Jump);
        _bubble.ShowMessage($"休息时间到啦，现在是 {now:HH:mm}。精神满满地回来吧！", GetPetScreenBounds(), TopMost, 7000);
        SystemSounds.Asterisk.Play();
    }

    private void SpeakProactively(DateTime now)
    {
        string message = _random.NextDouble() < 0.48
            ? GetTimeAwarePhrase(now)
            : GetRandomPhrase(ProactivePhrases, ref _lastProactivePhraseIndex);
        _bubble.ShowMessage(message, GetPetScreenBounds(), TopMost, 5200);
        _nextProactiveTalkAt = now.AddMinutes(_random.Next(3, 7));
    }

    private static string GetTimeAwarePhrase(DateTime now)
    {
        return now.Hour switch
        {
            >= 6 and < 11 => $"现在是 {now:HH:mm}。早上好，先喝口水，再从容开始吧。",
            >= 11 and < 14 => $"现在是 {now:HH:mm}。快到午饭时间了，别饿着肚子工作。",
            >= 14 and < 18 => $"现在是 {now:HH:mm}。下午容易犯困，记得眨眼和伸展一下。",
            >= 18 and < 22 => $"现在是 {now:HH:mm}。辛苦一天了，记得给眼睛放个假。",
            _ => $"现在已经 {now:HH:mm} 了。夜深啦，猫猫建议早点休息。"
        };
    }

    private void OpenAiChatWindow()
    {
        CancelWalk();
        if (_aiChatWindow is null || _aiChatWindow.IsDisposed)
        {
            _aiChatWindow = new AiChatWindow
            {
                TopMost = TopMost
            };
            _aiChatWindow.FormClosed += (_, _) => _aiChatWindow = null;
        }
        if (!_aiChatWindow.Visible)
        {
            _aiChatWindow.Show(this);
        }
        _aiChatWindow.WindowState = FormWindowState.Normal;
        _aiChatWindow.BringToFront();
        _aiChatWindow.Activate();
    }

    private void OpenChatDialog()
    {
        CancelWalk();
        using var dialog = new ChatDialog
        {
            TopMost = TopMost
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string reply = BuildChatReply(dialog.Message);
        _bubble.ShowMessage(reply, GetPetScreenBounds(), TopMost, 6200);
        _nextBehaviorAt = DateTime.Now.AddSeconds(_random.Next(4, 8));
        RenderFrame();
    }

    private string BuildChatReply(string userText)
    {
        string text = userText.Trim();
        DateTime now = DateTime.Now;
        if (text.Length == 0)
        {
            return "你好像还没说话，不过我已经感受到你的召唤了。";
        }
        if (ContainsAny(text, "你好", "在吗", "hi", "hello", "嗨"))
        {
            return $"你好呀！现在是 {now:HH:mm}，我一直都在桌面上陪你。";
        }
        if (ContainsAny(text, "几点", "时间", "现在"))
        {
            return GetTimeAwarePhrase(now);
        }
        if (ContainsAny(text, "累", "困", "休息", "眼睛酸"))
        {
            return $"辛苦啦。下次完整休息提醒约在 {_nextBreakAt:HH:mm}，现在也可以先闭眼放松一分钟。";
        }
        if (ContainsAny(text, "提醒", "多久"))
        {
            return _breakReminderEnabled
                ? $"我会每 {_breakIntervalMinutes} 分钟提醒你一次，下次约在 {_nextBreakAt:HH:mm}。"
                : "休息提醒现在是关闭的，你可以在右键菜单里重新打开。";
        }
        if (ContainsAny(text, "工作", "学习", "任务"))
        {
            return "把任务拆成一小块一小块，做完一块就夸夸自己。猫猫负责监督和鼓励！";
        }
        if (ContainsAny(text, "饿", "吃饭", "午饭", "晚饭"))
        {
            return "干饭也是正经事。先补充能量，回来后效率会更高喵。";
        }
        if (ContainsAny(text, "谢谢", "感谢", "乖"))
        {
            StartAnimation(PetAnimation.Jump);
            return "不客气！被你一夸，我都想跳起来啦。";
        }
        if (ContainsAny(text, "再见", "拜拜", "晚安"))
        {
            return "再见啦，我会乖乖守在桌面上。记得早点休息。";
        }
        if (ContainsAny(text, "名字", "你是谁"))
        {
            return "我是你的像素黑猫桌宠，负责巡逻桌面、陪你聊天和提醒休息。";
        }
        return GetRandomPhrase(ChatFallbacks, ref _lastChatFallbackIndex);
    }

    private static bool ContainsAny(string text, params string[] keywords)
    {
        return keywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static float EaseInOutSine(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return -((float)Math.Cos(Math.PI * t) - 1f) / 2f;
    }

    private int GetEatingFrame()
    {
        // 埋头吃为主，偶尔抬头舔嘴，更自然的三帧循环
        int phase = (int)(_animationWatch.Elapsed.TotalMilliseconds / 450.0) % 9;
        if (phase == 3) return 3;  // 抬头
        if (phase == 6) return 2;  // 舔嘴
        return 1;  // 埋头吃
    }

    private Bitmap GetCurrentSprite()
    {
        if (_activity == PetActivity.Eating)
        {
            int frame = GetEatingFrame();
            return frame switch
            {
                2 => _eatSprite2,
                3 => _eatSprite3,
                _ => _eatSprite1
            };
        }
        if (_activity == PetActivity.Sleeping)
        {
            return _sleepSprite;
        }
        if (_isWalking)
        {
            // 八帧平滑步态循环，帧率降低使动作更自然
            int frame = (int)(_walkWatch.Elapsed.TotalMilliseconds / 140.0) % 8;
            return frame switch
            {
                0 => _walkSprite1,
                1 => _walkSprite3,
                2 => _walkSprite4,
                3 => _walkSprite2,
                4 => _walkSprite5,
                5 => _walkSprite6,
                6 => _walkSprite2,
                _ => _walkSprite7
            };
        }
        if (_currentAnimation == PetAnimation.Lick)
        {
            // 舔毛：三帧循环，头部左右摆动更自然
            int phase = (int)(_animationWatch.Elapsed.TotalMilliseconds / 280.0) % 5;
            return phase switch
            {
                0 => _lickSprite1,
                1 => _lickSprite3,
                2 => _lickSprite2,
                3 => _lickSprite3,
                _ => _lickSprite1
            };
        }
        return _idleSprite;
    }

    private byte[] GetCurrentAlpha()
    {
        if (_activity == PetActivity.Eating)
        {
            int frame = GetEatingFrame();
            return frame switch
            {
                2 => _eatAlpha2,
                3 => _eatAlpha3,
                _ => _eatAlpha1
            };
        }
        if (_activity == PetActivity.Sleeping)
        {
            return _sleepAlpha;
        }
        if (_isWalking)
        {
            int frame = (int)(_walkWatch.Elapsed.TotalMilliseconds / 140.0) % 8;
            return frame switch
            {
                0 => _walkAlpha1,
                1 => _walkAlpha3,
                2 => _walkAlpha4,
                3 => _walkAlpha2,
                4 => _walkAlpha5,
                5 => _walkAlpha6,
                6 => _walkAlpha2,
                _ => _walkAlpha7
            };
        }
        if (_currentAnimation == PetAnimation.Lick)
        {
            int phase = (int)(_animationWatch.Elapsed.TotalMilliseconds / 280.0) % 5;
            return phase switch
            {
                0 => _lickAlpha1,
                1 => _lickAlpha3,
                2 => _lickAlpha2,
                3 => _lickAlpha3,
                _ => _lickAlpha1
            };
        }
        return _idleAlpha;
    }

    private (float OffsetX, float OffsetY, float ScaleX, float ScaleY) GetAnimationState()
    {
        if (_activity == PetActivity.Sleeping)
        {
            // 睡着后缓慢的呼吸起伏
            double seconds = _animationWatch.Elapsed.TotalSeconds;
            float breath = (float)Math.Sin(seconds * Math.PI / 1.9) * 0.5f + 0.5f; // 约 3.8 秒一次呼吸
            float scaleY = 1f + 0.028f * breath;
            return (0, 0, 1f - 0.014f * breath, scaleY);
        }
        if (_activity == PetActivity.Eating)
        {
            // 吃饭：更自然的埋头-抬头-舔嘴动作
            double seconds = _animationWatch.Elapsed.TotalSeconds;
            int phase = (int)(_animationWatch.Elapsed.TotalMilliseconds / 450.0) % 9;
            if (phase == 3)
            {
                // 抬头
                float rise = (float)Math.Sin(Math.PI * ((_animationWatch.Elapsed.TotalMilliseconds / 450.0) % 1.0));
                return (0, -4.5f * rise, 1f + 0.02f * rise, 1f - 0.02f * rise);
            }
            if (phase == 6)
            {
                // 舔嘴：轻微左右摆动
                float lick = (float)Math.Sin(seconds * Math.PI * 4.0) * 1.5f;
                return (lick, -2f, 1f, 1f);
            }
            // 埋头吃：前后点头
            float nod = -(float)Math.Abs(Math.Sin(seconds * Math.PI * 1.8)) * 3.0f;
            float bodyBob = (float)Math.Sin(seconds * Math.PI * 1.8) * 0.8f;
            return (0, nod + bodyBob, 1f + 0.01f * (float)Math.Sin(seconds * Math.PI * 3.6), 1f);
        }
        if (_currentAnimation == PetAnimation.None)
        {
            if (_isWalking)
            {
                // 走路：身体随四肢交替轻微左右摇摆 + 脚步触地时下沉回弹
                float squatAmt = (float)Math.Abs(Math.Sin(((double)_walkWatch.Elapsed.TotalMilliseconds / 140.0 % 1.0) * Math.PI));
                float stretchX = 1f + 0.022f * squatAmt;
                float stretchY = 1f - 0.022f * squatAmt;
                float lateralTilt = (float)Math.Sin(((double)_walkWatch.Elapsed.TotalMilliseconds / 280.0) * Math.PI * 2.0) * 1.5f;
                return (_walkLateralOffset + lateralTilt, _walkBobOffsetY, stretchX, stretchY);
            }
            // 待机时保持轻微呼吸起伏与偶发抖动，让猫显得有生命
            double ms = Environment.TickCount;
            float breath = (float)Math.Sin(ms / 900.0) * 0.004f;
            float restX = _isResting ? 0f : (float)Math.Sin(ms / 5000.0) * 1.6f;
            return (restX, -(float)Math.Abs(Math.Sin((ms % 74000) / 74000.0 * Math.PI)) * 0.6f, 1f + breath, 1f - breath);
        }

        float duration = _currentAnimation switch
        {
            PetAnimation.Jump => 720f,
            PetAnimation.Squash => 780f,
            PetAnimation.Shake => 680f,
            PetAnimation.Lick => 3000f,
            PetAnimation.Spin => 1100f,
            PetAnimation.Stretch => 1600f,
            PetAnimation.Pounce => 900f,
            _ => 1f
        };
        float t = Math.Clamp((float)(_animationWatch.Elapsed.TotalMilliseconds / duration), 0f, 1f);

        switch (_currentAnimation)
        {
            case PetAnimation.Jump:
            {
                float arc = (float)Math.Sin(Math.PI * t);
                float jumpHeight = Math.Max(34f, _petHeight * 0.30f);
                return (0, -jumpHeight * arc, 1f - 0.035f * arc, 1f + 0.07f * arc);
            }
            case PetAnimation.Squash:
            {
                if (t < 0.26f)
                {
                    float u = EaseOutCubic(t / 0.26f);
                    return (0, 0, Lerp(1f, 1.30f, u), Lerp(1f, 0.72f, u));
                }
                if (t < 0.58f)
                {
                    float u = EaseOutBack((t - 0.26f) / 0.32f);
                    return (0, 0, Lerp(1.30f, 0.88f, u), Lerp(0.72f, 1.15f, u));
                }
                float settle = (t - 0.58f) / 0.42f;
                float wobble = (float)Math.Sin(settle * Math.PI * 2.2) * (1f - settle) * 0.055f;
                return (0, 0, Lerp(0.88f, 1f, EaseOutCubic(settle)) - wobble,
                    Lerp(1.15f, 1f, EaseOutCubic(settle)) + wobble);
            }
            case PetAnimation.Lick:
            {
                // 舔毛：头部左右倾斜 + 点头，更自然的舔毛动作
                float headTilt = (float)Math.Sin(t * Math.PI * 6.0) * 4.0f;
                float nod = (float)Math.Sin(t * Math.PI * 4.0) * 2.5f;
                float stretch = 1f + (float)Math.Sin(t * Math.PI * 8.0) * 0.025f;
                float settle = (float)Math.Sin(t * Math.PI) * 0.02f;
                return (headTilt, nod, stretch + settle, 1f - settle);
            }
            case PetAnimation.Shake:
            {
                float envelope = 1f - t;
                float amplitude = Math.Max(8f, _petWidth * 0.055f);
                float x = (float)Math.Sin(t * Math.PI * 9.0) * amplitude * envelope;
                float stretch = 1f + 0.035f * (float)Math.Sin(t * Math.PI * 18.0) * envelope;
                return (x, 0, stretch, 1f / stretch);
            }
            case PetAnimation.Spin:
            {
                // 原地转圈：左右位移 + 横向拉伸/压缩产生旋转感
                float wave = (float)Math.Sin(t * Math.PI * 4.0);
                float x = wave * Math.Max(6f, _petWidth * 0.05f) * (1f - t * 0.3f);
                float squash = 1f - 0.22f * (float)Math.Sin(t * Math.PI * 2.0) * (1f - t * 0.5f);
                return (x, -Math.Abs(wave) * 5f, squash, 2f - squash);
            }
            case PetAnimation.Stretch:
            {
                // 伸懒腰：先压扁再向上拉伸，带轻微前倾
                if (t < 0.35f)
                {
                    float u = EaseOutCubic(t / 0.35f);
                    return (0, 0, Lerp(1f, 0.82f, u), Lerp(1f, 1.25f, u));
                }
                float v = (t - 0.35f) / 0.65f;
                float rise = (float)Math.Sin(Math.PI * Math.Clamp(v, 0f, 1f));
                float linger = 0.65f + 0.08f * (float)Math.Sin(Math.PI * v);
                float scale = Lerp(1f, linger, (float)Math.Clamp(v * 2.4f, 0f, 1f));
                return (0, -_petHeight * 0.12f * rise, scale, 1f / scale);
            }
            case PetAnimation.Pounce:
            {
                // 扑击：先蹲下蓄力，再向前上方扑出，带落地压扁
                if (t < 0.30f)
                {
                    float u = EaseOutCubic(t / 0.30f);
                    return (0, 0, Lerp(1f, 1.35f, u), Lerp(1f, 0.66f, u));
                }
                float paw = (t - 0.30f) / 0.45f;
                float arc = (float)Math.Sin(Math.PI * Math.Clamp(paw, 0f, 1f));
                float forward = arc * Math.Max(16f, _petWidth * 0.22f);
                float up = -arc * Math.Max(22f, _petHeight * 0.24f);
                return (forward * (_facingLeft ? 1f : -1f), up, 1f - 0.03f * arc, 1f + 0.09f * arc);
            }
            default:
                return (0, 0, 1, 1);
        }
    }

    private static float Lerp(float from, float to, float amount) => from + (to - from) * amount;

    private static float EaseOutCubic(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return 1f - (float)Math.Pow(1f - t, 3);
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        t = Math.Clamp(t, 0f, 1f);
        return 1f + c3 * (float)Math.Pow(t - 1f, 3) + c1 * (float)Math.Pow(t - 1f, 2);
    }

    private void RenderFrame()
    {
        if (!IsHandleCreated || Width <= 0 || Height <= 0)
        {
            return;
        }

        if (_currentAnimation != PetAnimation.None)
        {
            float duration = _currentAnimation switch
            {
                PetAnimation.Jump => 720f,
                PetAnimation.Squash => 780f,
                PetAnimation.Shake => 680f,
                PetAnimation.Lick => 3000f,
                PetAnimation.Spin => 1100f,
                PetAnimation.Stretch => 1600f,
                PetAnimation.Pounce => 900f,
                _ => 1f
            };
            if (_animationWatch.Elapsed.TotalMilliseconds >= duration)
            {
                _currentAnimation = PetAnimation.None;
                _animationTimer.Stop();
            }
        }

        var (offsetX, offsetY, scaleX, scaleY) = GetAnimationState();
        Bitmap currentSprite = GetCurrentSprite();
        using var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.None;

            float anchorX = _baseRect.X + _baseRect.Width / 2f + offsetX;
            float anchorY = _baseRect.Bottom + offsetY;
            using var transform = new Matrix();
            transform.Translate(anchorX, anchorY, MatrixOrder.Append);
            // 基础精灵面朝左，面朝左时不翻转，面朝右时水平翻转。
            transform.Scale(_facingLeft ? scaleX : -scaleX, scaleY, MatrixOrder.Append);
            graphics.Transform = transform;

            using var inverse = transform.Clone();
            inverse.Invert();
            _hitInverse = inverse.Elements;

            graphics.DrawImage(
                currentSprite,
                new RectangleF(-_baseRect.Width / 2f, -_baseRect.Height, _baseRect.Width, _baseRect.Height));
            graphics.ResetTransform();
        }

        SetBitmap(bitmap);
    }

    protected override void WndProc(ref Message message)
    {
        const int wmNchitTest = 0x0084;
        const int htTransparent = -1;
        const int htClient = 1;

        if (message.Msg == wmNchitTest)
        {
            int lParam = message.LParam.ToInt32();
            var screenPoint = new Point((short)(lParam & 0xFFFF), (short)((lParam >> 16) & 0xFFFF));
            Point clientPoint = PointToClient(screenPoint);
            message.Result = IsSpritePixelOpaque(clientPoint) ? new IntPtr(htClient) : new IntPtr(htTransparent);
            return;
        }

        base.WndProc(ref message);
    }

    private bool IsSpritePixelOpaque(Point clientPoint)
    {
        if (_hitInverse is null || _hitInverse.Length < 6)
        {
            return false;
        }

        float m11 = _hitInverse[0];
        float m12 = _hitInverse[1];
        float m21 = _hitInverse[2];
        float m22 = _hitInverse[3];
        float dx = _hitInverse[4];
        float dy = _hitInverse[5];

        float x = clientPoint.X * m11 + clientPoint.Y * m21 + dx;
        float y = clientPoint.X * m12 + clientPoint.Y * m22 + dy;
        float localX = x + _baseRect.Width / 2f;
        float localY = y + _baseRect.Height;

        if (localX < 0 || localY < 0 || localX >= _baseRect.Width || localY >= _baseRect.Height)
        {
            return false;
        }

        int sourceX = Math.Clamp((int)(localX / _baseRect.Width * _spriteWidth), 0, _spriteWidth - 1);
        int sourceY = Math.Clamp((int)(localY / _baseRect.Height * _spriteHeight), 0, _spriteHeight - 1);
        return GetCurrentAlpha()[sourceY * _spriteWidth + sourceX] >= 48;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _animationTimer.Stop();
        _behaviorTimer.Stop();
        _bubble.Close();
        _menu.Dispose();
        _animationTimer.Dispose();
        _behaviorTimer.Dispose();
        _idleSprite.Dispose();
        _lickSprite1.Dispose();
        _lickSprite2.Dispose();
        _lickSprite3.Dispose();
        _walkSprite1.Dispose();
        _walkSprite2.Dispose();
        _walkSprite3.Dispose();
        _walkSprite4.Dispose();
        _walkSprite5.Dispose();
        _walkSprite6.Dispose();
        _walkSprite7.Dispose();
        _walkSprite8.Dispose();
        _eatSprite1.Dispose();
        _eatSprite2.Dispose();
        _eatSprite3.Dispose();
        _sleepSprite.Dispose();
        _aiChatWindow?.Close();
        base.OnFormClosing(e);
    }
}

internal sealed class ChatDialog : Form
{
    private readonly TextBox _inputTextBox;

    public string Message => _inputTextBox.Text.Trim();

    public ChatDialog()
    {
        Text = "和猫猫聊天";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(430, 154);
        Font = new Font("Microsoft YaHei UI", 10f, FontStyle.Regular, GraphicsUnit.Point);

        var promptLabel = new Label
        {
            AutoSize = true,
            Location = new Point(18, 16),
            Text = "想对猫猫说什么？"
        };

        _inputTextBox = new TextBox
        {
            Location = new Point(18, 48),
            Size = new Size(394, 30)
        };

        var okButton = new Button
        {
            DialogResult = DialogResult.OK,
            Location = new Point(236, 100),
            Size = new Size(84, 34),
            Text = "发送"
        };
        var cancelButton = new Button
        {
            DialogResult = DialogResult.Cancel,
            Location = new Point(328, 100),
            Size = new Size(84, 34),
            Text = "取消"
        };

        Controls.Add(promptLabel);
        Controls.Add(_inputTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _inputTextBox.Focus();
        _inputTextBox.SelectAll();
    }
}

internal sealed class OffWorkTimeDialog : Form
{
    private readonly TextBox _timeTextBox;

    public string Value => _timeTextBox.Text.Trim();

    public OffWorkTimeDialog(string currentTime)
    {
        Text = "设置下班时间";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(300, 150);
        Font = new Font("Microsoft YaHei UI", 10f, FontStyle.Regular, GraphicsUnit.Point);

        var promptLabel = new Label
        {
            AutoSize = true,
            Location = new Point(18, 18),
            Text = "请输入下班时间（HH:mm）："
        };

        _timeTextBox = new TextBox
        {
            Location = new Point(18, 52),
            Size = new Size(180, 30),
            Text = currentTime
        };

        var okButton = new Button
        {
            DialogResult = DialogResult.OK,
            Location = new Point(122, 96),
            Size = new Size(76, 34),
            Text = "确定"
        };
        var cancelButton = new Button
        {
            DialogResult = DialogResult.Cancel,
            Location = new Point(206, 96),
            Size = new Size(76, 34),
            Text = "取消"
        };

        Controls.Add(promptLabel);
        Controls.Add(_timeTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _timeTextBox.Focus();
        _timeTextBox.SelectAll();
    }
}

internal sealed class ApiProviderPreset
{
    public string Name { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string? ModelListUrl { get; init; }
    public string? ApiKeyUrl { get; init; }
    public string ApiKeyHint { get; init; } = "API Key / Token";
}

internal sealed class ApiProviders
{
    public static readonly ApiProviderPreset[] Presets =
    [
        new ApiProviderPreset
        {
            Name = "OpenAI",
            BaseUrl = "https://api.openai.com/v1",
            Model = "gpt-4o-mini",
            ApiKeyUrl = "https://platform.openai.com/api-keys",
            ApiKeyHint = "API Key"
        },
        new ApiProviderPreset
        {
            Name = "DeepSeek",
            BaseUrl = "https://api.deepseek.com/v1",
            Model = "deepseek-chat",
            ApiKeyUrl = "https://platform.deepseek.com/api_keys",
            ApiKeyHint = "API Key"
        },
        new ApiProviderPreset
        {
            Name = "通义千问 (阿里云)",
            BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1",
            Model = "qwen-plus",
            ApiKeyUrl = "https://dashscope.console.aliyun.com/apiKey",
            ApiKeyHint = "API Key"
        },
        new ApiProviderPreset
        {
            Name = "月之暗面 (Kimi)",
            BaseUrl = "https://api.moonshot.cn/v1",
            Model = "moonshot-v1-8k",
            ApiKeyUrl = "https://platform.moonshot.cn/console/api-keys",
            ApiKeyHint = "API Key"
        },
        new ApiProviderPreset
        {
            Name = "智谱 AI (GLM)",
            BaseUrl = "https://open.bigmodel.cn/api/paas/v4",
            Model = "glm-4-flash",
            ApiKeyUrl = "https://open.bigmodel.cn/usercenter/apikeys",
            ApiKeyHint = "API Key"
        },
        new ApiProviderPreset
        {
            Name = "零一万物 (Yi)",
            BaseUrl = "https://api.lingyiwanwu.com/v1",
            Model = "yi-lightning",
            ApiKeyUrl = "https://platform.lingyiwanwu.com/apikeys",
            ApiKeyHint = "API Key"
        },
        new ApiProviderPreset
        {
            Name = "魔搭 ModelScope",
            BaseUrl = "https://api-inference.modelscope.cn/v1",
            Model = "deepseek-ai/DeepSeek-V4-Flash-0731",
            ApiKeyUrl = "https://modelscope.cn/my/myaccesstoken",
            ApiKeyHint = "Access Token"
        },
        new ApiProviderPreset
        {
            Name = "Ollama (本地)",
            BaseUrl = "http://localhost:11434/v1",
            Model = "qwen2.5:7b",
            ApiKeyHint = "留空即可"
        },
        new ApiProviderPreset
        {
            Name = "自定义 (OpenAI 兼容)",
            BaseUrl = "",
            Model = "",
            ApiKeyHint = "API Key"
        }
    ];

    public static ApiProviderPreset? FindByName(string name)
    {
        return Array.Find(Presets, p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}

internal sealed class ChatSettings
{
    public string ProviderName { get; set; } = "DeepSeek";
    public string BaseUrl { get; set; } = "https://api.deepseek.com/v1";
    public string Model { get; set; } = "deepseek-chat";
    public string ApiKey { get; set; } = string.Empty;

    public static readonly string[] CandidateModels =
    [
        "deepseek-chat",
        "deepseek-reasoner"
    ];

    public static readonly string[] RetiredModels = [];
}

internal sealed class ChatMessageRecord
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.Now;
    public bool IsLocalError { get; set; }
}

internal sealed class ChatSessionRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "新的聊天";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<ChatMessageRecord> Messages { get; set; } = [];

    [JsonIgnore]
    public string DisplayTitle => $"{Title}  ·  {UpdatedAt:MM-dd HH:mm}";
}

internal sealed class ChatHistoryStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _settingsFile;
    private readonly string _historyFile;

    public ChatHistoryStore()
    {
        string appDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PixelCatDesktopPet");
        Directory.CreateDirectory(appDirectory);
        _settingsFile = Path.Combine(appDirectory, "modelscope-settings.json");
        _historyFile = Path.Combine(appDirectory, "chat-history.json");
    }

    public ChatSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFile))
            {
                ChatSettings settings = JsonSerializer.Deserialize<ChatSettings>(File.ReadAllText(_settingsFile)) ?? new ChatSettings();
                // 已下线的 DeepSeek 模型自动迁移到当前可用的默认模型。
                if (ChatSettings.RetiredModels.Any(m => string.Equals(settings.Model, m, StringComparison.OrdinalIgnoreCase)))
                {
                    settings.Model = new ChatSettings().Model;
                }
                // 旧版魔搭配置自动迁移到 DeepSeek。
                if (string.IsNullOrEmpty(settings.ProviderName) &&
                    settings.BaseUrl.Contains("modelscope.cn", StringComparison.OrdinalIgnoreCase))
                {
                    settings.ProviderName = "DeepSeek";
                    settings.BaseUrl = "https://api.deepseek.com/v1";
                    settings.Model = "deepseek-chat";
                }
                return settings;
            }
        }
        catch
        {
            // A damaged local configuration should not prevent the desktop pet from starting.
        }
        return new ChatSettings();
    }

    public void SaveSettings(ChatSettings settings)
    {
        File.WriteAllText(_settingsFile, JsonSerializer.Serialize(settings, JsonOptions));
    }

    public List<ChatSessionRecord> LoadSessions()
    {
        try
        {
            if (File.Exists(_historyFile))
            {
                return JsonSerializer.Deserialize<List<ChatSessionRecord>>(File.ReadAllText(_historyFile)) ?? [];
            }
        }
        catch
        {
            string backupFile = _historyFile + ".broken-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                File.Copy(_historyFile, backupFile, true);
            }
            catch
            {
                // Ignore backup failures; a fresh history file will be created.
            }
        }
        return [];
    }

    public void SaveSessions(IEnumerable<ChatSessionRecord> sessions)
    {
        File.WriteAllText(_historyFile, JsonSerializer.Serialize(sessions, JsonOptions));
    }
}

internal sealed class ChatSettingsDialog : Form
{
    private readonly ComboBox _providerComboBox;
    private readonly TextBox _baseUrlTextBox;
    private readonly ComboBox _modelComboBox;
    private readonly TextBox _apiKeyTextBox;
    private readonly Button _detectButton;
    private readonly Label _detectStatusLabel;
    private readonly LinkLabel _apiKeyLink;
    private readonly ModelScopeChatClient _client = new();
    private bool _isUpdatingFields;

    public ChatSettings Result { get; private set; }

    public ChatSettingsDialog(ChatSettings settings)
    {
        Result = new ChatSettings
        {
            ProviderName = settings.ProviderName,
            BaseUrl = settings.BaseUrl,
            Model = settings.Model,
            ApiKey = settings.ApiKey
        };

        Text = "AI 聊天设置";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(580, 360);
        Font = new Font("Microsoft YaHei UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point);

        var providerLabel = new Label { AutoSize = true, Location = new Point(20, 20), Text = "服务提供商" };
        _providerComboBox = new ComboBox
        {
            Location = new Point(120, 16),
            Size = new Size(436, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        foreach (ApiProviderPreset preset in ApiProviders.Presets)
        {
            _providerComboBox.Items.Add(preset.Name);
        }
        _providerComboBox.SelectedIndexChanged += (_, _) => OnProviderChanged();
        string? matchedName = Array.Find(ApiProviders.Presets, p =>
            string.Equals(p.Name, settings.ProviderName, StringComparison.OrdinalIgnoreCase))?.Name;
        _providerComboBox.SelectedItem = matchedName ?? "自定义 (OpenAI 兼容)";

        var baseUrlLabel = new Label { AutoSize = true, Location = new Point(20, 66), Text = "Base URL" };
        _baseUrlTextBox = new TextBox { Location = new Point(120, 62), Size = new Size(436, 30), Text = Result.BaseUrl };
        var modelLabel = new Label { AutoSize = true, Location = new Point(20, 112), Text = "模型 ID" };
        _modelComboBox = new ComboBox
        {
            Location = new Point(120, 108),
            Size = new Size(316, 30),
            DropDownStyle = ComboBoxStyle.DropDown,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.ListItems
        };
        _modelComboBox.Text = Result.Model;
        _detectButton = new Button
        {
            Location = new Point(444, 106),
            Size = new Size(112, 32),
            Text = "检测模型"
        };
        _detectButton.Click += async (_, _) => await DetectAvailableModelsAsync();
        _detectStatusLabel = new Label
        {
            AutoSize = false,
            Location = new Point(120, 142),
            Size = new Size(436, 20),
            ForeColor = Color.DimGray,
            Text = string.Empty
        };
        var keyLabel = new Label { AutoSize = true, Location = new Point(20, 178), Text = "API Key" };
        _apiKeyTextBox = new TextBox
        {
            Location = new Point(120, 174),
            Size = new Size(436, 30),
            Text = Result.ApiKey,
            UseSystemPasswordChar = true
        };

        _apiKeyLink = new LinkLabel
        {
            AutoSize = true,
            Location = new Point(120, 214),
            Text = "获取 API Key"
        };
        _apiKeyLink.LinkClicked += (_, _) =>
        {
            string url = "https://platform.openai.com/api-keys";
            ApiProviderPreset? preset = ApiProviders.FindByName(_providerComboBox.SelectedItem?.ToString() ?? "");
            if (preset?.ApiKeyUrl is not null)
            {
                url = preset.ApiKeyUrl;
            }
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        };
        var privacyLabel = new Label
        {
            AutoSize = true,
            ForeColor = Color.DimGray,
            Location = new Point(120, 244),
            Text = "Key 只保存在当前 Windows 用户目录的本地配置文件中。"
        };

        var okButton = new Button
        {
            DialogResult = DialogResult.OK,
            Location = new Point(370, 300),
            Size = new Size(86, 34),
            Text = "保存"
        };
        okButton.Click += (_, _) => SaveResult();
        var cancelButton = new Button
        {
            DialogResult = DialogResult.Cancel,
            Location = new Point(470, 300),
            Size = new Size(86, 34),
            Text = "取消"
        };

        Controls.Add(providerLabel);
        Controls.Add(_providerComboBox);
        Controls.Add(baseUrlLabel);
        Controls.Add(_baseUrlTextBox);
        Controls.Add(modelLabel);
        Controls.Add(_modelComboBox);
        Controls.Add(_detectButton);
        Controls.Add(_detectStatusLabel);
        Controls.Add(keyLabel);
        Controls.Add(_apiKeyTextBox);
        Controls.Add(_apiKeyLink);
        Controls.Add(privacyLabel);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
        AcceptButton = okButton;
        CancelButton = cancelButton;

        OnProviderChanged();
    }

    private void OnProviderChanged()
    {
        if (_isUpdatingFields) return;
        _isUpdatingFields = true;

        string? selectedName = _providerComboBox.SelectedItem?.ToString();
        ApiProviderPreset? preset = selectedName is not null ? ApiProviders.FindByName(selectedName) : null;

        if (preset is not null)
        {
            _baseUrlTextBox.Text = preset.BaseUrl;
            _modelComboBox.Text = preset.Model;
            _apiKeyLink.Text = $"获取 {preset.Name} API Key";
        }

        _isUpdatingFields = false;
    }

    private async Task DetectAvailableModelsAsync()
    {
        string baseUrl = _baseUrlTextBox.Text.Trim().TrimEnd('/');
        string apiKey = _apiKeyTextBox.Text.Trim();
        if (baseUrl.Length == 0)
        {
            SetDetectStatus("请先填写 Base URL，再检测。", Color.DarkRed);
            return;
        }

        _detectButton.Enabled = false;
        SetDetectStatus("正在读取模型列表……", Color.DimGray);
        try
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            List<string> listed = await _client.ListModelIdsAsync(baseUrl, apiKey, cancellation.Token);
            if (listed.Count > 0)
            {
                _modelComboBox.Items.Clear();
                _modelComboBox.Items.AddRange(listed.Cast<object>().ToArray());
                _modelComboBox.Text = listed[0];
                SetDetectStatus($"已获取 {listed.Count} 个模型，已选中第一个。", Color.DarkGreen);
            }
            else
            {
                SetDetectStatus("接口未返回模型列表，请手动填写模型 ID。", Color.DimGray);
            }
        }
        catch (Exception ex)
        {
            SetDetectStatus("检测失败：" + ex.Message, Color.DarkRed);
        }
        finally
        {
            _detectButton.Enabled = true;
        }
    }

    private void SetDetectStatus(string text, Color color)
    {
        _detectStatusLabel.ForeColor = color;
        _detectStatusLabel.Text = text;
    }

    private void SaveResult()
    {
        string baseUrl = _baseUrlTextBox.Text.Trim().TrimEnd('/');
        string model = _modelComboBox.Text.Trim();
        if (baseUrl.Length == 0 || model.Length == 0)
        {
            MessageBox.Show(this, "Base URL 和模型 ID 不能为空。", "设置不完整", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.None;
            return;
        }

        Result = new ChatSettings
        {
            ProviderName = _providerComboBox.SelectedItem?.ToString() ?? "自定义 (OpenAI 兼容)",
            BaseUrl = baseUrl,
            Model = model,
            ApiKey = _apiKeyTextBox.Text.Trim()
        };
    }
}

internal sealed class ModelScopeChatClient
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(120)
    };

    public async Task<string> SendAsync(
        ChatSettings settings,
        IReadOnlyList<ChatMessageRecord> history,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException("还没有填写 API Key。请点击「API 设置」。");
        }

        var messages = new List<object>
        {
            new
            {
                role = "system",
                content = "你是一只住在 Windows 桌面上的像素黑猫桌宠。说话亲切、简洁、自然，可以适度使用“喵”，但不要每句都加。你会陪用户聊天、提醒休息，并给出清晰实用的回答。"
            }
        };
        foreach (ChatMessageRecord message in history.Where(m => !m.IsLocalError).TakeLast(18))
        {
            string role = message.Role == "assistant" ? "assistant" : "user";
            messages.Add(new { role, content = message.Content });
        }

        string endpoint = settings.BaseUrl.TrimEnd('/') + "/chat/completions";

        // API 偶发返回空 choices，这里采用指数退避重试。
        string[] candidateModels = BuildCandidateModels(settings.Model);
        const int maxPerModel = 3;
        string? lastError = null;
        for (int modelIndex = 0; modelIndex < candidateModels.Length; modelIndex++)
        {
            string currentModel = candidateModels[modelIndex];
            var payload = new
            {
                model = currentModel,
                messages,
                temperature = 0.7,
                max_tokens = 1024,
                stream = false
            };

            for (int attempt = 1; attempt <= maxPerModel; attempt++)
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

                using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
                string body = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    string shortBody = body.Length > 700 ? body[..700] + "……" : body;
                    lastError = $"API 请求失败（HTTP {(int)response.StatusCode}）：{shortBody}";
                    if (attempt == maxPerModel)
                    {
                        break;
                    }
                    await Task.Delay(2000 * attempt, cancellationToken);
                    continue;
                }

                string? result = TryExtractReply(body);
                if (result is not null)
                {
                    return result;
                }

                string emptyHint = body.Length > 500 ? body[..500] + "……" : body;
                lastError = $"API 返回了异常内容（没有可用的回复）：{emptyHint}";
                if (attempt == maxPerModel)
                {
                    break;
                }
                await Task.Delay(2000 * attempt + 1500, cancellationToken);
            }

            if (modelIndex < candidateModels.Length - 1)
            {
                // 主模型连续失败，切换备用模型前先等待片刻。
                await Task.Delay(1500, cancellationToken);
            }
        }

        throw new InvalidOperationException(lastError ?? "API 调用失败。");
    }

    private static string[] BuildCandidateModels(string configuredModel)
    {
        var candidates = new List<string>();
        string model = string.IsNullOrWhiteSpace(configuredModel)
            ? "deepseek-chat"
            : configuredModel.Trim();
        candidates.Add(model);

        return candidates.Distinct(StringComparer.Ordinal).ToArray();
    }

    private static string? TryExtractReply(string body)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(body);
            if (!document.RootElement.TryGetProperty("choices", out JsonElement choicesElement) ||
                choicesElement.ValueKind != JsonValueKind.Array ||
                choicesElement.GetArrayLength() == 0)
            {
                return null;
            }
            JsonElement message = choicesElement[0].GetProperty("message");
            if (!message.TryGetProperty("content", out JsonElement content))
            {
                return null;
            }
            string text = ExtractTextContent(content).Trim();
            return text.Length == 0 ? null : text;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task<List<string>> ListModelIdsAsync(
        string baseUrl,
        string apiKey,
        CancellationToken cancellationToken)
    {
        string endpoint = baseUrl.TrimEnd('/') + "/models";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string shortBody = body.Length > 300 ? body[..300] + "……" : body;
            throw new InvalidOperationException($"读取模型列表失败（HTTP {(int)response.StatusCode}）：{shortBody}");
        }

        var ids = new List<string>();
        using JsonDocument document = JsonDocument.Parse(body);
        if (document.RootElement.TryGetProperty("data", out JsonElement data) &&
            data.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in data.EnumerateArray())
            {
                if (item.TryGetProperty("id", out JsonElement id) && id.ValueKind == JsonValueKind.String)
                {
                    string? value = id.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        ids.Add(value);
                    }
                }
            }
        }
        return ids;
    }

    public async Task<bool> ProbeModelAsync(
        string baseUrl,
        string apiKey,
        string modelId,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            model = modelId,
            messages = new object[]
            {
                new { role = "user", content = "你好" }
            },
            max_tokens = 16,
            stream = false
        };
        string endpoint = baseUrl.TrimEnd('/') + "/chat/completions";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        try
        {
            using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            using JsonDocument document = JsonDocument.Parse(body);
            return document.RootElement.TryGetProperty("choices", out JsonElement choices) &&
                   choices.ValueKind == JsonValueKind.Array &&
                   choices.GetArrayLength() > 0;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    private static string ExtractTextContent(JsonElement content)
    {
        if (content.ValueKind == JsonValueKind.String)
        {
            return content.GetString() ?? string.Empty;
        }
        if (content.ValueKind != JsonValueKind.Array)
        {
            return content.ToString();
        }

        var builder = new System.Text.StringBuilder();
        foreach (JsonElement item in content.EnumerateArray())
        {
            if (item.TryGetProperty("text", out JsonElement text))
            {
                builder.Append(text.GetString());
            }
            else if (item.TryGetProperty("content", out JsonElement nested))
            {
                builder.Append(nested.GetString());
            }
        }
        return builder.ToString();
    }
}

internal sealed class AiChatWindow : Form
{
    private readonly ChatHistoryStore _store = new();
    private readonly ModelScopeChatClient _client = new();
    private readonly ListBox _sessionListBox;
    private readonly RichTextBox _historyTextBox;
    private readonly TextBox _inputTextBox;
    private readonly Button _newSessionButton;
    private readonly Button _deleteSessionButton;
    private readonly Button _settingsButton;
    private readonly Button _sendButton;
    private readonly Label _statusLabel;
    private ChatSettings _settings;
    private List<ChatSessionRecord> _sessions;
    private ChatSessionRecord _currentSession;
    private bool _isSending;

    public AiChatWindow()
    {
        Text = "像素黑猫 AI 聊天";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(920, 620);
        MinimumSize = new Size(760, 520);
        Font = new Font("Microsoft YaHei UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point);

        _settings = _store.LoadSettings();
        _sessions = _store.LoadSessions();
        if (_sessions.Count == 0)
        {
            _sessions.Add(new ChatSessionRecord());
        }
        _currentSession = _sessions.OrderByDescending(s => s.UpdatedAt).First();

        var leftPanel = new Panel { Dock = DockStyle.Left, Width = 238, Padding = new Padding(8) };
        _sessionListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            IntegralHeight = false
        };
        _sessionListBox.SelectedIndexChanged += (_, _) => ChangeSelectedSession();
        var leftButtonsPanel = new Panel { Dock = DockStyle.Bottom, Height = 84 };
        _newSessionButton = new Button { Dock = DockStyle.Top, Height = 36, Text = "新建聊天" };
        _newSessionButton.Click += (_, _) => CreateNewSession();
        _deleteSessionButton = new Button { Dock = DockStyle.Bottom, Height = 36, Text = "删除当前聊天" };
        _deleteSessionButton.Click += (_, _) => DeleteCurrentSession();
        leftButtonsPanel.Controls.Add(_newSessionButton);
        leftButtonsPanel.Controls.Add(_deleteSessionButton);
        leftPanel.Controls.Add(_sessionListBox);
        leftPanel.Controls.Add(leftButtonsPanel);

        var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        var topPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
        _settingsButton = new Button { Dock = DockStyle.Right, Width = 110, Text = "API 设置" };
        _settingsButton.Click += (_, _) => OpenSettings();
        _statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.DimGray
        };
        topPanel.Controls.Add(_statusLabel);
        topPanel.Controls.Add(_settingsButton);

        _historyTextBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            DetectUrls = true
        };

        var inputPanel = new Panel { Dock = DockStyle.Bottom, Height = 96, Padding = new Padding(0, 8, 0, 0) };
        _inputTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            AcceptsReturn = true,
            ScrollBars = ScrollBars.Vertical
        };
        _inputTextBox.KeyDown += InputTextBoxKeyDown;
        _sendButton = new Button { Dock = DockStyle.Right, Width = 96, Text = "发送" };
        _sendButton.Click += async (_, _) => await SendCurrentMessageAsync();
        inputPanel.Controls.Add(_inputTextBox);
        inputPanel.Controls.Add(_sendButton);

        rightPanel.Controls.Add(_historyTextBox);
        rightPanel.Controls.Add(inputPanel);
        rightPanel.Controls.Add(topPanel);
        Controls.Add(rightPanel);
        Controls.Add(leftPanel);

        RefreshSessionList();
        RenderCurrentSession();
        UpdateStatus();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _inputTextBox.Focus();
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            SetStatus("请先点击「API 设置」，填写 API Key。", Color.DarkOrange);
        }
    }

    private void UpdateStatus()
    {
        SetStatus($"模型：{_settings.Model}；聊天记录保存在本机。", Color.DimGray);
    }

    private void SetStatus(string text, Color color)
    {
        _statusLabel.Text = text;
        _statusLabel.ForeColor = color;
    }

    private void RefreshSessionList()
    {
        _sessions = _sessions.OrderByDescending(s => s.UpdatedAt).ToList();
        _sessionListBox.DataSource = null;
        _sessionListBox.DisplayMember = nameof(ChatSessionRecord.DisplayTitle);
        _sessionListBox.DataSource = _sessions;
        _sessionListBox.SelectedItem = _currentSession;
    }

    private void ChangeSelectedSession()
    {
        if (_sessionListBox.SelectedItem is ChatSessionRecord session)
        {
            _currentSession = session;
            RenderCurrentSession();
        }
    }

    private void CreateNewSession()
    {
        SaveHistory();
        _currentSession = new ChatSessionRecord();
        _sessions.Add(_currentSession);
        SaveHistory();
        RefreshSessionList();
        RenderCurrentSession();
        _inputTextBox.Focus();
    }

    private void DeleteCurrentSession()
    {
        DialogResult result = MessageBox.Show(
            this,
            $"确定删除“{_currentSession.Title}”及其聊天记录吗？",
            "删除聊天",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (result != DialogResult.Yes)
        {
            return;
        }

        _sessions.Remove(_currentSession);
        if (_sessions.Count == 0)
        {
            _sessions.Add(new ChatSessionRecord());
        }
        _currentSession = _sessions.OrderByDescending(s => s.UpdatedAt).First();
        SaveHistory();
        RefreshSessionList();
        RenderCurrentSession();
    }

    private void RenderCurrentSession()
    {
        _historyTextBox.SuspendLayout();
        _historyTextBox.Clear();
        foreach (ChatMessageRecord message in _currentSession.Messages)
        {
            AppendMessageToView(message);
        }
        _historyTextBox.SelectionStart = _historyTextBox.TextLength;
        _historyTextBox.ScrollToCaret();
        _historyTextBox.ResumeLayout();
    }

    private void AppendMessageToView(ChatMessageRecord message)
    {
        bool isUser = message.Role == "user";
        string speaker = isUser ? "你" : message.IsLocalError ? "连接提示" : "像素黑猫";
        Color speakerColor = message.IsLocalError ? Color.Firebrick : isUser ? Color.FromArgb(35, 92, 160) : Color.FromArgb(80, 80, 88);
        Color bodyColor = message.IsLocalError ? Color.Firebrick : Color.FromArgb(35, 35, 40);

        _historyTextBox.SelectionStart = _historyTextBox.TextLength;
        _historyTextBox.SelectionColor = speakerColor;
        _historyTextBox.SelectionFont = new Font(Font, FontStyle.Bold);
        _historyTextBox.AppendText($"{speaker}  ·  {message.Time:yyyy-MM-dd HH:mm}\r\n");
        _historyTextBox.SelectionColor = bodyColor;
        _historyTextBox.SelectionFont = new Font(Font, FontStyle.Regular);
        _historyTextBox.AppendText(message.Content.Trim() + "\r\n\r\n");
    }

    private void InputTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && !e.Shift)
        {
            e.SuppressKeyPress = true;
            _ = SendCurrentMessageAsync();
        }
    }

    private async Task SendCurrentMessageAsync()
    {
        if (_isSending)
        {
            return;
        }

        string text = _inputTextBox.Text.Trim();
        if (text.Length == 0)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            SetStatus("请先填写 API Key。", Color.DarkOrange);
            OpenSettings();
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                return;
            }
        }

        var userMessage = new ChatMessageRecord
        {
            Role = "user",
            Content = text,
            Time = DateTime.Now
        };
        _currentSession.Messages.Add(userMessage);
        if (_currentSession.Title == "新的聊天" || _currentSession.Messages.Count(m => m.Role == "user") == 1)
        {
            _currentSession.Title = text.Length <= 18 ? text : text[..18] + "…";
        }
        _currentSession.UpdatedAt = DateTime.Now;
        _inputTextBox.Clear();
        AppendMessageToView(userMessage);
        ScrollHistoryToBottom();
        SaveHistory();
        RefreshSessionList();

        SetSendingState(true);
        SetStatus("AI 正在思考……", Color.FromArgb(35, 92, 160));
        try
        {
            string reply = await _client.SendAsync(_settings, _currentSession.Messages, CancellationToken.None);
            var assistantMessage = new ChatMessageRecord
            {
                Role = "assistant",
                Content = reply.Length == 0 ? "（模型没有返回文字）" : reply,
                Time = DateTime.Now
            };
            _currentSession.Messages.Add(assistantMessage);
            _currentSession.UpdatedAt = DateTime.Now;
            AppendMessageToView(assistantMessage);
            SetStatus("回复完成，聊天记录已保存。", Color.DimGray);
        }
        catch (Exception exception)
        {
            var errorMessage = new ChatMessageRecord
            {
                Role = "assistant",
                Content = exception.Message,
                Time = DateTime.Now,
                IsLocalError = true
            };
            _currentSession.Messages.Add(errorMessage);
            _currentSession.UpdatedAt = DateTime.Now;
            AppendMessageToView(errorMessage);
            SetStatus("本次 API 调用失败，请检查 API Key、模型 ID 或额度。", Color.Firebrick);
        }
        finally
        {
            ScrollHistoryToBottom();
            SaveHistory();
            RefreshSessionList();
            SetSendingState(false);
        }
    }

    private void SetSendingState(bool sending)
    {
        _isSending = sending;
        _sendButton.Enabled = !sending;
        _newSessionButton.Enabled = !sending;
        _deleteSessionButton.Enabled = !sending;
        _settingsButton.Enabled = !sending;
        _sessionListBox.Enabled = !sending;
        _sendButton.Text = sending ? "等待中" : "发送";
    }

    private void ScrollHistoryToBottom()
    {
        _historyTextBox.SelectionStart = _historyTextBox.TextLength;
        _historyTextBox.ScrollToCaret();
    }

    private void OpenSettings()
    {
        using var dialog = new ChatSettingsDialog(_settings)
        {
            TopMost = TopMost
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _settings = dialog.Result;
            _store.SaveSettings(_settings);
            UpdateStatus();
        }
    }

    private void SaveHistory()
    {
        _store.SaveSessions(_sessions);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SaveHistory();
        base.OnFormClosing(e);
    }
}

internal sealed class BubbleWindow : LayeredForm
{
    private readonly System.Windows.Forms.Timer _hideTimer;
    private string _text = string.Empty;
    private Rectangle _anchorRect;
    private bool _tailOnLeft;

    public BubbleWindow()
    {
        Cursor = Cursors.Default;
        _hideTimer = new System.Windows.Forms.Timer { Interval = 2400 };
        _hideTimer.Tick += (_, _) => HideBubble();
    }

    public void ShowMessage(string text, Rectangle anchorRect, bool topMost, int displayMilliseconds = 2600)
    {
        _text = text;
        _anchorRect = anchorRect;
        _hideTimer.Stop();
        _hideTimer.Interval = Math.Max(1200, displayMilliseconds);
        RenderAndPlace();
        if (!Visible)
        {
            Show();
            // The first Show creates the native handle after the initial render pass.
            RenderAndPlace();
        }
        SyncTopMost(topMost);
        _hideTimer.Start();
    }

    public void SyncTopMost(bool topMost)
    {
        if (TopMost != topMost)
        {
            TopMost = topMost;
        }
    }

    public void HideBubble()
    {
        _hideTimer.Stop();
        if (Visible)
        {
            Hide();
        }
    }

    private void RenderAndPlace()
    {
        using var font = new Font("Microsoft YaHei UI", 10.5f, FontStyle.Regular, GraphicsUnit.Point);
        using var format = new StringFormat(StringFormat.GenericTypographic)
        {
            Trimming = StringTrimming.Word,
            FormatFlags = StringFormatFlags.LineLimit
        };

        const int maxTextWidth = 230;
        SizeF measured;
        using (var measureBitmap = new Bitmap(1, 1))
        using (var measureGraphics = Graphics.FromImage(measureBitmap))
        {
            measured = measureGraphics.MeasureString(_text, font, maxTextWidth, format);
        }

        int textWidth = Math.Max(34, (int)Math.Ceiling(measured.Width));
        int textHeight = Math.Max(20, (int)Math.Ceiling(measured.Height));
        const int paddingX = 15;
        const int paddingY = 10;
        const int tailWidth = 16;
        int bodyWidth = textWidth + paddingX * 2;
        int bodyHeight = textHeight + paddingY * 2;
        int windowWidth = bodyWidth + tailWidth;
        int windowHeight = bodyHeight;

        Rectangle workArea = Screen.FromPoint(_anchorRect.Location).WorkingArea;
        bool canPlaceRight = _anchorRect.Right + 10 + windowWidth <= workArea.Right;
        bool canPlaceLeft = _anchorRect.Left - 10 - windowWidth >= workArea.Left;
        bool placeRight = canPlaceRight || (!canPlaceLeft && workArea.Right - _anchorRect.Right >= _anchorRect.Left - workArea.Left);
        _tailOnLeft = placeRight;

        int x = placeRight ? _anchorRect.Right + 9 : _anchorRect.Left - 9 - windowWidth;
        int y = _anchorRect.Top + Math.Max(0, _anchorRect.Height / 10);
        x = Math.Clamp(x, workArea.Left + 4, Math.Max(workArea.Left + 4, workArea.Right - windowWidth - 4));
        y = Math.Clamp(y, workArea.Top + 4, Math.Max(workArea.Top + 4, workArea.Bottom - windowHeight - 4));

        Location = new Point(x, y);
        Size = new Size(windowWidth, windowHeight);

        using var bitmap = new Bitmap(windowWidth, windowHeight, PixelFormat.Format32bppPArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int bodyX = _tailOnLeft ? tailWidth : 0;
            var bodyRect = new Rectangle(bodyX + 1, 1, bodyWidth - 2, bodyHeight - 2);
            int tailCenterY = Math.Clamp(bodyHeight / 2, 22, Math.Max(22, bodyHeight - 22));
            Point[] tail = _tailOnLeft
                ? [new Point(bodyX + 2, tailCenterY - 9), new Point(1, tailCenterY), new Point(bodyX + 2, tailCenterY + 9)]
                : [new Point(bodyWidth - 2, tailCenterY - 9), new Point(windowWidth - 1, tailCenterY), new Point(bodyWidth - 2, tailCenterY + 9)];

            using var fillBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
            using var borderPen = new Pen(Color.FromArgb(255, 54, 54, 60), 2f);
            graphics.FillPolygon(fillBrush, tail);
            using (GraphicsPath path = CreateRoundedRectangle(bodyRect, 13))
            {
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);
            }
            graphics.DrawLine(borderPen, tail[0], tail[1]);
            graphics.DrawLine(borderPen, tail[1], tail[2]);

            var textRect = new Rectangle(bodyX + paddingX, paddingY, textWidth, textHeight);
            using var textBrush = new SolidBrush(Color.FromArgb(255, 35, 35, 40));
            graphics.DrawString(_text, font, textBrush, textRect, format);
        }

        if (IsHandleCreated)
        {
            SetBitmap(bitmap);
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!string.IsNullOrEmpty(_text) && Visible)
        {
            RenderAndPlace();
        }
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void WndProc(ref Message message)
    {
        const int wmNchitTest = 0x0084;
        const int htTransparent = -1;
        if (message.Msg == wmNchitTest)
        {
            message.Result = new IntPtr(htTransparent);
            return;
        }
        base.WndProc(ref message);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _hideTimer.Stop();
        _hideTimer.Dispose();
        base.OnFormClosing(e);
    }
}

internal abstract class LayeredForm : Form
{
    private const int WsExLayered = 0x00080000;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;
    private const int WsExTransparent = 0x00000020;

    protected LayeredForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.Black;
        MinimizeBox = false;
        MaximizeBox = false;
        ControlBox = false;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams parameters = base.CreateParams;
            parameters.ExStyle |= WsExLayered | WsExToolWindow | WsExNoActivate;
            return parameters;
        }
    }

    protected void SetBitmap(Bitmap bitmap)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        IntPtr screenDc = NativeMethods.GetDC(IntPtr.Zero);
        IntPtr memoryDc = NativeMethods.CreateCompatibleDC(screenDc);
        IntPtr hBitmap = IntPtr.Zero;
        IntPtr oldBitmap = IntPtr.Zero;

        try
        {
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            oldBitmap = NativeMethods.SelectObject(memoryDc, hBitmap);

            var windowPosition = new NativeMethods.Point32(Left, Top);
            var bitmapSize = new NativeMethods.Size32(bitmap.Width, bitmap.Height);
            var sourcePosition = new NativeMethods.Point32(0, 0);
            var blend = new NativeMethods.BlendFunction
            {
                BlendOp = NativeMethods.AcSrcOver,
                BlendFlags = 0,
                SourceConstantAlpha = 255,
                AlphaFormat = NativeMethods.AcSrcAlpha
            };

            NativeMethods.UpdateLayeredWindow(
                Handle,
                screenDc,
                ref windowPosition,
                ref bitmapSize,
                memoryDc,
                ref sourcePosition,
                0,
                ref blend,
                NativeMethods.UlwAlpha);
        }
        finally
        {
            if (oldBitmap != IntPtr.Zero)
            {
                NativeMethods.SelectObject(memoryDc, oldBitmap);
            }
            if (hBitmap != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(hBitmap);
            }
            if (memoryDc != IntPtr.Zero)
            {
                NativeMethods.DeleteDC(memoryDc);
            }
            if (screenDc != IntPtr.Zero)
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);
            }
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Layered-window pixels are supplied through UpdateLayeredWindow.
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Prevent GDI from painting an opaque fallback over the layered bitmap.
    }
}

internal static class NativeMethods
{
    public const byte AcSrcOver = 0x00;
    public const byte AcSrcAlpha = 0x01;
    public const int UlwAlpha = 0x02;

    [StructLayout(LayoutKind.Sequential)]
    public struct Point32
    {
        public int X;
        public int Y;

        public Point32(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size32
    {
        public int Width;
        public int Height;

        public Size32(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UpdateLayeredWindow(
        IntPtr hwnd,
        IntPtr hdcDst,
        ref Point32 pptDst,
        ref Size32 psize,
        IntPtr hdcSrc,
        ref Point32 pptSrc,
        int crKey,
        ref BlendFunction pblend,
        int dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint uFlags);
}
