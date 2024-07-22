using System;
using Godot;
using 植物大战僵尸.Entities;

public partial class 向日葵 : AnimatedSprite2D, IPlant
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }

    public event Action<IEntity>? EntityDied;

    const double GenerateSunInterval = 20;
    private double _generateSunTime = 5;

    private static readonly PackedScene _sunShinePackage = GD.Load<PackedScene>(
        "res://Entities/阳光.tscn"
    );

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

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
