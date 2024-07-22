using System;
using Godot;
using 植物大战僵尸.Helpers;
using 植物大战僵尸.Structs;

public partial class 卡牌 : TextureRect
{
    private double _restCd;

    public CardData? CardData { get; set; }
    public event Action<卡牌>? Click;
    public double RestCd
    {
        get => _restCd;
        set
        {
            value = Math.Max(value, 0);
            _restCd = value;
            if (_cdBar is not null && CardData is not null)
            {
                _cdBar.Value = (_restCd / CardData.Cd) * _cdBar.MaxValue;
            }
        }
    }

    private TextureProgressBar? _cdBar;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TooltipText = CardData?.Name ?? string.Empty;

        _cdBar = GetNode<TextureProgressBar>("%CdBar");
        Texture = GD.Load<Texture2D>(CardData!.TexturePath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        switch (@event)
        {
            case InputEventMouseButton mouseButton:
                if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
                {
                    Click?.Invoke(this);
                    GD.Print("Clicked");
                }
                break;
        }
    }
}
