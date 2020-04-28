using System.Linq;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using ScoreSystem.Signal;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    
    public class ScorePanelController : MonoBehaviour, ISubscriber
    {
        private int _currentScore;
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Awake()
        {
            Signaler.Instance.Subscribe<EnemyKilled>(this, OnEnemyKilled);
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
        }

        private bool OnGameOver(GameOver signal)
        {
            _currentScore = 0;
            return true;
        }

        private bool OnEnemyKilled(EnemyKilled signal)
        {
            var totalScoreIncrement = signal.HeroesInCaravan.Sum(heroEntity => heroEntity.EntityData.HealthPoint);
            _currentScore += totalScoreIncrement;
            scoreText.text = _currentScore.ToString(); 
            return true;
        }
    }
}
