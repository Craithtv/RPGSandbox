using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;//guard aggro range
        [SerializeField] float suspicionTime = 3f;//how long guard will wait before returning to post
        [SerializeField] float waypointDwellingTime = 2f; //how long wait per waypoint
        [SerializeField] PatrolPath patrolPath;//must be linked in scene
        [SerializeField] float waypointTolerance = 1f;//how far from waypoint mover will accept
        [Range(0,1)]//limits patrolSpeedFrac to be between 0 and 1
        [SerializeField] float patrolSpeedFraction = 0.2f;//to divide by movers maxSpeed

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceReachedWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>(); //gets ref to fighter component
            player = GameObject.FindWithTag("Player"); //finds the player
            health = GetComponent<Health>();//grabs health of object
            mover = GetComponent<Mover>();//grabs mover component

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }
        private void Start()
        {
            
            guardPosition.ForceInit();
        }

        private void Update()
        {//states by priorty. Dead>Attack>Sus>Patrol
            if (health.IsDead()) return; //if the object is dead, skip eveything else

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))//if player in chase distance AND enemy can attack player, 
            {//attack state

                
                                            //print(gameObject.name + "chase em"); debug to see who is chasing the player
                AttackBehaviour();

            }

            else if (timeSinceLastSawPlayer < suspicionTime)
            {//sus state
                SuspicionBehavior();
            }

            else
            {//guard/default state           
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;//timer so outside of state functions
            timeSinceReachedWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value; //guardPos is the defaultPos

            if (patrolPath != null)//if this has a patrolpath component attatched in scene
            {
                if (AtWaypoint())
                {
                    timeSinceReachedWaypoint = 0;

                    CycleWaypoint();//goes to next waypoint
                }
                nextPosition = GetCurrentWaypoint();//tells enemy where to move
            }

            if (timeSinceReachedWaypoint > waypointDwellingTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);//moves to next position
            }
            
        }

        private void CycleWaypoint()
        {//updates currentIndex with PatrolPaths knowledge of what the next waypoint should be
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);//grabs waypoint from PatrolPath
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance; //is true if close enough to waypoint
        }

        private void SuspicionBehavior()
        {//stops where it loses range of player and waits before returning to guardPos
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0; //if we attack we must see em
            print("attacking pc");
            fighter.Attack(player);//attacks them
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);//gets the distance between enemy and player
            
            return distanceToPlayer < chaseDistance; //returns true if player is in range
        }

        private void OnDrawGizmosSelected()
        {//called by unity to visualize chase distance
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}