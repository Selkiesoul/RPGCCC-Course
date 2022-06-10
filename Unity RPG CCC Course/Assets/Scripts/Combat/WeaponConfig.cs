using UnityEngine;
using UnityEngine.Serialization;
using RPGCCC.Attributes;
using System;

namespace RPGCCC.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject 
    {
        //[FormerlySerializedAs("animatorOverride")]
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        //[FormerlySerializedAs("handEquipmentPrefab")]
        [SerializeField] Weapon handEquipmentPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon SpawnWeapon(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if(handEquipmentPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(handEquipmentPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            // v lesson 125, this fixes a bug where the animation for the fireball doesn't switch back to the default animation after equipping a weapon with a different animation,
            // such as a sword
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            // ^
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            // v
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            // ^
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            // ^ this is a bugfix, sometimes the order of execution was inconsistent and could result in the new weapon having the same name as the one we were trying to destroy(according to lesson 119)
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }

            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            if (!target.IsDead())
            {
                Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
                projectileInstance.SetTarget(target, instigator, calculatedDamage);
            }
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
    }
}