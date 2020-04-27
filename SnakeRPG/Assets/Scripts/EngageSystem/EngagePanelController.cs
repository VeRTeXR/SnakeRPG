using CaravanSystem.Signal;
using echo17.Signaler.Core;
using SpawnerSystem.Data;
using UnityEngine;

namespace EngageSystem
{
    public class EngagePanelController : MonoBehaviour, ISubscriber
    {

        [SerializeField] private EntityStatDisplay heroStatDisplay; 
        [SerializeField] private EntityStatDisplay enemyStatDisplay;
        [SerializeField] private Transform layout; 
        private void Awake()
        {
            Signaler.Instance.Subscribe<EngageEnemySequence>(this, OnEngageEnemySequence);
        }

        private bool OnEngageEnemySequence(EngageEnemySequence signal)
        {
            DisplayEnemyStat(signal.EnemyEntity.EntityData);
            DisplayHeroStat(signal.HeroEntity.EntityData);
            return true;
        }

        private void DisplayHeroStat(BoardEntityData signalHeroEntity)
        {
            heroStatDisplay.Setup(signalHeroEntity);
        }

        private void DisplayEnemyStat(BoardEntityData signalEnemyEntity)
        {
            enemyStatDisplay.Setup(signalEnemyEntity);
        }
    }
}
