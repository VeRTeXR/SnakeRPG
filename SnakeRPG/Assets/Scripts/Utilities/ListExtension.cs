using System.Collections.Generic;
using SpawnerSystem.Data;

namespace Utilities
{
    public static class ListExtension 
    {
        public static List<HeroEntity> Swap(this List<HeroEntity> list, 
            int firstIndex, 
            int secondIndex) 

        {
            HeroEntity temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;

            return list;
        }
    }   
}