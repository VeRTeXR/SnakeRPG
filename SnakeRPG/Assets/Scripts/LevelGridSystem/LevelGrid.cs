using CaravanSystem;
using LevelGridSystem.Data;
using SpawnerSystem;
using UnityEngine;

namespace LevelGridSystem
{
    public class LevelGrid {

        private Vector2Int _newHeroGridPosition;
        private GameObject _heroGameObject;
        private int _width;
        private int _height;
        private CaravanController _caravanController;
        private HeroSpawner _heroSpawner;


        public LevelGrid(int width, int height) {
            _width = width;
            _height = height;
        }

        public Vector2Int GetLevelDimension()
        {
            return new Vector2Int(_width, _height);
        }

        public void Setup(CaravanController caravan, HeroSpawner heroSpawner)
        {
            _heroSpawner = heroSpawner;
            _caravanController = caravan;
            // SpawnHero();
        }

        private void SpawnHero() {
            do {
                _newHeroGridPosition = new Vector2Int(Random.Range(0, _width), Random.Range(0, _height));
            } while (_caravanController.GetCaravanGridPositionList().IndexOf(_newHeroGridPosition) != -1);
            
            _heroGameObject = new GameObject("Hero", typeof(SpriteRenderer));
            _heroGameObject.GetComponent<SpriteRenderer>().sprite = _heroSpawner.GetRandomHeroSprite();
            _heroGameObject.transform.position = new Vector3(_newHeroGridPosition.x, _newHeroGridPosition.y);
        
        }

        public bool CheckHeroCollision(Vector2Int gridPosition)
        {
            if (gridPosition != _newHeroGridPosition) return false;
            Object.Destroy(_heroGameObject);
            // SpawnHero();
            return true;
        }

        public Direction? ValidateGridPosition(Vector2Int gridPosition) {
            if (gridPosition.x < 0 || gridPosition.x > _width - 1)
                return gridPosition.y > _height / 2 ? Direction.Down : Direction.Up;

            if (gridPosition.y < 0 || gridPosition.y > _height - 1)
                return gridPosition.x > _width / 2 ? Direction.Left : Direction.Right;
            return null;
        }
    }
}