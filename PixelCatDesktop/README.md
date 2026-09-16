# PixelCat Desktop - 跨平台像素黑猫桌宠

基于 [Avalonia UI](https://avaloniaui.net/) 的跨平台像素黑猫桌宠，支持 macOS、Windows、Linux。

## 功能

- 透明悬浮窗渲染的像素黑猫
- 拖拽移动、点击交互
- AI 深度对话（OpenAI 兼容 API）
- 精灵动画系统
- 多平台支持

## 构建

```bash
dotnet restore
dotnet build -c Release
dotnet run -c Release
```

## 环境要求

- .NET 8.0 SDK
- macOS / Windows / Linux

## 项目结构

```
PixelCatDesktop/
├── Program.cs              # 入口
├── App.axaml               # Avalonia 配置
├── Views/
│   └── PetWindow.axaml     # 透明悬浮窗
├── ViewModels/
│   └── PetWindowViewModel.cs
├── Services/
│   ├── AiChatService.cs    # AI 聊天
│   └── SettingsService.cs  # 设置持久化
└── Models/
    ├── PetState.cs
    └── ChatMessage.cs
```
