using UnityEngine;
using UnityEngine.SceneManagement;

public class Castle : MonoBehaviour
{
    [SerializeField] private BoxCollider2D castleCollider;
    
    private int _currentHealth;
    
    private void Start()
    {
        _currentHealth = GameManager.Instance.maxHealth;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            _currentHealth--;
            GameManager.Instance.UpdateHealth(_currentHealth);
            if (_currentHealth <= 0)
            {
                castleCollider.enabled = false;
                SceneManager.LoadScene(0);
            }
        }
        
    }

    private void OnMouseDown()
    {
        Debug.LogError(_currentHealth);
    }
}
