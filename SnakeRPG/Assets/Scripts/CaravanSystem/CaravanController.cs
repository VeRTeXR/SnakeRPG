using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LevelGridSystem;
using LevelGridSystem.Data;
using SpawnerSystem;
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
        private HeroSpawner _heroSpawner;

        private void Awake()
        {
            _currentPositionOnGrid = new Vector2Int(5, 5);
            _moveTimerCurrent = _moveTimerMax;
            _currentDirection = Direction.Right;
            _movePositionList = new List<MovePosition>();
        }

        public void Setup(LevelGrid level, HeroSpawner heroSpawner) {
            _levelGrid = level;
            _heroSpawner = heroSpawner;
        }

        private void Update()
        {
            ProcessMovementInput();
            ProcessMovement();
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
            if (_movePositionList.Count > 0) {
                previousPositionOnGrid = _movePositionList[0];
            }
        

            var snakeMovePosition = new MovePosition( previousPositionOnGrid, _currentPositionOnGrid, _currentDirection);
            _movePositionList.Insert(0, snakeMovePosition);
        
            var gridMoveDirectionVector = ProcessMoveStep();
        
            _currentPositionOnGrid += gridMoveDirectionVector;

            var directionChange = _levelGrid.ValidateGridPosition(_currentPositionOnGrid);
       
            if (directionChange != null)
            {
                _currentPositionOnGrid -= gridMoveDirectionVector;
                _currentDirection = (Direction) directionChange;
                var changeDirectionStep = ProcessMoveStep();

                _currentPositionOnGrid += changeDirectionStep;

                var isCollideWithHero = _levelGrid.CheckHeroCollision(_currentPositionOnGrid);
                Debug.LogError(isCollideWithHero);
                
                AppliedPositionAndRotation(changeDirectionStep);
            }
            else
            {
                
                var isCollideWithHero = _levelGrid.CheckHeroCollision(_currentPositionOnGrid);
                if (isCollideWithHero)
                {
                    var collidedHero = _heroSpawner.GetHeroEntityFromGridPos(_currentPositionOnGrid);
                    Debug.LogError(collidedHero.HeroSprite.name);
                }
                Debug.LogError(isCollideWithHero);

                
                
                AppliedPositionAndRotation(gridMoveDirectionVector);
            }
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

        private static float GetAngleFromVector(Vector2Int dir) {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }
    
        public List<Vector2Int> GetCaravanGridPositionList() {
            var gridPositionList = new List<Vector2Int> { _currentPositionOnGrid };
            foreach (var movePosition in _movePositionList) 
                gridPositionList.Add(movePosition.GetGridPosition());
        
            return gridPositionList;
        }

    
        private void ProcessMovementInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                if (_currentDirection != Direction.Down)
                    _currentDirection = Direction.Up;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                if (_currentDirection != Direction.Up)
                    _currentDirection = Direction.Down;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                if (_currentDirection != Direction.Right)
                    _currentDirection = Direction.Left;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                if (_currentDirection != Direction.Left)
                    _currentDirection = Direction.Right;
        }
    }
}