using System.Collections.Generic;
using SpawnerSystem.Data;
using UnityEngine;

namespace CaravanSystem
{
    public class HeroController : MonoBehaviour
    {
        private HeroEntity _currentHero;
        private List<HeroEntity> _heroEntities = new List<HeroEntity>();
        private int _selectedHeroIndex; 
   
        private void Awake()
        {
            // var defaultHero = gameObject.AddComponent<HeroEntity>();
            // _heroEntities.Add();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) SwitchHeroLeft();

            if (Input.GetKeyDown(KeyCode.E)) SwitchHeroRight();
        }

        private void SwitchHeroRight()
        {
            _selectedHeroIndex++;
            if (_selectedHeroIndex > _heroEntities.Count) _selectedHeroIndex = 0;
            UpdateCurrentHero();
        }

    
        private void SwitchHeroLeft()
        {
            _selectedHeroIndex--;
            if (_selectedHeroIndex <= 0) _selectedHeroIndex = _heroEntities.Count;
            UpdateCurrentHero();
        }
    
        private void UpdateCurrentHero()
        {
            _currentHero = _heroEntities[_selectedHeroIndex];
        }

    }
}
