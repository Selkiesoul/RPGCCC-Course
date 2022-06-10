using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using RPGCCC.Control;
using RPGCCC.Movement;
using RPGCCC.Attributes;

namespace RPGCCC.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        //[FormerlySerializedAs("weapon")]
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] float respawnTime = 5f;
        
        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                GameObject.FindWithTag("Player").GetComponent<Mover>().StartMoveAction(transform.position);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
