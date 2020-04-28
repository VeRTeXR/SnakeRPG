using System;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA.Input;

namespace GameOverPanel
{
    public class GameOverPanelController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private CanvasGroup _scoreTextCanvasGroup;
        
        private void Awake()
        {
            _scoreTextCanvasGroup = scoreText.GetComponent<CanvasGroup>();
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
            
            scoreText.gameObject.SetActive(false);
        }

        private bool OnGameOver(GameOver signal)
        {
            scoreText.gameObject.SetActive(true);
            _scoreTextCanvasGroup.alpha = 0;
            scoreText.text = "GAME OVER";
            var seq = LeanTween.sequence();
            seq.append(LeanTween.alphaCanvas(_scoreTextCanvasGroup, 1, 0.4f));
            seq.append(0.5f);
            seq.append(() =>
            {
                Signaler.Instance.Purge();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
            return true;
        }
    }
}
