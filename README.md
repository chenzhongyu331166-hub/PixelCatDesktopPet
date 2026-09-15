# PixelCatDesktopPet - 像素黑猫桌宠

> **Vibe Coding 实习项目** — 全程使用 AI 辅助编程（OpenCode / Claude），从需求分析到功能实现到部署上线，一人一天完成。

一只住在你 Windows 桌面上的像素黑猫，提供交互陪伴和 AI 深度对话。

## 截图

<p align="center">
  <img src="screenshots/cat-transparent.png" width="200" alt="像素黑猫桌宠"/>
</p>

## 功能亮点

### 桌面宠物
- 像素黑猫精灵，透明窗口渲染，7种动画 + 行走 + 吃饭 + 睡觉
- 桌面拖拽、滚轮缩放（迷你到大大猫 5档大小）
- 自主巡逻行为（基于真实猫咪作息的随机行为权重）
- 主动说话、时间感知对话（早安/午安/晚安/下班倒计时）

### AI 聊天（核心功能）
- **支持 8+ 种 API 提供商**，用户只需填写 API Key 即可使用：
  - OpenAI、DeepSeek、通义千问、月之暗面、智谱 AI、零一万物、魔搭、Ollama（本地）
  - 以及所有 OpenAI 兼容的第三方接口
- 完整的多会话管理、聊天记录本地持久化
- 右键菜单快速切换 AI 聊天 / 本地逗猫

### 本地互动
- 快速逗猫（纯本地关键词匹配，无需网络）
- 休息提醒（可自定义间隔）
- 开机自启动、始终置顶

## 技术栈

| 层面 | 技术 |
|------|------|
| 语言 | C# |
| 框架 | .NET 8.0 (net8.0-windows) |
| UI | Windows Forms (WinForms) |
| 渲染 | Win32 分层窗口 + `UpdateLayeredWindow` P/Invoke |
| AI 接口 | OpenAI 兼容格式 HTTP API |
| 存储 | JSON 文件持久化 (%APPDATA%) |

## 快速开始

### 下载

前往 [Releases](https://github.com/chenzhongyu331166-hub/PixelCatDesktopPet/releases) 下载最新版本，解压后直接运行 `像素黑猫桌宠.exe`。

### 从源码编译

```bash
git clone https://github.com/chenzhongyu331166-hub/PixelCatDesktopPet.git
cd PixelCatDesktopPet
dotnet build -c Release
dotnet run -c Release
```

环境要求：Windows 10/11 + .NET 8.0 Desktop Runtime

## 项目结构

```
PixelCatDesktopPet/
├── Program.cs              # 全部源码（单文件架构，~3300行）
├── PixelCatDesktopPet.csproj
└── Assets/                 # 精灵图素材
    ├── cat.png             # 待机动画
    ├── cat_walk_*.png      # 行走动画
    ├── cat_lick_*.png      # 舔毛动画
    ├── cat_eat_*.png       # 吃饭动画
    └── cat_sleep.png       # 睡觉精灵
```

## Vibe Coding 实践记录

本项目是 **Vibe Coding** 的实践案例：

- **开发工具**：OpenCode CLI + Claude 模型
- **开发模式**：对话式编程，通过自然语言描述需求，AI 生成代码并迭代
- **开发周期**：单人一天完成从 0 到可部署的完整产品
- **代码量**：单文件约 3300 行 C# 代码
- **核心理念**：快速原型 → 功能迭代 → 部署上线，验证 AI 辅助开发的效率

## 跨平台说明

当前版本仅支持 **Windows**（使用了 WinForms + Win32 API）。

如需跨平台支持（macOS / Linux / Android / iOS），可考虑迁移至：
- [Avalonia UI](https://avaloniaui.net/) — 跨平台 .NET UI 框架
- [.NET MAUI](https://learn.microsoft.com/dotnet/maui/) — 移动端跨平台
- [Uno Platform](https://platform.uno/) — Web + 移动端

## 许可证

MIT License
