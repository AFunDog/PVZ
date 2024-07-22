using System;
using System.Diagnostics;
using Godot;

public partial class 阳光 : Control
{
    public event Action<阳光>? Click;
    public float TargetY { get; set; }

    private AudioStreamPlayer? _clickMusic;
    private AnimatedSprite2D _sprite2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _clickMusic = GetNode<AudioStreamPlayer>("%ClickMusic");
        _sprite2D = GetNode<AnimatedSprite2D>("%Sprite2D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Position.Y <= TargetY)
        {
            Position = Position with { Y = Position.Y + 500 * (float)delta };
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        //base._UnhandledInput(@event);
        switch (@event)
        {
            case InputEventMouseButton mouse:
                if (mouse.ButtonIndex == MouseButton.Left && mouse.Pressed)
                {
                    _clickMusic?.Play();
                    Click?.Invoke(this);
                }
                break;
            default:
                break;
        }
    }
}
