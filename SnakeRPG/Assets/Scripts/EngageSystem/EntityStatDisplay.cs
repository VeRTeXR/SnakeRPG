using System;
using SpawnerSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EngageSystem
{
    public class EntityStatDisplay : MonoBehaviour
    {
        [SerializeField] private Image sprite;
        [SerializeField] private TextMeshProUGUI healthText; 
        [SerializeField] private TextMeshProUGUI attackPointText; 
        [SerializeField] private TextMeshProUGUI defensePointText; 
        [SerializeField] private TextMeshProUGUI typeText; 
        [SerializeField] private TextMeshProUGUI statusText;
        private CanvasGroup _statusTextCanvasGroup;


        public void Setup(BoardEntityData boardEntityData)
        {
            
            _statusTextCanvasGroup = statusText.GetComponent<CanvasGroup>();
            _statusTextCanvasGroup.alpha = 0;
            
            var health = Mathf.Clamp(boardEntityData.HealthPoint, 0,Int32.MaxValue);
            healthText.text = "HP: "+health;
            
            sprite.sprite = boardEntityData.Sprite;
            attackPointText.text ="AP: "+boardEntityData.AttackPoint;
            defensePointText.text ="DP: "+boardEntityData.DefensePoint;
            typeText.text = "TYPE: "+boardEntityData.Element;

            if (health > 0) return;
            LeanTween.alphaCanvas(_statusTextCanvasGroup, 1, 0.4f);
            statusText.text = "DEAD";
            statusText.color = Color.red;
        }
    }
}