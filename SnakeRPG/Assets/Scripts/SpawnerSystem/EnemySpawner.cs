using System;
using System.Collections.Generic;
using CaravanSystem;
using CaravanSystem.Signal;
using echo17.Signaler.Core;
using LevelGridSystem;
using SpawnerSystem.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnerSystem
{
    public class EnemySpawner : MonoBehaviour, ISubscriber
    {
        [SerializeField] private List<Sprite> enemySprite;

        private CaravanController _caravanController;
        private LevelGrid _levelGrid;

        private bool _isTimerActive;
        private float _currentTimer;
        private float _maxTimer = 3f;
        private Vector2Int _newGridPosition;
        private int _levelWidth;
        private int _levelHeight;
        private Dictionary<Vector2Int, EnemyEntity> _gridPositionEntityPair = new Dictionary<Vector2Int, EnemyEntity>();
        private List<Vector2Int> _enemyOccupiedGridPosition = new List<Vector2Int>();

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
            _caravanController = caravanController;
            var levelDimension = levelGrid.GetLevelDimension();
            _levelGrid = levelGrid;
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
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            var heroOnGrid = _levelGrid.GetHeroOnGridPositionList();
            var enemyOnGrid = _levelGrid.GetEnemyOnGridPositionList();
            do
            {
                _newGridPosition = new Vector2Int(Random.Range(0, _levelWidth), Random.Range(0, _levelHeight));
            } while (_caravanController.GetCaravanGridPositionList().IndexOf(_newGridPosition) != -1 
                     && heroOnGrid.IndexOf(_newGridPosition) != -1
                     && enemyOnGrid.IndexOf(_newGridPosition) != -1);

            var enemyObject = new GameObject("Enemy", typeof(SpriteRenderer));
            enemyObject.transform.position = new Vector3(_newGridPosition.x, _newGridPosition.y);
            var enemyEntity = enemyObject.AddComponent<EnemyEntity>();
            enemyEntity.Setup(GetRandomSprite());
            _levelGrid.AppendEnemyPosition(_newGridPosition);
            _enemyOccupiedGridPosition.Add(_newGridPosition);
            _gridPositionEntityPair.Add(_newGridPosition, enemyEntity);
        }

        private Sprite GetRandomSprite()
        {
            var heroIndex  = Random.Range(0, enemySprite.Count);
            return enemySprite[heroIndex];
        }

        public EnemyEntity GetEnemyEntityFromGridPos(Vector2Int currentPositionOnGrid)
        {
            return _gridPositionEntityPair[currentPositionOnGrid];
        }
        
        public void RemoveEnemyEntityFromGridPos(Vector2Int currentPositionOnGrid)
        {
            _enemyOccupiedGridPosition.Remove(currentPositionOnGrid);
            _gridPositionEntityPair.Remove(currentPositionOnGrid);
        }

    }
}