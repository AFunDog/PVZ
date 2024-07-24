using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 植物大战僵尸.Entities;

namespace 植物大战僵尸.Structs
{
    public enum PutPlantResult
    {
        Success,
        PosInvaid,
        HasPlant,
    }

    public enum RemovePlantResult
    {
        Success,
        PosInvaid,
        NoPlant,
    }

    public class GameMap
    {
        const int MapWidth = 9;
        const int MapHeight = 5;
        public 植物?[,] Map { get; } = new 植物?[MapWidth, MapHeight];

        //public double Progress { get; set; } = 100;
        public List<IZombie> Zombies { get; } = [];

        public event Action? GameLose;
        public event Action? GameWin;

        public PutPlantResult PutPlant(植物 plant, int x, int y)
        {
            if (x >= MapWidth || y >= MapHeight || x < 0 || y < 0)
                return PutPlantResult.PosInvaid;
            if (Map[x, y] is not null)
                return PutPlantResult.HasPlant;
            Map[x, y] = plant;
            plant.X = x;
            plant.Y = y;
            plant.EntityDied += OnPlantDied;
            return PutPlantResult.Success;
        }

        public RemovePlantResult RemovePlant(int x, int y)
        {
            if (x >= MapWidth || y >= MapHeight || x < 0 || y < 0)
                return RemovePlantResult.PosInvaid;
            if (Map[x, y] is null)
                return RemovePlantResult.NoPlant;

            Map[x, y]!.QueueFree();
            Map[x, y] = null;
            return RemovePlantResult.Success;
        }

        public void AddZombie(IZombie zombie)
        {
            Zombies.Add(zombie);
            zombie.MoveToHome += OnZombieMoveToHome;
        }

        private void OnZombieMoveToHome(IZombie zombie)
        {
            GameLose?.Invoke();
            GameLose = null;
        }

        private void OnPlantDied(IEntity entity)
        {
            var plant = (IPlant)entity!;
            Map[plant.X, plant.Y] = null;
        }
    }
}
