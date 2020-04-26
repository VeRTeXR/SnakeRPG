using System.Collections.Generic;
using CaravanSystem;
using LevelGridSystem.Data;
using SpawnerSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelGridSystem
{
    public class LevelGrid {

        private List<Vector2Int> _newHeroGridPosition = new List<Vector2Int>();
        private GameObject _heroGameObject;
        private int _width;
        private int _height;
        private CaravanController _caravanController;


        public LevelGrid(int width, int height) {
            _width = width;
            _height = height;
        }

        public Vector2Int GetLevelDimension()
        {
            return new Vector2Int(_width, _height);
        }

        public void Setup(CaravanController caravan)
        {
            _caravanController = caravan;
        }


        public bool CheckHeroCollision(Vector2Int gridPosition)
        {
            foreach (var gridPos in _newHeroGridPosition)
                if (gridPosition == gridPos)
                    return true;
            return false;
        }

        public Direction? ValidateGridPosition(Vector2Int gridPosition) {
            if (gridPosition.x < 0 || gridPosition.x > _width - 1)
                return gridPosition.y > _height / 2 ? Direction.Down : Direction.Up;

            if (gridPosition.y < 0 || gridPosition.y > _height - 1)
                return gridPosition.x > _width / 2 ? Direction.Left : Direction.Right;
            return null;
        }

        public void AppendNewHeroPosition(Vector2Int newHeroGridPosition)
        {
            _newHeroGridPosition.Add(newHeroGridPosition);
        }
    }
}