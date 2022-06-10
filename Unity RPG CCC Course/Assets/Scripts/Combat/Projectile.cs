using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPGCCC.Attributes;
using UnityEngine.Events;

namespace RPGCCC.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 1f;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0f;
        [SerializeField] bool isHoming = false;
        bool hasSetTarget = false;

        void Update()
        {
            if (target == null) return;
            AcquireTarget();
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        private void AcquireTarget()
        {
            if (!isHoming && hasSetTarget) return;
            if (target.IsDead()) return;
            transform.LookAt(GetAimLocation());
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            hasSetTarget = true;
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 1.5f;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject != target.gameObject) return;
            if (target.IsDead()) return;

            projectileSpeed = 0f;

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            target.TakeDamage(instigator, damage);


            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
