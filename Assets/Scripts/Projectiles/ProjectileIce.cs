using System;
using System.Collections.Generic;
using Abstract;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileIce : AProjectile
    {
        public void Init(float speedMultiplier, float duration)
        {
            List<Collider2D> hitEnemies = new List<Collider2D>();
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            contactFilter2D.NoFilter();
            Physics2D.OverlapCircle(transform.position, 1.5f, contactFilter2D, hitEnemies);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<AEnemy>()?.ApplySpeedDebuff(speedMultiplier, duration);
            }
        }
        

        // called from event
        public void OnAnimationFinished()
        {
            Destroy(gameObject);
        }

        protected override void OnTriggerEnter2D(Collider2D col) { }
    }
}