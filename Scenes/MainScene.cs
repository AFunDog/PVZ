using System;
using Godot;
using 植物大战僵尸.Helpers;

public partial class MainScene : Control
{
    private Control? _backgroundRect;

    private AudioStreamPlayer _buttonTabMusic;

    public override void _Ready()
    {
        _backgroundRect = GetNode<Control>("%BackgroundRect");

        _buttonTabMusic = GetNode<AudioStreamPlayer>("%ButtonTabMusic");

        ToStartState();
    }

    private void ToStartState() { }

    private void StartButton_Pressed()
    {
        _buttonTabMusic.Play();
        GetTree().ChangeSceneToPacked(SceneLibrary.GameScenePackage);
    }
}
