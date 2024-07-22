using System;
using Godot;

public partial class 僵尸按钮 : Control
{
    private Label? _content;
    private Container? _container;

    [Export]
    public string Content { get; set; } = string.Empty;

    [Signal]
    public delegate void ClickedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _content = GetNode<Label>("%Content");
        _content.Text = Content;

        _container = GetNode<Container>("%Container");
        _container.GuiInput += OnPanelGuiInput;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void OnMouseEntered()
    {
        _content?.AddThemeColorOverride("font_color", Color.Color8(20, 255, 20));
    }

    private void OnMouseExited()
    {
        _content?.AddThemeColorOverride("font_color", Color.Color8(17, 181, 17));
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
