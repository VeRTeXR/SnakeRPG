using System.Collections.Generic;
using CaravanSystem;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using LevelGridSystem;
using SpawnerSystem.Data;
using UnityEngine;

namespace SpawnerSystem
{
    public class HeroSpawner: MonoBehaviour, ISubscriber
    {
        
        [SerializeField] private HeroEntity heroPrefab;
        [SerializeField] private List<Sprite> heroSprites;
        private CaravanController _caravanController;
        private bool _isTimerActive;
        private float _currentTimer;
        private float _maxTimer = 8f;
        private LevelGrid _levelGrid;
        private int _levelWidth;
        private int _levelHeight;
        private Vector2Int _newHeroGridPosition;
        private GameObject _heroGameObject;

        private List<Vector2Int> _heroOccupiedGridPosition = new List<Vector2Int>();
        private Dictionary<Vector2Int, HeroEntity> _gridPositionHeroEntityPair = new Dictionary<Vector2Int, HeroEntity>();

        
        
        private void Awake()
        {
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
        }

        private bool OnGameOver(GameOver signal)
        {
            _isTimerActive = false;
            return true;
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

        public void SpawnHero()
        {
            var heroOnGrid = _levelGrid.GetHeroOnGridPositionList();
            var enemyOnGrid = _levelGrid.GetEnemyOnGridPositionList();
            do
            {
                _newHeroGridPosition = new Vector2Int(Random.Range(0, _levelWidth), Random.Range(0, _levelHeight));
            } while (_caravanController.GetCaravanGridPositionList().IndexOf(_newHeroGridPosition) != -1 &&
                     heroOnGrid.IndexOf(_newHeroGridPosition) != -1 && enemyOnGrid.IndexOf(_newHeroGridPosition) != -1);
            
            _heroGameObject = new GameObject("Hero"+GetInstanceID(), typeof(SpriteRenderer));
            _heroGameObject.transform.position = new Vector3(_newHeroGridPosition.x, _newHeroGridPosition.y);
            var heroEntity = _heroGameObject.AddComponent<HeroEntity>();
            heroEntity.Setup(GetRandomHeroSprite());
            
            _levelGrid.AppendNewHeroPosition(_newHeroGridPosition);
            _heroOccupiedGridPosition.Add(_newHeroGridPosition);
            _gridPositionHeroEntityPair.Add(_newHeroGridPosition, heroEntity);
        }

        public Sprite GetRandomHeroSprite()
        {
            var heroIndex = Random.Range(0, heroSprites.Count);
            return heroSprites[heroIndex];
        }

        public HeroEntity GetHeroEntityFromGridPos(Vector2Int gridPos)
        {
            var heroInGrid = _gridPositionHeroEntityPair[gridPos];
            _gridPositionHeroEntityPair.Remove(gridPos);
            _heroOccupiedGridPosition.Remove(gridPos);
            return heroInGrid;
        }
    }
}