using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateHealth(currentHealth);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            TakeDamage(1);
        }
    }

    void TakeDamage(int damage)
    {   
        if (CameraFollow.Instance != null) CameraFollow.Instance.TriggerShake(0.2f, 0.25f);
        currentHealth -= damage;
        
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowGameOver();
            }
            Destroy(gameObject);
        }
    }
}