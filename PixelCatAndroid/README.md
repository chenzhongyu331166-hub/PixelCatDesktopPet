# PixelCat Android - 像素猫咪桌面宠物

基于 C# WinForms 桌面宠物 (PixelCatDesktopPet) 的 .NET for Android 移植版。

## 功能特性

- 像素风格猫咪精灵，程序化绘制
- 系统悬浮窗显示（SYSTEM_ALERT_WINDOW）
- 前台服务保持运行
- 触摸拖拽移动猫咪
- 点击触发 AI 聊天对话
- OpenAI 兼容 API 支持

## 项目结构

```
PixelCatAndroid/
├── PixelCatAndroid.csproj          # 项目文件
├── AndroidManifest.xml             # 清单文件（权限声明）
├── MainActivity.cs                 # 主界面（权限请求、服务控制）
├── Services/
│   └── OverlayService.cs           # 前台服务（悬浮窗管理）
├── Views/
│   ├── SpriteView.cs               # SkiaSharp 精灵渲染
│   └── SettingsActivity.cs         # 设置页面
├── Models/
│   ├── ChatSettings.cs             # API 设置
│   └── ChatMessage.cs              # 聊天消息模型
├── Assets/                         # 精灵资源（预留）
└── Resources/
    ├── drawable/                    # 图标资源
    └── values/
        ├── strings.xml             # 字符串资源
        └── colors.xml              # 颜色资源
```

## 构建要求

- .NET 8.0 SDK (with Android workload)
- Android SDK 34+

## 构建命令

```bash
dotnet restore
dotnet build
```

## 安装部署

1. 构建 APK：
   ```bash
   dotnet publish -c Release -r android-arm64
   ```

2. 安装到设备：
   ```bash
   adb install bin/Release/net8.0-android-arm64/com.pixelcat.desktoppet.apk
   ```

3. 启动应用后：
   - 点击"检查悬浮窗权限"授予必要权限
   - 点击"设置"配置 OpenAI API
   - 点击"启动桌宠"开始使用

## AI 配置

在设置页面中配置以下内容：
- **API 地址**：OpenAI 兼容端点（如 `https://api.openai.com/v1/chat/completions`）
- **API 密钥**：你的 API Key（如 `sk-xxx`）
- **模型名称**：模型标识（如 `gpt-3.5-turbo`）
- **系统提示词**：自定义猫咪性格

未配置 API 时，猫咪会使用预设的中文问候语回复。
