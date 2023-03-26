using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;

        [SerializeField] Weapon defaultWeapon = null;//takes weapon scriptobj
      


        Health target;
        float timeSinceLastAttack = Mathf.Infinity;//infinity so you can always attack on load
        LazyValue<Weapon> currentWeapon;


        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttatchWeapon(defaultWeapon);
           
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
            
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttatchWeapon(weapon);
        }

        private void AttatchWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>(); ;//grabs animator
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {//getter for enemy health display
            return target; 
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;
            //if there is no target or it is dead, skip the rest of the update function

            

            if (!GetIsInRange())
            {
                MoveToPosition(target.transform.position);//if target is not in range, move
            }
            else
            {
                GetComponent<Mover>().Stop();//stop if it is
                AttackBehavior();//begin attack loop                                
            }
        }

        public void MoveToPosition(Vector3 movePosition)
        {
            GetComponent<Mover>().MoveTo(movePosition, 1f);
        }

        private void AttackBehavior()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                transform.LookAt(target.transform);//faces enemy

                //This will trigger the Hit() event.
                TriggerAttack();
                timeSinceLastAttack = 0;

            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");//resets trigger to make sure it plays

            GetComponent<Animator>().SetTrigger("attack");//play attack animation
        }

        void Hit()
        {
            //This is an animation event called in the editor
            if (target == null)
            {
                return;//prevents throwing an exception if no target
            }


            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);//uses damage in both instance         
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                
                target.TakeDamage(gameObject, damage);
                
            }
            
        }

        void Shoot()
        {//projectile animation event
            Hit();
        }


        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();//accesses targets health
            print("bam");

        }

        public bool CanAttack(GameObject combatTarget)
        {//getting grabbed by PlayerController to prevent dead bodies from blocking raycasts
            if (combatTarget == null)
            {
                return false; //if the selected combat target is null, CanAttack is false
            }
            print("can attack");
            Health targetToTest = combatTarget.GetComponent<Health>();//teasting to see if target has any health
            return targetToTest != null && !targetToTest.IsDead(); //if targtet is not null and not dead, can attack 
        }

        public void Stop()
        {
            StopAttack();

            target = null;
            GetComponent<Mover>().Stop();

        }
        public IEnumerable <float> GetAdditiveModifers(Stat stat)
        {
            if(stat == Stat.Damage)
            {//if stat requested is damage, return weapon damage
                yield return currentWeapon.value.GetDamage();
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {//if stat requested is damage, return weapon damage
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");//cancel attack animation
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state; //puts saved obj into string
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName); //loads weapon based on string 
            EquipWeapon(weapon);//equips it, duh
        }

       
    } 
}