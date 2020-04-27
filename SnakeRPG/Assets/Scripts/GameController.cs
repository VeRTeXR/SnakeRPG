using CaravanSystem;
using LevelGridSystem;
using SpawnerSystem;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LevelGrid _levelGrid;
    [SerializeField] private CaravanController caravanController;
    [SerializeField] private HeroSpawner heroSpawner;
    [SerializeField] private EnemySpawner enemySpawner;
    private void Start()
    {
       _levelGrid = new LevelGrid(10,10);
       caravanController.Setup(_levelGrid, heroSpawner, enemySpawner);
       heroSpawner.Setup(caravanController, _levelGrid);
       enemySpawner.Setup(caravanController, _levelGrid);
       _levelGrid.Setup(caravanController);
    }
}
