using System.Collections.Generic;
using Abstract;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void Init(int damage)
    {
        List<Collider2D> hitEnemies = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.NoFilter();
        Physics2D.OverlapCircle(transform.position, .5f, contactFilter2D, hitEnemies);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<AEnemy>()?.OnHit(damage);
        }
    }

    // called from event
    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}