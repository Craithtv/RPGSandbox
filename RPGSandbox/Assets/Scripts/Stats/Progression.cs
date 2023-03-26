using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
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

            if (levels.Length < level)
            { 
                return 0; //if level is wrong, default it
            }
            
            return levels[level - 1];         
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

            private void BuildLookup()
            {
                if (lookupTable != null) return; //only build lookup if it hasnt already

                lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

                foreach (ProgressionCharacterClass progressionClass in characterClasses)
                {
                    var statLookupTable = new Dictionary<Stat, float[]>();//this finds what stat

                    foreach (ProgressionStat progressionStat in progressionClass.stats)
                    {
                        statLookupTable[progressionStat.stat] = progressionStat.levels; //finds levels
                    }

                    lookupTable[progressionClass.characterClass] = statLookupTable;
                }

            }


       [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;//defines which class in the array the SO is for
            public ProgressionStat[] stats;
            //public float[] health;//lets us define health levels

        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;//tells us which stat
            public float[] levels;
        }


    }
}