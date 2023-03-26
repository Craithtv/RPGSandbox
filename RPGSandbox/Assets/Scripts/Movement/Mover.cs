using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour , IAction, ISaveable
    {

        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        Health health;
        NavMeshAgent navMeshAgent;



        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead(); //disables navmesh agent when dead

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            
            MoveTo(destination, speedFraction);
        }


        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;            //gets destination as a new Vector3 that PlayerController.cs calls

            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);       //Clamp01 forces the value to be between 0 and 1
            navMeshAgent.isStopped = false;
            

        }

        public void Stop()
        {
            navMeshAgent.isStopped = true;
        }

       

        private void UpdateAnimator()
        {//gets global velocity from NavMeshAgent
         // converts it to local velocity
         // creates variable based of local velocity
         // passes float to animator to set animation
            Vector3 velocity =navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); //inverse converts global velocity to local
            float speed = localVelocity.z; //only need to know how fast going forward
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

       // struct MoverSaveData
      //  {
        //    SerializableVector3 position;
      //      SerializableVector3 rotation;
      //  }

        public object CaptureState()
        {//must be serializable
            Dictionary<string, object> data = new Dictionary<string, object>();//makes dictionary
            data["position"] = new SerializableVector3(transform.position);//saves position
            data["rotation"] = new SerializableVector3(transform.eulerAngles);//saves rotation
            return data;

        }

        public void RestoreState(object state)//happens after awake but before start
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;//casts obj to state
            navMeshAgent.enabled = false;//stops navmesh from messing with pos on load

            transform.position = ((SerializableVector3)data["position"]).ToVector(); //returns vector3 out of dictionary and sets pos to saved state
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector(); //returns vector3 out of dictionary and sets rotation to saved state

           navMeshAgent.enabled = true;//reenables navmesh after loading saved pos
        }
    }
}
