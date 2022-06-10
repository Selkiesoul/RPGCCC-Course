using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCCC.Attributes;
using RPGCCC.Control;

namespace RPGCCC.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
                //RaycastHit.transform identifies which object we're focusing on. hit.collider and hit.rigidbody would also work, but this works for objects with either colliders or rigidbodies
                //GetComponent will look through all the other component on this object once we've identified which object to look through
                //the only purpose of getting this component is to check if our object is targettable. alternative to using a tag
                //CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                // continue is similar to return, skips the rest of this iteration of the loop
                //if (target == null) continue;
                if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
                {
                    return false;
                }

                if (Input.GetMouseButton(0))
                {
                    callingController.GetComponent<Fighter>().Attack(gameObject);
                }
                return true;
        }
    }
}