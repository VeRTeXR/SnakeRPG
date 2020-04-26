using CaravanSystem;
using LevelGridSystem;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LevelGrid _levelGrid;
    [SerializeField] private CaravanController caravanController;
    
    private void Start()
    {
       _levelGrid = new LevelGrid(10,10);
       caravanController.Setup(_levelGrid);
       _levelGrid.Setup(caravanController);
    }
}
