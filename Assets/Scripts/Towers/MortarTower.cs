using Abstract;
using Projectiles;

namespace Towers
{
    public class MortarTower : ATower
    {
        protected override AProjectile Shoot()
        {
            AProjectile projectile = base.Shoot();
            ((ProjectileMortar)projectile).towerPosition = projectileSpawnPoint.position;

            return projectile;
        }
    }
}