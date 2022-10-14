using Abstract;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileFireball : AProjectile
    {
        protected override void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                col.GetComponent<AEnemy>().OnHit(damage);
                Destroy(gameObject);
            }
        }

        protected override void Update()
        {
            if (targetEnemy == null)
            {
                Destroy(gameObject);
                return;
            }
            
            // rotate sprite to face target
            Vector3 dir = targetEnemy.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            base.Update();
            if (targetEnemy == null || transform == null) return;
            transform.Translate((targetEnemy.position - transform.position).normalized * (projectileSpeed * Time.deltaTime), Space.World);
        }
    }
}