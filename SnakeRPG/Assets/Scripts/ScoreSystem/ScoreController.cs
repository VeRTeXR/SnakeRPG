using System;
using System.Linq;
using System.Net;
using CaravanSystem;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using ScoreSystem.Signal;
using SpawnerSystem.Data;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    
    public class ScoreController : MonoBehaviour, ISubscriber
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
