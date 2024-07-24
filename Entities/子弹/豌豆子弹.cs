using System;
using Godot;
using 植物大战僵尸.Entities;

public partial class 豌豆子弹 : Area2D, IBullet
{
    const double ShotSpeed = 400;

    public int Damage { get; set; } = 20;
    public bool IsCold { get; set; } = false;

    public override void _PhysicsProcess(double delta)
    {
        if (Position is (< -40 or > 1600, _))
        {
            QueueFree();
        }
        Position = Position with { X = (float)(Position.X + ShotSpeed * delta) };
    }
}
