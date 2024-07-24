using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace 植物大战僵尸.Entities
{
    public partial class 僵尸 : Area2D, IZombie
    {
        private AnimatedSprite2D? _sprite2D;
        private int _health = 270;
        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                if (_health < 0)
                {
                    OnZombieDied();
                }
            }
        }

        public virtual double MoveSpeed { get; set; } = 20;

        public SpriteFrames? SpriteFrames
        {
            get => _sprite2D?.SpriteFrames;
            set
            {
                if (_sprite2D is not null)
                {
                    _sprite2D.SpriteFrames = value;
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            Position = Position with { X = (float)(Position.X - MoveSpeed * delta) };
        }

        private void OnZombieDied()
        {
            EntityDied?.Invoke(this);
            _hitTimer?.Dispose();
            _hitTimer = null;
            _frozenTimer?.Dispose();
            _frozenTimer = null;
            QueueFree();
        }

        public event Action<IEntity>? EntityDied;

        private void OnAreaEntered(Area2D area)
        {
            if (area is IBullet bullet)
            {
                BeHit(bullet);
                area.QueueFree();
            }
        }

        const double HitInterval = 0.1;
        const double FrozenInterval = 5;
        private SceneTreeTimer? _hitTimer = null;
        private SceneTreeTimer? _frozenTimer = null;

        private void BeHit(IBullet bullet)
        {
            Health -= bullet.Damage;

            if (_sprite2D is null)
                return;

            if (_hitTimer is not null)
            {
                _hitTimer.TimeLeft = HitInterval;
            }
            else
            {
                (_sprite2D.Material as ShaderMaterial)!.SetShaderParameter("hit", true);
                _hitTimer = GetTree().CreateTimer(HitInterval);
                _hitTimer.Timeout += () =>
                {
                    (_sprite2D.Material as ShaderMaterial)!.SetShaderParameter("hit", false);
                    _hitTimer = null;
                };
            }

            if (bullet.IsCold)
            {
                if (_frozenTimer is not null)
                {
                    _frozenTimer.TimeLeft = FrozenInterval;
                }
                else
                {
                    (_sprite2D.Material as ShaderMaterial)!.SetShaderParameter("frozen", true);
                    double orginSpeed = MoveSpeed;
                    MoveSpeed /= 2;
                    _frozenTimer = GetTree().CreateTimer(FrozenInterval);
                    _frozenTimer.Timeout += () =>
                    {
                        (_sprite2D.Material as ShaderMaterial)!.SetShaderParameter("frozen", false);
                        MoveSpeed = orginSpeed;
                        _frozenTimer = null;
                    };
                }
            }
        }
    }
}
