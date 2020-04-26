using System.Collections.Generic;
using CaravanSystem;
using LevelGridSystem;
using SpawnerSystem.Data;
using UnityEngine;

namespace SpawnerSystem
{
    public class HeroSpawner: MonoBehaviour
    {
        [SerializeField] private List<Sprite> heroSprites;
        private CaravanController _caravanController;
        private bool _isTimerActive;
        private float _currentTimer;
        private float _maxTimer = 3f;
        private LevelGrid _levelGrid;
        private int _levelWidth;
        private int _levelHeight;
        private Vector2Int _newHeroGridPosition;
        private GameObject _heroGameObject;

        private Dictionary<Vector2Int, HeroEntity> _gridPositionHeroEntityPair = new Dictionary<Vector2Int, HeroEntity>();

        public Sprite GetRandomHeroSprite()
        {
            var heroIndex  = Random.Range(0, heroSprites.Count);
 
            return heroSprites[heroIndex];
        }


        public void Setup(CaravanController caravanController, LevelGrid levelGrid)
        {
            _currentTimer = _maxTimer;
            _levelGrid = levelGrid;
            _caravanController = caravanController;

            var levelDimension = _levelGrid.GetLevelDimension();
            _levelWidth = levelDimension.x;
            _levelHeight = levelDimension.y;
            _isTimerActive = true;
        }

        private void Update()
        {
            if (!_isTimerActive) return;
            _currentTimer -= Time.deltaTime;
            if (_currentTimer <= 0)
            {
                _currentTimer = _maxTimer;
                SpawnHero();
            }
        }
        
        private void SpawnHero() {
            do {
                _newHeroGridPosition = new Vector2Int(Random.Range(0, _levelWidth), Random.Range(0, _levelHeight));
            } while (_caravanController.GetCaravanGridPositionList().IndexOf(_newHeroGridPosition) != -1);
            
            _heroGameObject = new GameObject("Hero", typeof(SpriteRenderer));
            _heroGameObject.transform.position = new Vector3(_newHeroGridPosition.x, _newHeroGridPosition.y);
            var heroEntity = _heroGameObject.AddComponent<HeroEntity>();
            heroEntity.Setup(GetRandomHeroSprite());
            _levelGrid.AppendNewHeroPosition(_newHeroGridPosition);
            
            _gridPositionHeroEntityPair.Add(_newHeroGridPosition, heroEntity);

        }

        public HeroEntity GetHeroEntityFromGridPos(Vector2Int gridPos)
        {
            return _gridPositionHeroEntityPair[gridPos];
        } 
    }
}