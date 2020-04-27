using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LevelGridSystem;
using LevelGridSystem.Data;
using SpawnerSystem;
using SpawnerSystem.Data;
using UnityEngine;

namespace CaravanSystem
{
    public class CaravanController : MonoBehaviour
    {
        private Direction _currentDirection;
        private float _moveTimerMax = 0.5f;
        private float _moveTimerCurrent;
        private List<MovePosition> _movePositionList;
        private Vector2Int _currentPositionOnGrid;
        private LevelGrid _levelGrid;
        
        private List<HeroEntity> _heroInCaravans = new List<HeroEntity>();
        private HeroSpawner _heroSpawner;
        private EnemySpawner _enemySpawner;

        private HeroEntity _leadingEntity;
        private int _selectedHeroIndex = 0;
        
        private void Awake()
        {
            _currentPositionOnGrid = new Vector2Int(5, 5);
            _moveTimerCurrent = _moveTimerMax;
            _currentDirection = Direction.Right;
            _movePositionList = new List<MovePosition>();
        }

        public void Setup(LevelGrid level, HeroSpawner heroSpawner)
        {
            _levelGrid = level;
            _heroSpawner = heroSpawner;
            var currentEntity = gameObject.AddComponent<HeroEntity>();
            currentEntity.Setup(heroSpawner.GetRandomHeroSprite());
            _heroInCaravans.Add(currentEntity);
            _leadingEntity = currentEntity;
        }

        private void Update()
        {
            ProcessLeadHeroSwitching();
            ProcessMovementInput();
            ProcessMovement();
        }

        private void ProcessLeadHeroSwitching()
        {
            if (Input.GetKeyDown(KeyCode.Q)) SwitchHeroLeft();
            if (Input.GetKeyDown(KeyCode.E)) SwitchHeroRight();
        }

        private void SwitchHeroRight()
        {
            _selectedHeroIndex++;
            if (_selectedHeroIndex > _heroInCaravans.Count - 1) _selectedHeroIndex = 0;
            UpdateCurrentHero();
        }


        private void SwitchHeroLeft()
        {
            _selectedHeroIndex--;
            if (_selectedHeroIndex <= 0) _selectedHeroIndex = _heroInCaravans.Count - 1;
            UpdateCurrentHero();
        }

        private void UpdateCurrentHero()
        {
            SwapAvatar(_selectedHeroIndex);
        }

        private void SwapAvatar(int nextAvatarIndex)
        {
            var nextAvatar = _heroInCaravans[Mathf.Clamp(nextAvatarIndex, 0, _heroInCaravans.Count)].gameObject
                .GetComponent<HeroEntity>();
            if (nextAvatar != null)
            {
                var tempHealth = _leadingEntity.HealthPoint;
                var tempAttack = _leadingEntity.AttackPoint;
                var tempShield = _leadingEntity.DefensePoint;
                var tempType = _leadingEntity.Element;
                var tempSprite = _leadingEntity.GetComponent<SpriteRenderer>().sprite;

                _leadingEntity.HealthPoint = nextAvatar.HealthPoint;
                _leadingEntity.AttackPoint = nextAvatar.AttackPoint;
                _leadingEntity.DefensePoint = nextAvatar.DefensePoint;
                _leadingEntity.Element = nextAvatar.Element;
                _leadingEntity.GetComponent<SpriteRenderer>().sprite =
                    nextAvatar.gameObject.GetComponent<SpriteRenderer>().sprite;

                nextAvatar.HealthPoint = tempHealth;
                nextAvatar.AttackPoint = tempAttack;
                nextAvatar.DefensePoint = tempShield;
                nextAvatar.Element = tempType;
                nextAvatar.gameObject.GetComponent<SpriteRenderer>().sprite = tempSprite;
            }
        }

        private void ProcessMovement()
        {
            _moveTimerCurrent -= Time.deltaTime;
            if (_moveTimerCurrent <= 0)
            {
                MoveForward();
                _moveTimerCurrent = _moveTimerMax;
            }
        }

        private void MoveForward()
        {
            MovePosition previousPositionOnGrid = null;
            if (_movePositionList.Count > 0)
            {
                previousPositionOnGrid = _movePositionList[0];
            }


            var snakeMovePosition = new MovePosition(previousPositionOnGrid, _currentPositionOnGrid, _currentDirection);
            _movePositionList.Insert(0, snakeMovePosition);

            var gridMoveDirectionVector = ProcessMoveStep();

            _currentPositionOnGrid += gridMoveDirectionVector;

            var directionChange = _levelGrid.ValidateGridPosition(_currentPositionOnGrid);

            if (directionChange != null)
            {
                RemoveCurrentHero();
                
                _currentPositionOnGrid -= gridMoveDirectionVector;
                _currentDirection = (Direction) directionChange;
                var changeDirectionStep = ProcessMoveStep();

                _currentPositionOnGrid += changeDirectionStep;

                CollisionChecks();

                AppliedPositionAndRotation(changeDirectionStep);
            }
            else
            {
                CollisionChecks();

                AppliedPositionAndRotation(gridMoveDirectionVector);
            }

            UpdateCaravanMemberPosition();
        }

        private void CollisionChecks()
        {
            var isCollideWithHero = _levelGrid.CheckHeroCollision(_currentPositionOnGrid);
            if (isCollideWithHero)
                AddHeroToCaravan();

            var isCollideWithEnemy = _levelGrid.CheckEnemyCollision(_currentPositionOnGrid);
            if (isCollideWithEnemy)
                EngageWithEnemy();
        }

        private void EngageWithEnemy()
        {
            _enemySpawner.GetEnemyEntityFromGridPos(_currentPositionOnGrid);
            
        }

        private void RemoveCurrentHero()
        {
            _selectedHeroIndex++;
            if (_selectedHeroIndex > _heroInCaravans.Count - 1) _selectedHeroIndex = 0;
            var nextAvatar = _heroInCaravans[Mathf.Clamp(_selectedHeroIndex, 0, _heroInCaravans.Count - 1)].gameObject
                .GetComponent<HeroEntity>();
            if (nextAvatar != null)
            {
                _leadingEntity.HealthPoint = nextAvatar.HealthPoint;
                _leadingEntity.AttackPoint = nextAvatar.AttackPoint;
                _leadingEntity.DefensePoint = nextAvatar.DefensePoint;
                _leadingEntity.Element = nextAvatar.Element;
                _leadingEntity.GetComponent<SpriteRenderer>().sprite = nextAvatar.gameObject.GetComponent<SpriteRenderer>().sprite;
                _heroInCaravans.RemoveAt(_selectedHeroIndex);
                Destroy(nextAvatar.gameObject);
                _selectedHeroIndex = 0;
            }
        }

        private void AddHeroToCaravan()
        {
            var collidedHero = _heroSpawner.GetHeroEntityFromGridPos(_currentPositionOnGrid);
            _heroSpawner.RemoveHeroEntityFromGridPos(_currentPositionOnGrid);
            _heroInCaravans.Add(collidedHero);

            Debug.LogError(collidedHero.HeroSprite.name);
        }

        private void UpdateCaravanMemberPosition()
        {
            for (var i = 0; i < _heroInCaravans.Count; i++)
                _heroInCaravans[i].SetCaravanMemberMovePosition(_movePositionList[i]);
        }

        private void AppliedPositionAndRotation(Vector2Int gridMoveDirectionVector)
        {
            transform.position = new Vector3(_currentPositionOnGrid.x, _currentPositionOnGrid.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
        }

        private Vector2Int ProcessMoveStep()
        {
            Vector2Int gridMoveDirectionVector;
            switch (_currentDirection)
            {
                default:
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(+1, 0);
                    break;
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, +1);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
            }

            return gridMoveDirectionVector;
        }

        private static float GetAngleFromVector(Vector2Int dir)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }

        public List<Vector2Int> GetCaravanGridPositionList()
        {
            var gridPositionList = new List<Vector2Int> {_currentPositionOnGrid};
            foreach (var movePosition in _movePositionList)
                gridPositionList.Add(movePosition.GetGridPosition());

            return gridPositionList;
        }


        private void ProcessMovementInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                if (_currentDirection != Direction.Down)
                    _currentDirection = Direction.Up;

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                if (_currentDirection != Direction.Up)
                    _currentDirection = Direction.Down;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                if (_currentDirection != Direction.Right)
                    _currentDirection = Direction.Left;

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                if (_currentDirection != Direction.Left)
                    _currentDirection = Direction.Right;
        }
    }
}