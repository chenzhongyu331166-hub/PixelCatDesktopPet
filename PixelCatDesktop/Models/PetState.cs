namespace PixelCatDesktop.Models;

public enum PetActivity
{
    None,
    Eating,
    Sleeping
}

public enum PetAnimation
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

public class PetState
{
    public PetActivity Activity { get; set; } = PetActivity.None;
    public PetAnimation CurrentAnimation { get; set; } = PetAnimation.None;
    public bool IsWalking { get; set; }
    public bool FacingLeft { get; set; } = true;
    public float ScaleFactor { get; set; } = 1.0f;
}
