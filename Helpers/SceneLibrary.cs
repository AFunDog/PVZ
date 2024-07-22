using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace 植物大战僵尸.Helpers
{
    public static class SceneLibrary
    {
        public const string LoadingScene = "res://Scenes/LoadingScene.tscn";
        public const string MainScene = "res://Scenes/MainScene.tscn";
        public const string GameScene = "res://Scenes/GameScene.tscn";

        public static readonly PackedScene LoadingScenePackage = GD.Load<PackedScene>(LoadingScene);
        public static readonly PackedScene MainScenePackage = GD.Load<PackedScene>(MainScene);
        public static readonly PackedScene GameScenePackage = GD.Load<PackedScene>(GameScene);
    }
}
