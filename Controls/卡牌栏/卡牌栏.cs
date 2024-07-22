using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class 卡牌栏 : Control
{
    public event Action<卡牌>? CardClick;

    private readonly List<卡牌> _cards = [];

    private Container? _topCardBackground;

    private bool _isGameStarted = false;

    public List<卡牌> Cards => _cards;

    public override void _Ready()
    {
        _topCardBackground = GetNode<Container>("%TopCardBackground");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isGameStarted)
            return;

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].RestCd > 0)
            {
                _cards[i].RestCd -= delta;
            }
        }
    }

    public bool AddCard(卡牌 card, Vector2 sourcePos)
    {
        if (GetChildCount() >= _topCardBackground!.GetChildCount())
            return false;

        CallDeferred(MethodName.AddChild, card);
        card.TreeEntered += OnCardEntered;
        card.Click += OnCardClicked;
        void OnCardEntered()
        {
            _cards.Add(card);
            card.TreeEntered -= OnCardEntered;
            card.TreeExited += OnCardLeave;
            card.GlobalPosition = sourcePos;
            card.ZIndex = 1;

            Tween tween = CreateTween();
            tween.TweenProperty(
                card,
                PropertyName.GlobalPosition.ToString(),
                _topCardBackground!.GetChild<Control>(_cards.Count - 1).GlobalPosition,
                0.2
            );
        }
        void OnCardLeave()
        {
            _cards.Remove(card);
            GD.Print("CardLeave");
            card.Click -= OnCardClicked;
            card.TreeExited -= OnCardLeave;
            card.GlobalPosition = sourcePos;
            card.ZIndex = 0;
        }

        return true;
    }

    public void StartGame()
    {
        foreach (卡牌 card in _cards)
        {
            card.RestCd = 0;
        }
        _isGameStarted = true;
    }

    private void OnCardClicked(卡牌 card)
    {
        CardClick?.Invoke(card);
        ResetPosition();
    }

    private void ResetPosition()
    {
        Tween tween = CreateTween();

        for (int i = 0; i < _cards.Count; i++)
        {
            tween
                .Parallel()
                .TweenProperty(
                    _cards[i],
                    Control.PropertyName.Position.ToString(),
                    _topCardBackground!.GetChild<Control>(i).Position,
                    0.2
                );
        }
    }
}
