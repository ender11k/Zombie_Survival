using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SpawnHitEffect(Vector3 position, Color color, int particleCount = 8)
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject("Particle");
            p.transform.position = position;
            p.transform.localScale = Vector3.one * Random.Range(0.08f, 0.18f);

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSquareSprite();
            sr.color = color;
            sr.sortingOrder = 10;

            Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            
            // Scatter particle in random direction
            Vector2 randomVelocity = Random.insideUnitCircle.normalized * Random.Range(3f, 8f);
            rb.linearVelocity = randomVelocity;

            Destroy(p, Random.Range(0.08f, 0.18f)); // Quick burst duration
        }
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(2, 2);
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
    }
}