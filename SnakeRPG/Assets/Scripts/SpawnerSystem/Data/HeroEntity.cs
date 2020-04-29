using LevelGridSystem.Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnerSystem.Data
{
    public class HeroEntity : MonoBehaviour
    {
        public BoardEntityData EntityData;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

        }

        public void Setup(Sprite heroSprite)
        {
            _spriteRenderer.sprite = heroSprite;
            EntityData = new BoardEntityData
            {
                Sprite = heroSprite,
                HealthPoint = Random.Range(1, 5),
                AttackPoint = Random.Range(1, 5),
                DefensePoint = Random.Range(1, 5),
                Element = (ElementType) Random.Range(0, 3),
            };
        }

        public void SetCaravanMemberMovePosition(MovePosition movePosition)
        {
            transform.position = new Vector3(movePosition.GetGridPosition().x, movePosition.GetGridPosition().y);
            float angle;
            switch (movePosition.GetDirection())
            {
                default:
                case Direction.Up:
                    switch (movePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left:
                            angle = 0 + 45;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Right:
                            angle = 0 - 45;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                    }

                    break;
                case Direction.Down:
                    switch (movePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left:
                            angle = 180 - 45;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                        case Direction.Right:
                            angle = 180 + 45;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }

                    break;
                case Direction.Left:
                    switch (movePosition.GetPreviousDirection())
                    {
                        default:
                            angle = +90;
                            break;
                        case Direction.Down:
                            angle = 180 - 45;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                        case Direction.Up:
                            angle = 45;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }

                    break;
                case Direction.Right:
                    switch (movePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down:
                            angle = 180 + 45;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Up:
                            angle = -45;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                    }

                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
}