using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zombie"))
        {
            if (GameUIManager.Instance != null) GameUIManager.Instance.AddScore(1);
        
            // Spawn blood splatter effect
            if (EffectsManager.Instance != null) 
                EffectsManager.Instance.SpawnHitEffect(transform.position, Color.red, 12);

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            // Spawn wall spark effect
            if (EffectsManager.Instance != null) 
                EffectsManager.Instance.SpawnHitEffect(transform.position, Color.yellow, 6);

            Destroy(gameObject);
        }
    }
}