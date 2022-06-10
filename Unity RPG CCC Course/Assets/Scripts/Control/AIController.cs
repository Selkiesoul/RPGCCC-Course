using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCCC.Combat;
using RPGCCC.Movement;
using RPGCCC.Core;
using RPGCCC.Attributes;
using GameDevTV.Utils;

namespace RPGCCC.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        GameObject player;
        Fighter fighter;
        Mover mover;
        Health health;
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSpentAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] float waypointDwellTime = 2f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        int currentWaypointIndex = 0;
        [SerializeField] float patrolSpeed = 2.5f;
        [SerializeField] float chaseSpeed = 5f;
        [SerializeField] float shoutDistance = 5f;

        private void Awake() 
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }
        
        void Update()
        {
            if (health.IsDead()) return;
            enemyStates();
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSpentAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0f;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void enemyStates()
        {
            if (IsAggrevated())
            {
                //chase state
                if (!fighter.CanAttack(player)) return;
                ChaseBehavior();
            }
            else if(timeSinceLastSawPlayer <= suspicionTime)
            {
                //suspicion state
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else
            {
                //guard / patrol state
                PatrolBehavior();
            }
        }

        private void PatrolBehavior()
        {
            mover.SetSpeed(patrolSpeed);
            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if (!AtWaypoint())
                {
                    timeSpentAtWaypoint = 0f;
                }
                if (timeSpentAtWaypoint >= waypointDwellTime)
                {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            
            mover.StartMoveAction(nextPosition);
        }

        private void ChaseBehavior()
        {
            fighter.Attack(player);
            AggrevateNearbyEnemies();
            timeSinceLastSawPlayer = 0f;
            mover.SetSpeed(chaseSpeed);
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if(ai == null) continue;

                ai.Aggrevate();
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private bool IsAggrevated()
        {
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }
    
        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
