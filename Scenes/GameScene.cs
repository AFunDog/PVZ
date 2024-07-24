using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using 植物大战僵尸.Entities;
using 植物大战僵尸.Helpers;
using 植物大战僵尸.Structs;

public partial class GameScene : Control
{
    private TextureRect? _backgroundRect;
    private Label? _sunShineLabel;
    private Control? _gameMapContainer;
    private Camera2D? _gameCamera;
    private Control? _topRectContainer;
    private 卡牌栏? _cardToolbar;
    private Control? _choseCardRect;
    private Container? _choseCardBackground;
    private Control? _cardContainer;
    private Control? _plantCardRect;
    private TextureButton? _shovelBox;

    private Timer? _sunShinetimer;
    private Timer? _zombieTimer;

    private readonly GameMap _gameMap = new();
    private int _sunShine = 100;

    public bool IsGameStart { get; private set; }

    public int SunShine
    {
        get => _sunShine;
        set
        {
            _sunShine = value;
            if (_sunShineLabel is null)
                return;
            _sunShineLabel.Text = $"{_sunShine}";
        }
    }

    private static readonly PackedScene _sunShinePackage = GD.Load<PackedScene>(
        "res://Entities/阳光.tscn"
    );
    private static readonly PackedScene _cardPackage = GD.Load<PackedScene>(
        "res://Controls/卡牌/卡牌.tscn"
    );

    public override void _Ready()
    {
        GetTree().NodeAdded += (e) =>
        {
            if (e is 阳光 sun)
            {
                sun.Click += OnSunShineClick;
            }
        };

        _sunShinetimer = new Timer();
        _sunShinetimer.Timeout += GenerateSunShine;
        CallDeferred(MethodName.AddChild, _sunShinetimer);

        _zombieTimer = new Timer();
        _zombieTimer.Timeout += GenerateZombie;
        CallDeferred(MethodName.AddChild, _zombieTimer);

        _backgroundRect = GetNode<TextureRect>("%BackgroundRect");
        _backgroundRect.Texture = GD.Load<Texture2D>(AssetLibrary.场景.白天);

        _sunShineLabel = GetNode<Label>("%SunShineLabel");

        _gameMapContainer = GetNode<Control>("%GameMapContainer");
        _gameMapContainer.GuiInput += GameMapContainer_GuiInput;

        _gameCamera = GetNode<Camera2D>("%GameCamera");

        _topRectContainer = GetNode<Control>("%TopRectContainer");
        _cardToolbar = GetNode<卡牌栏>("%CardToolBar");
        _cardToolbar.CardClick += CardToolBarCardClicked_BeforeGameStart;

        _choseCardRect = GetNode<Control>("%ChoseCardRect");
        _choseCardBackground = GetNode<Container>("%ChoseCardBackground");

        _cardContainer = GetNode<Control>("%CardContainer");
        _plantCardRect = GetNode<Control>("%PlantCardRect");

        _shovelBox = GetNode<TextureButton>("%ShovelBox");

        SetCardData();
        Ready += ToChoseCardState;
    }

    public override void _Input(InputEvent @event)
    {
        base._GuiInput(@event);
        switch (@event)
        {
            case InputEventMouseMotion mouseMotion:
                if (_isPlanting && _showPlant is not null)
                {
                    _showPlant.Position = mouseMotion.Position;
                }
                break;
            case InputEventMouseButton mouseButton:
                if (
                    mouseButton.ButtonIndex == MouseButton.Right
                    && mouseButton.Pressed
                    && _isPlanting
                )
                {
                    CancelPlant();
                }
                break;
            default:
                break;
        }
    }

    private void GameMapContainer_GuiInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton mouseButton:
                if (
                    mouseButton.ButtonIndex == MouseButton.Left
                    && mouseButton.Pressed
                    && _isPlanting
                )
                {
                    using PackedScene scene = GD.Load<PackedScene>(
                        _selectedCard!.CardData!.EntityInfo.ScenePath
                    );
                    var plant = scene.Instantiate<AnimatedSprite2D>();
                    var pos = (Vector2I)mouseButton.Position / new Vector2I(90, 110);
                    GD.Print(pos);
                    if (
                        _gameMap.PutPlant((plant as IPlant)!, pos.X, pos.Y)
                        == PutPlantResult.Success
                    )
                    {
                        SunShine -= _selectedCard.CardData.Cost;
                        _selectedCard.RestCd = _selectedCard.CardData.Cd;

                        plant.Position = pos * new Vector2I(90, 110) + new Vector2I(45, 55);
                        _gameMapContainer?.CallDeferred(MethodName.AddChild, plant);
                        plant.Play();
                    }

                    CancelPlant();
                }
                break;
            default:
                break;
        }
    }

    private void SetCardData()
    {
        for (int i = 0; i < CardDataLibrary.CardDatas.Count; i++)
        {
            var c = CardDataLibrary.CardDatas[i];
            var card = _cardPackage.Instantiate<卡牌>();
            card.CardData = c;
            card.Click += OnCardClicked;
            _cardContainer?.CallDeferred(MethodName.AddChild, card);
            var index = i;
            card.TreeEntered += () =>
            {
                card.Position = (_choseCardBackground!.GetChild(index) as Control)!.Position;
            };
        }
    }

    private async void ToChoseCardState()
    {
        if (_gameCamera is null || _topRectContainer is null || _choseCardRect is null)
            return;

        const double DurationS = 2;

        await Task.Delay(1000);
        var tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(
            _gameCamera,
            Camera2D.PropertyName.Position.ToString(),
            _gameCamera.Position with
            {
                X = _gameCamera.Position.X + 361
            },
            DurationS
        );
        tween
            .Parallel()
            .TweenProperty(
                _topRectContainer,
                Container.PropertyName.Position.ToString(),
                _topRectContainer.Position with
                {
                    X = _topRectContainer.Position.X + 361
                },
                DurationS
            );
        tween.TweenProperty(
            _topRectContainer,
            Container.PropertyName.Position.ToString(),
            _topRectContainer.Position with
            {
                X = _topRectContainer.Position.X + 361,
                Y = 0
            },
            0.8
        );
        tween
            .Parallel()
            .TweenProperty(
                _choseCardRect,
                Container.PropertyName.Position.ToString(),
                _choseCardRect.Position with
                {
                    Y = 88
                },
                0.8
            );
    }

    private void ToStartGameState()
    {
        if (
            _choseCardRect is null
            || _topRectContainer is null
            || _gameCamera is null
            || _sunShinetimer is null
            || _zombieTimer is null
        )
            return;

        _cardToolbar!.CardClick -= CardToolBarCardClicked_BeforeGameStart;
        for (int i = 0; i < _cardToolbar.Cards.Count; i++)
        {
            _cardToolbar.Cards[i].RestCd = _cardToolbar.Cards[i].CardData!.Cd;
        }

        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(
            _choseCardRect,
            PropertyName.Position.ToString(),
            _choseCardRect.Position with
            {
                Y = 648
            },
            0.6
        );
        tween.TweenProperty(
            _topRectContainer,
            PropertyName.Position.ToString(),
            _topRectContainer.Position with
            {
                X = _topRectContainer.Position.X - 361
            },
            1.5
        );
        tween
            .Parallel()
            .TweenProperty(
                _gameCamera,
                PropertyName.Position.ToString(),
                _gameCamera.Position with
                {
                    X = _gameCamera.Position.X - 361
                },
                1.5
            );
        tween.Finished += () =>
        {
            _cardToolbar.CardClick += CardToolBarCardClicked_OnGameStart;
            _cardToolbar.StartGame();
            _sunShinetimer.Start(10);
            _zombieTimer.Start(20);
        };
        IsGameStart = true;
    }

    private void GenerateSunShine()
    {
        var pos = Random.Shared.Next(120, 720);
        var targetY = Random.Shared.Next(120, 480);
        var sun = _sunShinePackage.Instantiate<阳光>();
        sun.Position = new Vector2(pos, -16);
        sun.TargetY = targetY;
        CallDeferred(Control.MethodName.AddChild, sun);
    }

    private void GenerateZombie()
    {
        using PackedScene zombieScene = GD.Load<PackedScene>("res://Entities/僵尸/僵尸.tscn");
        Span<int> collect = [110, 220, 330, 440, 550];
        var posY = Random.Shared.GetItems<int>(collect, 1)[0];
        var zombie = zombieScene.Instantiate<Node2D>();
        zombie.Position = new Vector2(1280, posY);
        CallDeferred(MethodName.AddChild, zombie);
        _zombieTimer!.WaitTime = Math.Max(10, _zombieTimer.WaitTime * 0.9);
    }

    public void OnSunShineClick(阳光 sun)
    {
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(sun, PropertyName.GlobalPosition.ToString(), new Vector2(35, 35), 0.5);
        tween.Finished += () =>
        {
            SunShine += 25;
            sun.QueueFree();
        };
    }

    private void MainMenuButtonClicked()
    {
        GetTree().ChangeSceneToPacked(SceneLibrary.MainScenePackage);
    }

    private void CardToolBarCardClicked_BeforeGameStart(卡牌 card)
    {
        var pos = card.GlobalPosition;
        _cardToolbar?.CallDeferred(Control.MethodName.RemoveChild, card);
        _cardContainer?.CallDeferred(Control.MethodName.AddChild, card);
        card.Click += OnCardClicked;
        card.TreeEntered += OnCardEntered;
        void OnCardEntered()
        {
            card.TreeEntered -= OnCardEntered;
            card.GlobalPosition = pos;

            Tween tween = CreateTween();
            GD.Print(_choseCardBackground!.GetChildCount());
            GD.Print(CardDataLibrary.CardDatas.IndexOf(card.CardData!));
            tween.TweenProperty(
                card,
                PropertyName.GlobalPosition.ToString(),
                _choseCardBackground!
                    .GetChild<Control>(CardDataLibrary.CardDatas.IndexOf(card.CardData!))
                    .GlobalPosition,
                0.2
            );
        }
        //void OnCardExited()
        //{

        //}
    }

    private bool _isPlanting = false;
    private 卡牌? _selectedCard = null;
    private Node2D? _showPlant = null;

    private void CardToolBarCardClicked_OnGameStart(卡牌 card)
    {
        GD.Print($"{nameof(CardToolBarCardClicked_OnGameStart)} {card.RestCd}");
        if (card.RestCd == 0 && card.CardData!.Cost <= SunShine)
        {
            if (_isPlanting)
            {
                CancelPlant();
            }
            else
            {
                TryToPlant(card);
            }
        }
        else { }
    }

    private void TryToPlant(卡牌 card)
    {
        _isPlanting = true;
        _selectedCard = card;
        using PackedScene scene = GD.Load<PackedScene>(card.CardData!.EntityInfo.ScenePath);
        _showPlant = scene.Instantiate<Node2D>();
        _showPlant.GlobalPosition = GetViewport().GetMousePosition();
        card.Modulate = Color.Color8(255, 255, 255, 80);
        CallDeferred(MethodName.AddChild, _showPlant);
    }

    private void CancelPlant()
    {
        if (_selectedCard is null)
            return;

        _showPlant?.QueueFree();
        _showPlant = null;
        _selectedCard.Modulate = Colors.White;
        _isPlanting = false;
    }

    private void OnCardClicked(卡牌 card)
    {
        var pos = card.GlobalPosition;
        _cardContainer?.CallDeferred(MethodName.RemoveChild, card);
        _cardToolbar?.AddCard(card, pos);
        card.Click -= OnCardClicked;
    }

    private void OnStartGameButtonClicked()
    {
        ToStartGameState();
    }

    private void OnShovelBoxClicked(bool toggled)
    {
        if (!IsGameStart)
            return;

        if (toggled)
        {
            _shovelBox!.TextureNormal = GD.Load<Texture2D>("res://Assets/组件/无铲子盒.png");
            var image = GD.Load<Texture2D>("res://Assets/组件/铲子.png");
            Input.SetCustomMouseCursor(image, hotspot: image.GetSize() / 2);
        }
        else
        {
            _shovelBox!.TextureNormal = GD.Load<Texture2D>("res://Assets/组件/铲子盒.png");
            Input.SetCustomMouseCursor(null);
        }
    }
}
