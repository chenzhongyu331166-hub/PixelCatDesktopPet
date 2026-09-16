using Android.Content;
using Android.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace PixelCatAndroid.Views
{
    /// <summary>
    /// 使用 SkiaSharp 渲染像素猫咪精灵的自定义 View
    /// 负责绘制猫咪图形、聊天气泡和动画帧
    /// </summary>
    public class SpriteView : SKCanvasView
    {
        // 猫咪精灵帧
        private static readonly SKColor[,] IdleFrame1 = new SKColor[16, 16];
        private static readonly SKColor[,] IdleFrame2 = new SKColor[16, 16];
        private static readonly SKColor[,] WalkFrame1 = new SKColor[16, 16];
        private static readonly SKColor[,] WalkFrame2 = new SKColor[16, 16];

        private SKBitmap? _catBitmap;
        private int _currentFrame;
        private int _animationCounter;
        private bool _isWalking;

        // 聊天气泡
        private string _bubbleText = string.Empty;
        private int _bubbleAlpha = 0;
        private System.Timers.Timer? _bubbleTimer;

        // 猫咪位置
        private float _catX = 100;
        private float _catY = 200;
        private int _spriteSize = 128; // 绘制时放大的尺寸

        // 颜色定义（像素猫调色板）
        private static readonly SKColor BodyColor = new(255, 180, 100);      // 橙色身体
        private static readonly SKColor DarkBodyColor = new(220, 140, 70);   // 深橙色（阴影）
        private static readonly SKColor EyeColor = new(40, 40, 40);          // 黑色眼睛
        private static readonly SKColor NoseColor = new(255, 120, 120);      // 粉色鼻子
        private static readonly SKColor WhiskerColor = new(180, 180, 180);   // 灰色胡须
        private static readonly SKColor EarInner = new(255, 150, 150);       // 内耳粉色
        private static readonly SKColor Transparent = SKColors.Transparent;

        public float CatX { get => _catX; set { _catX = value; Invalidate(); } }
        public float CatY { get => _catY; set { _catY = value; Invalidate(); } }
        public bool IsWalking { get => _isWalking; set => _isWalking = value; }

        public SpriteView(Context context) : base(context)
        {
            InitBitmap();
        }

        /// <summary>
        /// 生成猫咪精灵位图（程序化像素画）
        /// </summary>
        private void InitBitmap()
        {
            _catBitmap = new SKBitmap(16, 16, SKColorType.Rgba8888, SKAlphaType.Premul);
            DrawCatToBitmap(_catBitmap, 0);
        }

        /// <summary>
        /// 绘制猫咪到 SKBitmap
        /// </summary>
        private static void DrawCatToBitmap(SKBitmap bitmap, int frame)
        {
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false // 像素风格，关闭抗锯齿
            };

            // 简单的 16x16 像素猫绘制
            // 身体
            paint.Color = BodyColor;
            canvas.DrawRect(new SKRect(4, 5, 12, 13), paint);

            // 头部
            canvas.DrawRect(new SKRect(3, 2, 13, 7), paint);

            // 耳朵
            paint.Color = BodyColor;
            canvas.DrawRect(new SKRect(3, 0, 6, 3), paint);  // 左耳
            canvas.DrawRect(new SKRect(10, 0, 13, 3), paint); // 右耳

            // 内耳
            paint.Color = EarInner;
            canvas.DrawRect(new SKRect(4, 1, 5, 2), paint);
            canvas.DrawRect(new SKRect(11, 1, 12, 2), paint);

            // 眼睛（根据帧有轻微变化）
            paint.Color = EyeColor;
            int eyeOffset = frame % 2 == 0 ? 0 : 0; // 可扩展眨眼动画
            canvas.DrawRect(new SKRect(5 + eyeOffset, 3, 6 + eyeOffset, 5), paint);
            canvas.DrawRect(new SKRect(9 + eyeOffset, 3, 10 + eyeOffset, 5), paint);

            // 鼻子
            paint.Color = NoseColor;
            canvas.DrawRect(new SKRect(7, 5, 9, 6), paint);

            // 胡须
            paint.Color = WhiskerColor;
            canvas.DrawRect(new SKRect(1, 4, 3, 5), paint);
            canvas.DrawRect(new SKRect(13, 4, 15, 5), paint);
            canvas.DrawRect(new SKRect(1, 6, 3, 7), paint);
            canvas.DrawRect(new SKRect(13, 6, 15, 7), paint);

            // 前腿
            paint.Color = BodyColor;
            canvas.DrawRect(new SKRect(5, 11, 7, 14), paint);
            canvas.DrawRect(new SKRect(9, 11, 11, 14), paint);

            // 脚掌
            paint.Color = NoseColor;
            canvas.DrawRect(new SKRect(5, 13, 7, 14), paint);
            canvas.DrawRect(new SKRect(9, 13, 11, 14), paint);

            // 尾巴（根据帧摆动）
            paint.Color = DarkBodyColor;
            int tailOffset = frame % 2 == 0 ? 0 : -1;
            canvas.DrawRect(new SKRect(12, 8 + tailOffset, 15, 10 + tailOffset), paint);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var surface = e.Surface;
            if (surface == null) return;

            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            // 绘制猫咪
            if (_catBitmap != null)
            {
                using var paint = new SKPaint
                {
                    FilterQuality = SKFilterQuality.None,
                    IsAntialias = false
                };
                canvas.DrawBitmap(_catBitmap,
                    new SKRect(_catX, _catY, _catX + _spriteSize, _catY + _spriteSize),
                    paint);
            }

            // 绘制聊天气泡
            if (_bubbleAlpha > 0 && !string.IsNullOrEmpty(_bubbleText))
            {
                DrawChatBubble(canvas);
            }

            // 更新动画
            _animationCounter++;
            if (_animationCounter >= 15) // 每 15 帧切换
            {
                _animationCounter = 0;
                _currentFrame = (_currentFrame + 1) % 4;
                if (_catBitmap != null)
                    DrawCatToBitmap(_catBitmap, _currentFrame);
            }

            if (_bubbleAlpha > 0)
            {
                PostInvalidateDelayed(50); // 保持气泡动画
            }
        }

        /// <summary>
        /// 绘制聊天气泡
        /// </summary>
        private void DrawChatBubble(SKCanvas canvas)
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 24,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Noto Sans SC", SKFontStyleWeight.Normal,
                    SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(_bubbleText, ref textBounds);

            float padding = 16;
            float bubbleWidth = textBounds.Width + padding * 2;
            float bubbleHeight = textBounds.Height + padding * 2;
            float bubbleX = _catX + _spriteSize / 2 - bubbleWidth / 2;
            float bubbleY = _catY - bubbleHeight - 20;

            using var bubblePaint = new SKPaint
            {
                Color = new SKColor(255, 255, 255, (byte)Math.Min(_bubbleAlpha, 240)),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // 气泡背景（圆角矩形）
            using var path = new SKPath();
            path.AddRoundRect(new SKRect(bubbleX, bubbleY, bubbleX + bubbleWidth, bubbleY + bubbleHeight), 12, 12);
            canvas.DrawPath(path, bubblePaint);

            // 气泡边框
            using var borderPaint = new SKPaint
            {
                Color = new SKColor(200, 200, 200, (byte)Math.Min(_bubbleAlpha, 240)),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true
            };
            canvas.DrawPath(path, borderPaint);

            // 气泡小三角
            float triangleX = _catX + _spriteSize / 2;
            float triangleY = bubbleY + bubbleHeight;
            var triPath = new SKPath();
            triPath.MoveTo(triangleX - 8, triangleY);
            triPath.LineTo(triangleX, triangleY + 10);
            triPath.LineTo(triangleX + 8, triangleY);
            triPath.Close();
            canvas.DrawPath(triPath, bubblePaint);
            canvas.DrawPath(triPath, borderPaint);

            // 绘制文字
            using var textPaintAlpha = new SKPaint
            {
                Color = new SKColor(50, 50, 50, (byte)Math.Min(_bubbleAlpha, 240)),
                TextSize = 24,
                IsAntialias = true,
                Typeface = textPaint.Typeface
            };
            canvas.DrawText(_bubbleText, bubbleX + padding, bubbleY + padding + textBounds.Height,
                textPaintAlpha);
        }

        /// <summary>
        /// 显示聊天气泡（持续 5 秒后淡出）
        /// </summary>
        public void ShowBubble(string text)
        {
            _bubbleText = text;
            _bubbleAlpha = 255;
            Invalidate();

            _bubbleTimer?.Dispose();
            _bubbleTimer = new System.Timers.Timer(5000);
            _bubbleTimer.Elapsed += (s, e) =>
            {
                _bubbleAlpha = 0;
                _bubbleText = string.Empty;
                _bubbleTimer?.Dispose();
                _bubbleTimer = null;
                Invalidate();
            };
            _bubbleTimer.Start();
        }

        /// <summary>
        /// 检查点击是否在猫咪范围内
        /// </summary>
        public bool HitTest(float touchX, float touchY)
        {
            return touchX >= _catX && touchX <= _catX + _spriteSize
                && touchY >= _catY && touchY <= _catY + _spriteSize;
        }

        /// <summary>
        /// 获取猫咪中心 X 坐标
        /// </summary>
        public float GetCenterX() => _catX + _spriteSize / 2;

        /// <summary>
        /// 获取猫咪中心 Y 坐标
        /// </summary>
        public float GetCenterY() => _catY + _spriteSize / 2;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _catBitmap?.Dispose();
                _bubbleTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
