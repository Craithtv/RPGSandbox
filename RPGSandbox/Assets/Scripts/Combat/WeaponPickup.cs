using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{ 
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponent<Fighter>());
                            

            }
        }

        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            //Destroy(gameObject);
            StartCoroutine(HideForSeconds(respawnTime));
                        Debug.Log("pickem");

        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickUp(false);
            yield return new WaitForSeconds(seconds);
            ShowPickUp(true);
                       

        }


        private void ShowPickUp(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            
            foreach (Transform child in transform)//all child transforms of current transform
            {
                child.gameObject.SetActive(shouldShow);
                            Debug.Log(message: "showup");

            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
           if(Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.GetComponent<Fighter>());//grabs fighter component and picksup
                //callingController.GetComponent<Fighter>().MoveToPosition(gameObject.transform.position);

            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
