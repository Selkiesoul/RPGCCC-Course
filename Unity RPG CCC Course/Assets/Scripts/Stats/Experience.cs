using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCCC.Saving;
using System;

namespace RPGCCC.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0f;

        // public delegate void ExperienceGainedDelegate();
        // public event ExperienceGainedDelegate onExperienceGained;
        // the "Action" is shorthand for the above 2 lines
        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float) state;
        }
    }
}
