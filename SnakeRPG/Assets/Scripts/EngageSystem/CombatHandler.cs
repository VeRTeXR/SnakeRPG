using SpawnerSystem.Data;
using UnityEngine;

namespace EngageSystem
{
    public static class CombatHandler 
    {
        public static void AttackTarget(BoardEntityData source, BoardEntityData target)
        {
            var typeAttackMultiplier = 1;
            
            if (source.Element == target.Element) typeAttackMultiplier = 2;
            var modDamage = source.AttackPoint * typeAttackMultiplier;
            var playerDamage = modDamage - target.DefensePoint;
            
            playerDamage = Mathf.Clamp(playerDamage, 1, int.MaxValue);
            Debug.LogError("player dam : "+playerDamage);
            var enemyRemainingHealth = target.HealthPoint - playerDamage; 
            target.HealthPoint = enemyRemainingHealth;
        }
    }
}
