using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{

    public class Projectile : MonoBehaviour
    {
        Health target = null;
        float damage = 0;
        GameObject instigator = null;


        [SerializeField] bool isHoming = true;
        [SerializeField] float speed = 1;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2;

        private void Start()
        {
            transform.LookAt(GetAimLocation());

        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;//protects against null

            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());

            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)//needs target and damage parameters
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2; //makes projectile aim for center of capsule collider target
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target)return;//making sure collision has health
            if (target.IsDead()) return; //if target is dead, keep going. stops arrows from hitting dead body colliders
            target.TakeDamage(instigator, damage);
            speed = 0;//so projectile doesnt keep traveling

            if (hitEffect != null)
            {//plays hit effect
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {//destroys GO first and trail after
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
