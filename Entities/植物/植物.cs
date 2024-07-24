using System;
using Godot;
using 植物大战僵尸.Entities;

public partial class 植物 : Area2D, IPlant
{
    public int X { get; set; }
    public int Y { get; set; }
    public virtual int MaxHealth { get; set; }
    public virtual double WorkInterval { get; set; }
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                OnEntityDied();
            }
        }
    }
    protected virtual AnimatedSprite2D? Sprite2D { get; set; }
    public event Action<IEntity>? EntityDied;

    private Timer? _workTimer;

    public override void _Ready()
    {
        _health = MaxHealth;
        if (WorkInterval > 0)
        {
            _workTimer = new Timer();
            CallDeferred(MethodName.AddChild, _workTimer);
            _workTimer.TreeEntered += () =>
            {
                _workTimer.Timeout += OnPlantWork;
                _workTimer.Start(WorkInterval);
            };
        }
    }

    public void Play()
    {
        Sprite2D?.Play();
    }

    private void OnEntityDied()
    {
        EntityDied?.Invoke(this);
        OnPlantDied();
        QueueFree();
    }

    protected virtual void OnPlantWork() { }

    protected virtual void OnPlantDied() { }
}
