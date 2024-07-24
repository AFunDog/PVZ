using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 植物大战僵尸.Entities
{
    public interface IBullet
    {
        int Damage { get; set; }

        bool IsCold { get; set; }
    }
}
