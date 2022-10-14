using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Abstract
{
    public abstract class ATower : MonoBehaviour
    {
        [Title("Settings")] [SerializeField] protected float range;
        [SerializeField] protected float attackDelay;
        [SerializeField] protected int attackDamage;
        [SerializeField] protected bool rotates;

        [Title("References")] [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected CircleCollider2D rangeCollider;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform projectileSpawnPoint;

        private float _attackTimer;
        protected Transform target;
        private HashSet<Collider2D> _enemiesInRange = new HashSet<Collider2D>();

        protected virtual void LoadShot()
        {
            if (animator != null) animator.Play("Shoot");
        }
        
        // Called from an event
        protected virtual AProjectile Shoot()
        {
            AProjectile projectile = GameObject.Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity)
                .GetComponent<AProjectile>();
            projectile.Init(attackDamage, target);

            return projectile;
        }

        protected virtual void Start()
        {
            _attackTimer = attackDelay / 2f;
            if (rangeCollider != null) rangeCollider.radius = range;
        }

        private void Update()
        {
            if (target != null && _attackTimer <= 0)
            {
                LoadShot();
                _attackTimer = attackDelay;
            }
            else
            {
                if (target == null)
                {
                    if (_enemiesInRange.Count == 0) return;
                    try
                    {
                        target = _enemiesInRange.First(x => x != null).transform;
                    }
                    catch
                    {
                        _enemiesInRange.Clear();
                    }
                }
                else if (rotates)
                {
                    // rotate sprite to face target
                    Vector3 dir = target.position - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }

                _attackTimer -= Time.deltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (range == 0) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemiesInRange.Add(other);
                if (target == null)
                {
                    target = other.transform;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.transform == target)
                {
                    target = null;
                }
                _enemiesInRange.Remove(other);
            }
        }
    }
}