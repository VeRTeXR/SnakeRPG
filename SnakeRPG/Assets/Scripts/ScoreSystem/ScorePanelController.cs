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
        private CanvasGroup _scoreTextCanvasGroup;
        
        private void Awake()
        {
            Signaler.Instance.Subscribe<EnemyKilled>(this, OnEnemyKilled);
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);

            _scoreTextCanvasGroup = scoreText.GetComponent<CanvasGroup>();
        }

        private bool OnGameOver(GameOver signal)
        {
            return true;
        }

        private bool OnEnemyKilled(EnemyKilled signal)
        {
            var totalScoreIncrement = signal.HeroesInCaravan.Sum(heroEntity => heroEntity.EntityData.HealthPoint);
            _currentScore += totalScoreIncrement;
            AnimateEnter(); 
            return true;
        }

        private void AnimateEnter()
        {
            scoreText.text = _currentScore.ToString();
            LeanTween.scale(scoreText.rectTransform, new Vector3(1.3f,1.3f,1.3f), 0.4f).setOnComplete(() =>
                {
                    LeanTween.scale(scoreText.rectTransform, Vector3.one, 0.4f);
                });
        }
    }
}
