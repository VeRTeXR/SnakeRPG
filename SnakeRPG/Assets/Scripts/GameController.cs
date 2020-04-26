using CaravanSystem;
using LevelGridSystem;
using SpawnerSystem;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LevelGrid _levelGrid;
    [SerializeField] private CaravanController caravanController;
    [SerializeField] private HeroSpawner heroSpawner;
    
    private void Start()
    {
       _levelGrid = new LevelGrid(10,10);
       caravanController.Setup(_levelGrid, heroSpawner);
       heroSpawner.Setup(caravanController, _levelGrid);
       _levelGrid.Setup(caravanController);
    }
}
