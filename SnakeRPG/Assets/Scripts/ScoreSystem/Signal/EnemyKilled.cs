using System.Collections.Generic;
using SpawnerSystem.Data;

namespace ScoreSystem.Signal
{
    public struct EnemyKilled
    {
        public List<HeroEntity> HeroesInCaravan;
    }
}