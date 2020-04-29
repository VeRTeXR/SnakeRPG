using CaravanSystem.Signal;
using echo17.Signaler.Core;
using SpawnerSystem.Data;
using UnityEngine;

namespace EngageSystem
{
    public class EngagePanelController : MonoBehaviour, ISubscriber,IBroadcaster
    {

        [SerializeField] private EntityStatDisplay heroStatDisplay; 
        [SerializeField] private EntityStatDisplay enemyStatDisplay;
        [SerializeField] private Transform layout; 
        private void Awake()
        {
            Signaler.Instance.Subscribe<EngageEnemySequence>(this, OnEngageEnemySequence);
            layout.gameObject.SetActive(false);
        }

        private bool OnEngageEnemySequence(EngageEnemySequence signal)
        {
            var sequence = LeanTween.sequence();
            sequence.append(() =>
            {
                layout.gameObject.SetActive(true);
                DisplayEnemyStat(signal.EnemyEntity.EntityData);
                DisplayHeroStat(signal.HeroEntity.EntityData);
            });
            sequence.append(0.8f);
            sequence.append(() => layout.gameObject.SetActive(false));
            sequence.append(BroadcastResolveEngage);
            
            return true;
        }

        private void BroadcastResolveEngage()
        {
            
            Signaler.Instance.Broadcast(this, new EngageEnemySequenceFinish());
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
