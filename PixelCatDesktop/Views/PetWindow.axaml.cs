using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PixelCatDesktop.ViewModels;
using System;

namespace PixelCatDesktop.Views;

public partial class PetWindow : Window
{
    private PetWindowViewModel _viewModel;
    private bool _isDragging;
    private Point _dragStart;

    public PetWindow()
    {
        InitializeComponent();
        _viewModel = new PetWindowViewModel();
        DataContext = _viewModel;

        Opened += OnWindowOpened;

        var animTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        animTimer.Tick += (_, _) => _viewModel.UpdateAnimation();
        animTimer.Start();
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        var screen = Screens?.Primary;
        if (screen != null)
        {
            var workArea = screen.WorkingArea;
            Position = new PixelPoint(
                workArea.Right - (int)Width - 48,
                workArea.Bottom - (int)Height - 24);
        }
    }

    private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _dragStart = e.GetPosition(this);
            e.Handled = true;
        }
    }

    private void OnCanvasMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            var pos = e.GetPosition(this);
            var deltaX = pos.X - _dragStart.X;
            var deltaY = pos.Y - _dragStart.Y;
            Position = new PixelPoint(
                Position.X + (int)deltaX,
                Position.Y + (int)deltaY);
        }
    }

    private void OnCanvasReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }
}
