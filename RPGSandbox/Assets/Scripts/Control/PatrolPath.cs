using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{ 
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.3f;
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)//goes from 0 to how many children in the object -1 (patrol path) 
            {
                int j = GetNextIndex(i);//used to find next waypoint for line 
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetNextIndex(int i)
        {//passes in current index and gets the next one
           if(i + 1 == transform.childCount)
            {//if at top of index, go to 0
                return 0;
            }
            return i + 1;//if not last waypoint, go to the next one
        }

        public Vector3 GetWaypoint(int i)
        {//gets the child waypoints transform
            return transform.GetChild(i).position;
        }
    }
}