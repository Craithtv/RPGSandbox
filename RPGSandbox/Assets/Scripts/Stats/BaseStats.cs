using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]//puts a slider for starting level
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);

        }
        private void Start()
        {
            currentLevel.ForceInit();
            
        }

        private void OnEnable()
        {//do not use external functions here
            if (experience != null)
            {//registers callback so is safe here
                experience.onExperienceGained += UpdateLevel;//adds updatelevel to list of methods in onExperienceGained
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {//registers callback so is safe here
                experience.onExperienceGained -= UpdateLevel;//disables callback if BaseStats somehow gets disabled
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
               
                currentLevel.value = newLevel;
                // print("level up baybay");
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat)/100);
        }

     

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }


        public int GetLevel()
        {
            return currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {

            if (!shouldUseModifiers) return 0; //prevents evenmies from getting modifiers
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifers(stat))
                {//grabs each provider stat and adds them to total
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0; //prevents evenmies from getting modifiers
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifiers in provider.GetPercentageModifiers(stat))
                {//grabs each provider stat and adds them to total
                    total += modifiers;
                }
            }
            return total;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null) return startingLevel; //this is for enemies

            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);//finds second to last level
            for (int level = 1; level < penultimateLevel; level++)//goes over every level
            {
               float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);//finds xp to level up for a level
                if (XPToLevelUp > currentXP)//compares xp
                {
                    return level;//finds what level we should be at
                }
            }

            return penultimateLevel + 1;// caps mex level
        }

        
    }
}
