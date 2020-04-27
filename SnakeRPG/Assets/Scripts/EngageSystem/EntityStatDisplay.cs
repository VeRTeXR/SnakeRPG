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

        public void Setup(BoardEntityData boardEntityData)
        {
            sprite.sprite = boardEntityData.Sprite;
            healthText.text = boardEntityData.HealthPoint.ToString();
            attackPointText.text = boardEntityData.AttackPoint.ToString();
            defensePointText.text = boardEntityData.DefensePoint.ToString();
            typeText.text = boardEntityData.Element.ToString();
        }
    }
}