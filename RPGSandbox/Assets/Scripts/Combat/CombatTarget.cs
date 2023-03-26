using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]//automatically places health script if none present

    public class CombatTarget : MonoBehaviour, IRaycastable
    {

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
        public bool HandleRaycast(PlayerController callingController)
        {

            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject)) //if CanAttack is false
            {
                return false; //go to next hit in loop to ignore dead bodies
            }

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            
            return true;//goes true even if hovering over enemy without clicking
        }
    }


}