using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using LevelGridSystem;
using LevelGridSystem.Data;
using SpawnerSystem;
using SpawnerSystem.Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CaravanSystem
{
    public class CaravanController : MonoBehaviour, IBroadcaster, ISubscriber
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

        private bool _isCaravanActive;
        
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
            var currentEntity = gameObject.AddComponent<HeroEntity>();
            currentEntity.Setup(heroSpawner.GetRandomHeroSprite());
            _heroInCaravans.Add(currentEntity);
            _leadingEntity = currentEntity;
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
            _selectedHeroIndex++;
            if (_selectedHeroIndex > _heroInCaravans.Count - 1) _selectedHeroIndex = 0;    
            SwapAvatar(_selectedHeroIndex);
        }


        private void SwitchHeroLeft()
        {
            _selectedHeroIndex--;
            if (_selectedHeroIndex <= 0) _selectedHeroIndex = _heroInCaravans.Count - 1;
            SwapAvatar(_selectedHeroIndex);    
        }
        
        private void SwapAvatar(int nextAvatarIndex)
        {
            var nextAvatar = _heroInCaravans[Mathf.Clamp(nextAvatarIndex, 0, _heroInCaravans.Count)].gameObject
                .GetComponent<HeroEntity>();
            if (nextAvatar != null)
            {
                var tempData = _leadingEntity.EntityData;

                _leadingEntity.EntityData = nextAvatar.EntityData;
                _leadingEntity.GetComponent<SpriteRenderer>().sprite =
                    nextAvatar.gameObject.GetComponent<SpriteRenderer>().sprite;

                nextAvatar.EntityData = tempData;
                nextAvatar.gameObject.GetComponent<SpriteRenderer>().sprite = tempData.Sprite;
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
            if (_movePositionList.Count > 0) previousPositionOnGrid = _movePositionList[0];

            var movePosition = new MovePosition(previousPositionOnGrid, _currentPositionOnGrid, _currentDirection);
            _movePositionList.Insert(0, movePosition);

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
                EngageEnemy();
        }

        private void EngageEnemy()
        { 
            var enemyEntity =_enemySpawner.GetEnemyEntityFromGridPos(_currentPositionOnGrid);
            Signaler.Instance.Broadcast(this, new EngageEnemySequence
            {
                EnemyEntity = enemyEntity, HeroEntity = _leadingEntity
            });
            
            PauseCaravan();
        }

        private void PauseCaravan()
        {
            _isCaravanActive = false;
        }

        private void RemoveCurrentHero()
        {
            _selectedHeroIndex++;
            if (_selectedHeroIndex > _heroInCaravans.Count - 1) _selectedHeroIndex = 0;
            var nextAvatar = _heroInCaravans[Mathf.Clamp(_selectedHeroIndex, 0, _heroInCaravans.Count - 1)].gameObject
                .GetComponent<HeroEntity>();
            if (nextAvatar != null)
            {
                _leadingEntity.EntityData = nextAvatar.EntityData;
                _leadingEntity.GetComponent<SpriteRenderer>().sprite = nextAvatar.gameObject.GetComponent<SpriteRenderer>().sprite;
                _heroInCaravans.RemoveAt(_selectedHeroIndex);
                Destroy(nextAvatar.gameObject);
                _selectedHeroIndex = 0;
            }
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