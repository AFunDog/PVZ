using System;
using Godot;

public partial class 工具按钮 : Control
{
    private Label? _content;
    private PanelContainer? _container;
    private StyleBoxTexture _normalStyle = GD.Load<StyleBoxTexture>(
        "res://Controls/工具按钮/工具样式_正常.tres"
    );
    private StyleBoxTexture _pointerOverStyle = GD.Load<StyleBoxTexture>(
        "res://Controls/工具按钮/工具样式_高亮.tres"
    );

    [Export]
    public string Content { get; set; } = string.Empty;

    [Signal]
    public delegate void ClickedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _content = GetNode<Label>("%Content");
        _content.Text = Content;

        _container = GetNode<PanelContainer>("%Container");
        _container.GuiInput += OnPanelGuiInput;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void OnMouseEntered()
    {
        _container?.AddThemeStyleboxOverride("panel", _pointerOverStyle);
    }

    private void OnMouseExited()
    {
        _container?.AddThemeStyleboxOverride("panel", _normalStyle);
    }

    public void OnPanelGuiInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton mouseButton:
                if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
                {
                    EmitSignal(nameof(Clicked));
                }
                break;
        }
    }
}
