using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 植物大战僵尸.Structs;

namespace 植物大战僵尸.Helpers
{
    public static class CardDataLibrary
    {
        public static List<CardData> CardDatas { get; } =
            [
                new CardData(
                    "向日葵",
                    AssetLibrary.卡牌.向日葵,
                    50,
                    7.5,
                    new("向日葵", "res://Entities/植物/向日葵.tscn")
                ),
                new CardData(
                    "豌豆射手",
                    AssetLibrary.卡牌.豌豆射手,
                    100,
                    7.5,
                    new("豌豆射手", "res://Entities/植物/豌豆射手.tscn")
                ),
                new CardData(
                    "寒冰射手",
                    AssetLibrary.卡牌.寒冰射手,
                    175,
                    7.5,
                    new("寒冰射手", "res://Entities/植物/寒冰射手.tscn")
                )
            ];
    }
}
