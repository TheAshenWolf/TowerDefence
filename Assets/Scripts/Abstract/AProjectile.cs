using System;
using UnityEngine;

namespace Abstract
{
    public abstract class AProjectile : MonoBehaviour
    {
        [SerializeField] private float timeout;
        [SerializeField] protected float projectileSpeed;
        
        protected int damage;
        protected Transform targetEnemy;
        
        public virtual void Init(int dmg, Transform target)
        {
            damage = dmg;
            targetEnemy = target;
        }

        protected abstract void OnTriggerEnter2D(Collider2D col);

        protected virtual void Update()
        {
            timeout -= Time.deltaTime;
            if (timeout <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}