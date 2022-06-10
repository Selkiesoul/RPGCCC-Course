using System;
using System.Collections;
using System.Collections.Generic;
using RPGCCC.Movement;
using RPGCCC.Combat;
using RPGCCC.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPGCCC.Control
{
    public class PlayerController : MonoBehaviour
    {
        bool hasHit;
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxNavPathLength = 40f;
        [SerializeField] float raycastRadius = 0.2f;

        void Awake()
        {
            health = GetComponent<Health>();
        }
        void Update()
        {
            if(InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;    
            }

            if(InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
           RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
           float[] distances = new float[hits.Length];
           for (int i = 0; i < hits.Length; i++)
           {
               distances[i] = hits[i].distance;
           }

           Array.Sort(distances, hits);
           return hits;
        }

        private bool InteractWithUI()
        {
           if (EventSystem.current.IsPointerOverGameObject())// a "game object" in this case means a UI element, since Unity's event system deals with UI
           // for this to work we need to uncheck interactable and blocks raycasts in the fader canvas group, since that's also a UI element
           {
               SetCursor(CursorType.UI);
               return true;
           } 
            return false;
        }

        private bool InteractWithMovement()
        {
            //physics.raycast stores information about where the ray collides with something in the second variable (hit)
            // the raycast method is also a bool, so we can tell if the ray hits something or not (which is why we set the value to a bool variable)
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if(Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit,maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if(GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0]; //protection against not having a mapping for a particular situation
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
