# PixelCatDesktopPet - 像素黑猫桌宠

一个基于 C# WinForms 的 Windows 桌面宠物应用，以一只像素风格的黑猫形象驻留在桌面上，提供交互、陪伴和 AI 聊天功能。

## 功能特性

- 像素黑猫精灵的透明窗口渲染（7种动画 + 行走 + 吃饭 + 睡觉）
- 桌面拖拽、缩放（滚轮调整大小）
- 自主巡逻行为（基于真实猫咪作息的随机行为权重）
- 主动说话、时间感知对话
- 休息提醒（可自定义间隔）
- 下班倒计时与节假日信息
- 两种对话模式：
  - **快速本地逗猫**：纯本地关键词匹配，无需网络
  - **AI 深度对话**：支持多种 API 提供商的 OpenAI 兼容接口
- 完整的聊天记录本地持久化存储（JSON 文件）
- 开机自启动
- 始终置顶

## AI 聊天支持的提供商

| 提供商 | Base URL | 模型 |
|--------|----------|------|
| OpenAI | `https://api.openai.com/v1` | gpt-4o-mini |
| DeepSeek | `https://api.deepseek.com/v1` | deepseek-chat |
| 通义千问 (阿里云) | `https://dashscope.aliyuncs.com/compatible-mode/v1` | qwen-plus |
| 月之暗面 (Kimi) | `https://api.moonshot.cn/v1` | moonshot-v1-8k |
| 智谱 AI (GLM) | `https://open.bigmodel.cn/api/paas/v4` | glm-4-flash |
| 零一万物 (Yi) | `https://api.lingyiwanwu.com/v1` | yi-lightning |
| 魔搭 ModelScope | `https://api-inference.modelscope.cn/v1` | deepseek-ai/DeepSeek-V4-Flash-0731 |
| Ollama (本地) | `http://localhost:11434/v1` | qwen2.5:7b |
| 自定义 (OpenAI 兼容) | 用户自定义 | 用户自定义 |

## 环境要求

- Windows 10/11
- .NET 8.0 Desktop Runtime（或使用 self-contained 发布版本）

## 编译与运行

```bash
# 克隆仓库
git clone https://github.com/你的用户名/PixelCatDesktopPet.git
cd PixelCatDesktopPet

# 编译
dotnet build -c Release

# 运行
dotnet run -c Release
```

## 发布独立版本

```bash
dotnet publish -c Release -r win-x64 --self-contained true -o publish
```

## 技术栈

- 语言：C#
- 框架：.NET 8.0 (net8.0-windows)
- UI 框架：Windows Forms (WinForms)
- 渲染方式：Win32 分层窗口 (Layered Window) + `UpdateLayeredWindow` P/Invoke
- AI 接口：OpenAI 兼容格式的 HTTP API

## 许可证

MIT License
