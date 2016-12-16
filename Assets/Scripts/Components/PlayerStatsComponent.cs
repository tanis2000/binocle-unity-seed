using Binocle.Components;
using System.Collections.Generic;

namespace App
{
    public class PlayerStatsComponent : BaseMonoBehaviour
    {
        public float ElapsedTime = 0;
        public int DeadImmigrants = 0;
        public int AliveImmigrants = 0;
        public int TransferredImmigrants = 0;
        public int Money = 0;
        public int Food = 0;
        public List<UnitComponent> Boats = new List<UnitComponent>();
        public List<UnitComponent> CoastGuards = new List<UnitComponent>();
        public List<UnitComponent> Immigrants = new List<UnitComponent>();
        public List<UnitComponent> Shelters = new List<UnitComponent>();
        public int SpotsPerShelter = 1;
        public int GameOverDeadImmigrants = 10;

    }
}
