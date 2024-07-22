using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 植物大战僵尸.Entities
{
    public interface IPlant : IEntity
    {
        int X { get; set; }
        int Y { get; set; }
        int Health { get; set; }
    }
}
