using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Saving;


namespace RPG.SceneManagement
{

    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;



        private void Start()
        {
            
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //print("PORTAL BABY");
                StartCoroutine(Transition());
            }

            

        }

        private IEnumerator Transition()
        {//ques up the next level

            if(sceneToLoad <0)
            {//if we haven't loaded a new scene, skip
                Debug.LogError("Scene not set dummy");
                yield break;
            }

            DontDestroyOnLoad(gameObject); //dont destroy portal on load so coroutine can print


            Fader fader = FindObjectOfType<Fader>();//finds our fader


            yield return fader.FadeOut(fadeOutTime);//start fading out

            //save current level
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();//grabs ref to wrapper
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);//wait for scene to load

            // load current level
            wrapper.Load();


            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);//move player to spawn point
            wrapper.Save();//saves player pos after portaling but before scene loads. prevents player from saving while portaling


            yield return new WaitForSeconds(fadeWaitTime); //tells scene to wait a time for things to load/stabilize
            yield return fader.FadeIn(fadeInTime);//fades back in

            print("Scene Loads");//can transfer information to new scene
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {

            GameObject player = GameObject.FindWithTag("Player");//finds player
            player.GetComponent<NavMeshAgent>().enabled = false;


            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);//tells NavMesh where to spawn to avoid conflicts      
            player.transform.rotation = otherPortal.spawnPoint.rotation;//then rotation which doesnt conflict with NavMesh

            player.GetComponent<NavMeshAgent>().enabled = true;

        }

        private Portal GetOtherPortal()
        {
           foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;//prevents it from grabbing portal we're at
                if (portal.destination != destination) continue; //if the portal doesn't have the right destination, skip

                return portal;//returns other portal

            }
            return null; //if no portal found
        }
    }
}
