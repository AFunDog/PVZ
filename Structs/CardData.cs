using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 植物大战僵尸.Structs
{
    public record CardData(
        string Name,
        string TexturePath,
        int Cost,
        double Cd,
        EntityInfo EntityInfo
    );
}
