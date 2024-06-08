using UnityEngine;

public class SizeChangeController : MonoBehaviour
{
    public float changeInterval = 1f;
    private float timeSinceLastChange = 0f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.DamagePlayer(1); // Réduire la santé du joueur
        }
    }

    void Update()
    {
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= changeInterval)
        {
            ChangeSize();
            timeSinceLastChange = 0f;
        }
    }

    void ChangeSize()
    {
        float scaleFactor = Random.Range(0.5f, 1.5f);
        transform.localScale = originalScale * scaleFactor;
    }
}
