using UnityEngine;

namespace SpawnerSystem.Data
{
    public class EnemyEntity : MonoBehaviour
    {
        public Sprite EnemySprite;
        public int HealthPoint;
        public int AttackPoint;
        public int DefensePoint;
        public ElementType Element;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void Setup(Sprite enemySprite)
        {
            _spriteRenderer.sprite = enemySprite;
            EnemySprite = enemySprite;
            HealthPoint = Random.Range(1, 5);
            AttackPoint = Random.Range(1, 5);
            DefensePoint = Random.Range(1, 5);
            Element = (ElementType) Random.Range(0, 3);            
        }
    }
}

