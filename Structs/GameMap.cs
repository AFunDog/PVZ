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
        public IPlant?[,] Map { get; } = new IPlant?[MapWidth, MapHeight];

        public List<IZombie> Zombies { get; } = new();

        public PutPlantResult PutPlant(IPlant plant, int x, int y)
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
            Map[x, y] = null;
            return RemovePlantResult.Success;
        }

        private void OnPlantDied(IEntity entity)
        {
            var plant = (IPlant)entity!;
            Map[plant.X, plant.Y] = null;
        }
    }
}
