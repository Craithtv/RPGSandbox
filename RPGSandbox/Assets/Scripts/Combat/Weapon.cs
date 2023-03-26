using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {//only things that change in ScriptableObject
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] Projectile projectile = null;
        [SerializeField] bool isRightHanded = true;


        const string weaponName = "Weapon"; 

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride; //overrides default animator and sets to weapon
            }
            else
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController != null)
                {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController; //if therer's an override, find parent and reoveride animator
                    //fixes attack animations when picking up a weapon while having one
                }
            }

        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);//checks right hand for weapon
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);//checks left hand
            }
            if (oldWeapon == null) return; //after checking both hands, determine theres no equip weapon and move on

            oldWeapon.name = "DESTROYING"; //avoids rare bug that tries to destroy new equipd wep
            Destroy(oldWeapon.gameObject); //if we find one, destroy it 
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;//do we have a projectile equipped to call?
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);//spawns projectile
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetRange()
        {//makes range public
            return weaponRange;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetDamage()
        {//makes damage public 
            return weaponDamage;
        }
    }




}