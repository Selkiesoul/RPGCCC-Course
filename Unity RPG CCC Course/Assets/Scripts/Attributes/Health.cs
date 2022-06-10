using UnityEngine;
using RPGCCC.Saving;
using RPGCCC.Stats;
using RPGCCC.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPGCCC.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;
        LazyValue<float> healthPoints;
        bool isDead = false;
        BaseStats baseStats;

        private void Awake() 
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            baseStats = GetComponent<BaseStats>();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void OnEnable() 
        {
            if (baseStats != null)
            {
                baseStats.onLevelUp += RestoreHealthToMax;
            }
        }

        private void OnDisable() 
        {
            if (baseStats != null)
            {
                baseStats.onLevelUp -= RestoreHealthToMax;
            }
        }

        private void Start() 
        {
            healthPoints.ForceInit(); //this is optional, we can force the initialization of the lazy value before it is called
        }

        private void RestoreHealthToMax()
        {
            healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            
            if (healthPoints.value == 0f && !isDead)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void AwardExperience(GameObject instigator)
        {
                Experience experience = instigator.GetComponent<Experience>();
                if (experience == null) return;

                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetPercentHealth()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health)) * 100;
        }

        void Die()
        {
            if (healthPoints.value == 0f && !isDead)
            {
                GetComponent<Animator>().SetTrigger("die");
                isDead = true;
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float) state;

            Die();
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(GetComponent<BaseStats>().GetStat(Stat.Health), healthPoints.value + healthToRestore);
        }
    }
}