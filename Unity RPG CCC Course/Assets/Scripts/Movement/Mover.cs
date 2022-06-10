using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPGCCC.Core;
using RPGCCC.Saving;
using RPGCCC.Attributes;

namespace RPGCCC.Movement
{
        public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        NavMeshAgent navMeshAgent;
        Health health;
        //Ray latestRay; (used in the Debug.DrawRay)
        void Awake() 
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            //Debug.DrawRay(latestRay.origin, latestRay.direction * 100); 
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination);
        }
        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
        void UpdateAnimator()
        {
            Vector3 navMeshVelocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(navMeshVelocity);
            //we're getting the z velocity so the animator can properly blend the animations, since they are based on the z velocity
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
