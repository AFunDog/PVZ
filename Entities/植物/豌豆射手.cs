using System;
using Godot;
using 植物大战僵尸.Entities;

public partial class 豌豆射手 : AnimatedSprite2D, IPlant
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }

    public event Action<IEntity>? EntityDied;

    const double ShootInterval = 1.2;
    private double _shootTime = 0;

    private static readonly PackedScene _bulletScene = GD.Load<PackedScene>(
        "res://Entities/子弹/豌豆子弹.tscn"
    );

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Math.Abs(_shootTime - 0) < 0.1)
        {
            Shot();
            _shootTime = ShootInterval;
        }
        _shootTime -= delta;
    }

    private void Shot()
    {
        var bullet = _bulletScene.Instantiate<豌豆子弹>();
        var control = GetParent<Control>();
        if (control.Name != "GameMapContainer")
        {
            return;
        }
        control.CallDeferred(MethodName.AddChild, bullet);
        bullet.TreeEntered += BulletEntered;
        void BulletEntered()
        {
            bullet.TreeEntered -= BulletEntered;
            bullet.GlobalPosition = GlobalPosition with
            {
                X = GlobalPosition.X + 20,
                Y = GlobalPosition.Y - 18
            };
        }
    }
}
