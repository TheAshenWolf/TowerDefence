using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Abstract
{
    public abstract class AEnemy : MonoBehaviour
    {
        [Title("Settings")] [SerializeField] protected int maxHealth;
        [SerializeField] protected float speed;
        [SerializeField] protected int goldReward;

        [Title("References")] [SerializeField] private SpriteRenderer image;

        [HideInInspector] public float speedMultiplier = 1;
        [HideInInspector] public float speedMultiplierDuration;

        [ReadOnly] private int _nextNodeIndex;
        private Vector3 _nextNodePosition;
        private int _currentHealth;

        private void Move()
        {
            transform.Translate((_nextNodePosition - transform.position).normalized *
                                (speed * speedMultiplier * Time.deltaTime), Space.World);

            speedMultiplierDuration -= Time.deltaTime;
            if (speedMultiplierDuration <= 0) speedMultiplier = 1;

            if (Vector2.Distance(transform.position, _nextNodePosition) > .05f) return;

            _nextNodeIndex++;
            _nextNodePosition = GameManager.Instance.tilemap.CellToWorld(GameManager.pathNodes[_nextNodeIndex]);
            transform.rotation = Quaternion.identity;
            Transform trf = transform;
            transform.RotateAround(trf.position, Vector3.forward, Vector2.SignedAngle(trf.right, _nextNodePosition - trf.position));

        }

        public void Spawn()
        {
            _currentHealth = maxHealth * (1 + GameManager.Instance.currentWave / 10);
            _nextNodeIndex = 0;
            _nextNodePosition = GameManager.Instance.tilemap.CellToWorld(GameManager.pathNodes[_nextNodeIndex]);
            transform.rotation = Quaternion.identity;
            Transform trf = transform;
            transform.RotateAround(trf.position, Vector3.forward, Vector2.SignedAngle(trf.right, _nextNodePosition - trf.position));
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Castle"))
            {
                // Castle is responsible for handling damage onTrigger; we just destroy the enemy here
                Destroy(gameObject);
            }
        }

        public void OnHit(int damage)
        {
            _currentHealth -= damage;

            StartCoroutine(HitAnim());

            if (_currentHealth <= 0)
            {
                GameManager.Instance.AddMoney(goldReward, transform.position);
                GameManager.Instance.OnEnemyKilled();
                Destroy(gameObject);
            }
        }

        private IEnumerator HitAnim()
        {
            image.color = Color.red;
            yield return new WaitForSeconds(.1f);
            image.color = Color.white;
        }

        private void Update()
        {
            Move();
        }

        public void ApplySpeedDebuff(float multiplier, float duration)
        {
            speedMultiplier = multiplier;
            speedMultiplierDuration = duration;
        }
    }
}