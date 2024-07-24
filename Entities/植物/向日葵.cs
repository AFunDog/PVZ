using System;
using Godot;
using 植物大战僵尸.Entities;

public partial class 向日葵 : 植物
{
    public override int MaxHealth { get; set; } = 300;
    protected override AnimatedSprite2D? Sprite2D => GetNode<AnimatedSprite2D>("%Sprite2D");

    const double GenerateSunInterval = 20;
    private double _generateSunTime = 5;

    private static readonly PackedScene _sunShinePackage = GD.Load<PackedScene>(
        "res://Entities/阳光.tscn"
    );

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Math.Abs(_generateSunTime - 0) < 0.1)
        {
            _generateSunTime = GenerateSunInterval;
            GenerateSunShine();
        }
        _generateSunTime -= delta;
    }

    private void GenerateSunShine()
    {
        var sun = _sunShinePackage.Instantiate<阳光>();
        CallDeferred(Control.MethodName.AddChild, sun);
    }
}
