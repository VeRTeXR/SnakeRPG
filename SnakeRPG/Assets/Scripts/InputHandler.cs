using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Direction _currentDirection;
    private float _moveTimerMax = 1f;
    private float _moveTimerCurrent;
    private List<MovePosition> _movePositionList;
    private Direction _moveDirection;
    private Vector2Int _currentPositionOnGrid;

    private void Awake()
    {
        _moveTimerCurrent = _moveTimerMax;
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

        Vector2Int gridMoveDirectionVector;
        switch (_currentDirection) {
            default:
            case Direction.Right:   gridMoveDirectionVector = new Vector2Int(+1, 0); break;
            case Direction.Left:    gridMoveDirectionVector = new Vector2Int(-1, 0); break;
            case Direction.Up:      gridMoveDirectionVector = new Vector2Int(0, +1); break;
            case Direction.Down:    gridMoveDirectionVector = new Vector2Int(0, -1); break;
        }

        _currentPositionOnGrid += gridMoveDirectionVector;

        // _currentPositionOnGrid = levelGrid.ValidateGridPosition(gridPosition);

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

public enum Direction {
    Left,
    Right,
    Up,
    Down
}