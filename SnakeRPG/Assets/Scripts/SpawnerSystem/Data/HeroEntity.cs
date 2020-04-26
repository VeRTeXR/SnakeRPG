using System;
using UnityEngine;

namespace SpawnerSystem.Data
{
    public class HeroEntity : MonoBehaviour
    {
        public Sprite HeroSprite;
        public int HealthPoint;
        public int AttackPoint;
        public int DefensePoint;
        public ElementType Element;
        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Setup(Sprite heroSprite)
        {
            _spriteRenderer.sprite = heroSprite;
            HeroSprite = heroSprite;
        }
    }
}
