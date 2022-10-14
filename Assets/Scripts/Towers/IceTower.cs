using Abstract;
using Projectiles;
using UnityEngine;

namespace Towers
{
    public class IceTower : ATower
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float slowDuration;
        
        protected override AProjectile Shoot()
        {
            AProjectile projectile = base.Shoot();
            
            ((ProjectileIce)projectile).Init(speedMultiplier, slowDuration);

            return projectile;
        }
    }
}