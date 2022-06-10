using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCCC.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.3f;
        // Transform childTransform;

        // private void Start() 
        // {
        //     childTransform = transform.GetChild(0).transform;
        // }
        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
                // if (i>0)
                // {
                // Gizmos.DrawLine(childTransform.position,transform.GetChild(i).transform.position);
                // childTransform = transform.GetChild(i).transform;
                // }

            }
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 < transform.childCount)
            {
                return i + 1;
            }
            else
            {
                return 0;
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
