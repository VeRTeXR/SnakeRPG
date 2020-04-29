using System.Collections.Generic;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using EngageSystem;
using LevelGridSystem;
using LevelGridSystem.Data;
using ScoreSystem.Signal;
using SpawnerSystem;
using SpawnerSystem.Data;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;
using Direction = LevelGridSystem.Data.Direction;

namespace CaravanSystem
{
    public class CaravanController : MonoBehaviour, IBroadcaster, ISubscriber
    {
        private Direction _currentDirection;
        private float _moveTimerMax = 0.5f;
        private float _increaseSpeedStep = 0.025f;
        private float _moveTimerCurrent;
        private List<MovePosition> _movePositionList;
        private Vector2Int _currentPositionOnGrid;
        private LevelGrid _levelGrid;

        private List<HeroEntity> _heroInCaravans = new List<HeroEntity>();
        private HeroSpawner _heroSpawner;
        private EnemySpawner _enemySpawner;

        private HeroEntity _leadingEntity;

        private bool _isCaravanActive;

        private int _currentScore;

        private void Awake()
        {
            _currentPositionOnGrid = new Vector2Int(5, 5);
            _moveTimerCurrent = _moveTimerMax;
            _currentDirection = Direction.Right;
            _movePositionList = new List<MovePosition>();

            Signaler.Instance.Subscribe<EngageEnemySequenceFinish>(this, OnEngageEnemySequenceFinished);
            _isCaravanActive = true;
        }

        private bool OnEngageEnemySequenceFinished(EngageEnemySequenceFinish signal)
        {
            if(_heroInCaravans.Count > 0)
                UnpauseCaravan();
            
            return true;
        }

        private void UnpauseCaravan()
        {
            _isCaravanActive = true;
        }

        public void Setup(LevelGrid level, HeroSpawner heroSpawner, EnemySpawner enemySpawner)
        {
            _levelGrid = level;
            _heroSpawner = heroSpawner;
            _enemySpawner = enemySpawner;

            var defaultHero = new GameObject("DefaultHero", typeof(SpriteRenderer));
            var currentEntity = defaultHero.AddComponent<HeroEntity>();
            currentEntity.Setup(heroSpawner.GetRandomHeroSprite());
            _heroInCaravans.Add(currentEntity);
            _leadingEntity = currentEntity;
            _leadingEntity.EntityData = currentEntity.EntityData;
        }

        private void Update()
        {
            if (_isCaravanActive)
            {
                ProcessLeadHeroSwitching();
                ProcessMovementInput();
                ProcessMovement();
            }
        }

        private void ProcessLeadHeroSwitching()
        {
            if (Input.GetKeyDown(KeyCode.Q)) SwitchHeroLeft();
            if (Input.GetKeyDown(KeyCode.E)) SwitchHeroRight();
        }

        private void SwitchHeroRight()
        {
            if (_heroInCaravans.Count > 1) 
                SwapAvatar(2);
        }


        private void SwitchHeroLeft()
        {
            if (_heroInCaravans.Count > 1) 
                SwapAvatar(_heroInCaravans.Count);
        }

        private void SwapAvatar(int nextAvatarIndex)
        {
            _heroInCaravans.Move(0,nextAvatarIndex-1);
            UpdateLeadingHero();
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
            if (_movePositionList.Count > 0) previousPositionOnGrid = _movePositionList[0];

            var movePosition = new MovePosition(previousPositionOnGrid, _currentPositionOnGrid, _currentDirection);
            _movePositionList.Insert(0, movePosition);

            var gridMoveDirectionVector = ProcessMoveStep();

            _currentPositionOnGrid += gridMoveDirectionVector;

            var directionChange = _levelGrid.ValidateGridPosition(_currentPositionOnGrid);

            if (directionChange != null)
            {
                RemoveCurrentHero();
                if (_heroInCaravans.Count <= 0)
                {
                    TriggerGameOver();
                    PauseCaravan();
                    return;
                }

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
                EngageEnemy();
        }

        private void EngageEnemy()
        {
            var enemyEntity = _enemySpawner.GetEnemyEntityFromGridPos(_currentPositionOnGrid);
            Signaler.Instance.Broadcast(this, new EngageEnemySequence
            {
                EnemyEntity = enemyEntity, HeroEntity = _leadingEntity
            });

            ProcessEngagement(enemyEntity);
            PauseCaravan();
        }

        private void ProcessEngagement(EnemyEntity enemyEntity)
        {
            var engagedHeroData = _leadingEntity.EntityData;
            var enemyData = enemyEntity.EntityData;

            CombatHandler.AttackTarget(engagedHeroData, enemyData);

            if (enemyData.HealthPoint > 0)
            {
                CombatHandler.AttackTarget(enemyData, engagedHeroData);
                if (engagedHeroData.HealthPoint <= 0)
                {
                    RemoveCurrentHero();
                    if (_heroInCaravans.Count <= 0)
                    {
                        TriggerGameOver();
                        PauseCaravan();
                    }
                }
            }
            else
            {
                Signaler.Instance.Broadcast(this, new EnemyKilled {HeroesInCaravan = _heroInCaravans});
                _levelGrid.RemoveEnemyFromPosition(_currentPositionOnGrid);
                _enemySpawner.RemoveEnemyEntityFromGridPos(_currentPositionOnGrid);
                Destroy(enemyEntity.gameObject);
                
                _moveTimerMax -= _increaseSpeedStep;
            }
        }

        private void TriggerGameOver()
        {
            PauseCaravan();
            Signaler.Instance.Broadcast(this, new GameOver());
        }


        private void PauseCaravan()
        {
            _isCaravanActive = false;
        }

        private void RemoveCurrentHero()
        {
            if (_heroInCaravans.Count <= 1)
            {
                Destroy(_heroInCaravans[0].gameObject);
                _heroInCaravans.RemoveAt(0);
                return;
            }

            _heroInCaravans.Move(0, 1);
            UpdateLeadingHero();
            
            var nextAvatar = _heroInCaravans[1];
            _heroInCaravans.RemoveAt(1);
            Destroy(nextAvatar.gameObject);
        }

        private void UpdateLeadingHero()
        {
            _leadingEntity = _heroInCaravans[0];
            _leadingEntity.EntityData = _heroInCaravans[0].EntityData;
            
            _leadingEntity.GetComponent<SpriteRenderer>().sprite = _heroInCaravans[0].EntityData.Sprite;
        }

        private void AddHeroToCaravan()
        {
            var collidedHero = _heroSpawner.GetHeroEntityFromGridPos(_currentPositionOnGrid);
            _heroInCaravans.Add(collidedHero);
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
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                if(_currentDirection == Direction.Up)    
                    _currentDirection = Direction.Left;
                else if (_currentDirection == Direction.Down)
                    _currentDirection = Direction.Right;
                else if (_currentDirection == Direction.Left)
                    _currentDirection = Direction.Down;
                else if (_currentDirection == Direction.Right)
                    _currentDirection = Direction.Up;

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                if (_currentDirection == Direction.Down)
                    _currentDirection = Direction.Left;
                else if (_currentDirection == Direction.Up)
                    _currentDirection = Direction.Right;
                else if (_currentDirection == Direction.Right)
                    _currentDirection = Direction.Down;
                else if (_currentDirection == Direction.Left)
                    _currentDirection = Direction.Up;

        }
    }
}