using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPGCCC.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject 
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];

            if(levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
            //we replace the below code with nested dictionaries as this is much less expensive than running large foreach loops every frame
            // foreach(ProgressionCharacterClass progressionClass in characterClasses)
            // {
            //     if (progressionClass.characterClass != characterClass) continue;

            //     foreach (ProgressionStat progressionStat in progressionClass.stats)
            //     {
            //         if (progressionStat.stat != stat) continue;

            //         if (progressionStat.levels.Length < level) continue;
            //         // ^ guarding against looking for a level in a stat array that isn't long enough

            //         return progressionStat.levels[level - 1];
            //     }
            // }
            // return 30;
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach(ProgressionCharacterClass progressionClass in characterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}