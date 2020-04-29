using UnityEngine;

namespace SpawnerSystem.Data
{
    public class EnemyEntity : MonoBehaviour
    {

        public BoardEntityData EntityData;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Setup(Sprite enemySprite)
        {
            EntityData = new BoardEntityData
            {
                Sprite = enemySprite,
                HealthPoint = Random.Range(1, 5),
                AttackPoint = Random.Range(1, 5),
                DefensePoint = Random.Range(1, 5),
                Element = (ElementType) Random.Range(0, 3),
            };
            _spriteRenderer.sprite = enemySprite;
        }
    }
}

