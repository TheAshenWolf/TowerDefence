using Abstract;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileMortar : AProjectile
    {
        [SerializeField] private GameObject _explosionPrefab;
        private Vector3 _targetLocation = Vector3.zero;
        private float _flightLength = 2.2f;
        private float _projectileTimer;
        public Vector2 towerPosition;
        protected override void OnTriggerEnter2D(Collider2D col)
        {
            // This projectile doesn't care about enemies
        }

        public override void Init(int dmg, Transform target)
        {
            base.Init(dmg, target);

            _projectileTimer = _flightLength;
        }

        protected override void Update()
        {
            base.Update();

            if (_targetLocation == Vector3.zero)
            {
                if (targetEnemy == null)
                {
                    Destroy(gameObject);
                    return;
                }
                _targetLocation = targetEnemy.position;
            }
            
            _projectileTimer -= Time.deltaTime;
            transform.position = Vector2.Lerp(towerPosition, _targetLocation, _flightLength - _projectileTimer);
            if (_projectileTimer <= 0)
            {
                _projectileTimer = _flightLength;
            }
            
            // transform.Translate((_targetLocation - transform.position).normalized * (projectileSpeed * Time.deltaTime));

            if (Vector2.Distance(transform.position, _targetLocation) < 0.1f)
            {
                GameObject.Instantiate(_explosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosion>().Init(damage);
                Destroy(gameObject);
            }
        }
    }
}