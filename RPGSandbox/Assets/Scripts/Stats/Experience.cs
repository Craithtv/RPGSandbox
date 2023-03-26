using RPG.Saving;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public delegate void ExperienceGainedDelegate();
        public event Action onExperienceGained;//used in place of delegate

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();//calls everything in delegate list

        }

        public float GetPoints()
        {//getter method for display
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }
        public void RestoreState(object state)
        {
            experiencePoints = (float)state;//casts the state to a float and putting into XP

        }
    }
}
