using System;
using Godot;
using 植物大战僵尸.Entities;

public partial class 寒冰射手 : 植物
{
    public override int MaxHealth { get; set; } = 300;
    protected override AnimatedSprite2D? Sprite2D => GetNode<AnimatedSprite2D>("%Sprite2D");
    public override double WorkInterval { get; set; } = 1.2;

    private static readonly PackedScene _bulletScene = GD.Load<PackedScene>(
        "res://Entities/子弹/豌豆子弹.tscn"
    );

    protected override void OnPlantWork()
    {
        Shot();
    }

    private void Shot()
    {
        var bullet = _bulletScene.Instantiate<豌豆子弹>();
        bullet.IsCold = true;
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
