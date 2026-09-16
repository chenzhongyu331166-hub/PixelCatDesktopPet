using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PixelCatDesktop.ViewModels;

public class PetWindowViewModel : INotifyPropertyChanged
{
    private string _bubbleText = "你好呀！我是像素黑猫~";
    private bool _isBubbleVisible = true;
    private int _animationFrame;
    private string _currentState = "idle";

    public string BubbleText
    {
        get => _bubbleText;
        set { _bubbleText = value; OnPropertyChanged(); }
    }

    public bool IsBubbleVisible
    {
        get => _isBubbleVisible;
        set { _isBubbleVisible = value; OnPropertyChanged(); }
    }

    public int AnimationFrame
    {
        get => _animationFrame;
        set { _animationFrame = value; OnPropertyChanged(); }
    }

    public string CurrentState
    {
        get => _currentState;
        set { _currentState = value; OnPropertyChanged(); }
    }

    public void UpdateAnimation()
    {
        AnimationFrame = (AnimationFrame + 1) % 4;
    }

    public void ShowBubble(string message)
    {
        BubbleText = message;
        IsBubbleVisible = true;
    }

    public void HideBubble()
    {
        IsBubbleVisible = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
