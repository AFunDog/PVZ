using System.Threading.Tasks;
using Godot;
using 植物大战僵尸.Helpers;

public partial class LoadingScene : Control
{
    private Range _progressBar;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        _progressBar = GetNode<Range>("LoadingProgressBar");
        var progress = new System.Progress<int>(i =>
        {
            _progressBar.Value = i;
            if (i == 100)
            {
                GetTree().ChangeSceneToPacked(SceneLibrary.MainScenePackage);
            }
        });
        await Loading(progress);
    }

    private async Task Loading(System.IProgress<int> progress)
    {
        for (int i = 0; i <= 100; i++)
        {
            await Task.Delay(10);
            progress.Report(i);
        }
    }
}
