using UnityEngine;

namespace LevelGridSystem.Data
{
    public class MovePosition
    {
        private MovePosition _previousMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public MovePosition(MovePosition previousMovePosition, Vector2Int gridPosition, Direction direction) {
            _previousMovePosition = previousMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition() {
            return gridPosition;
        }

        public Direction GetDirection() {
            return direction;
        }

        public Direction GetPreviousDirection() {
            if (_previousMovePosition == null) {
                return Direction.Right;
            } else {
                return _previousMovePosition.direction;
            }
        }
    }
}