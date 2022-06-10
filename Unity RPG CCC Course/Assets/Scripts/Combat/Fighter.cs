using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using RPGCCC.Movement;
using RPGCCC.Core;
using RPGCCC.Saving;
using RPGCCC.Attributes;
using RPGCCC.Stats;
using GameDevTV.Utils;

namespace RPGCCC.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        //[FormerlySerializedAs("rightHandTransform")]
        [SerializeField] Transform rightHandTransform = null;
        //[FormerlySerializedAs("leftHandTransform")]
        [SerializeField] Transform leftHandTransform = null;
        //[FormerlySerializedAs("defaultWeapon")]
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        
        private void Awake() 
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start() 
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (target != null && !GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if (timeBetweenAttacks <= timeSinceLastAttack)
            {
                // this will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            //Debug.Log("Targetting");
        }

        public void Cancel()
        {
            target = null;
            ResetAttackTrigger();
            GetComponent<Mover>().Cancel();
            //Debug.Log("Cancelled attack");
        }

        private void ResetAttackTrigger()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        // v Animation Event
        void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }
        // v Animation Event
        void Shoot()
        {
            Hit();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            Health targetToTest = combatTarget.GetComponent<Health>();
            if (targetToTest != null && !targetToTest.IsDead())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>((string)state);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}
