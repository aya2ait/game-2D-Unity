using UnityEngine;

public class DusmanController : MonoBehaviour
{
    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    public float speed = 5f;
    private float roomWidth = 22f;
    private float roomHeight = 10f;
    private Rigidbody2D rb;
    public float maxHealth;
    private float health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    void Update()
    {
       /* // Générer une direction de mouvement aléatoire
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Déplacer le DusmanController dans la direction spécifiée avec une vitesse constante
        rb.velocity = randomDirection * speed;

        // Limiter la position du DusmanController aux limites de la salle
        ClampPosition();*/
    }

    void ClampPosition()
    {
        // Limiter la position du DusmanController aux limites de la salle
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -roomWidth / 2f, roomWidth / 2f);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -roomHeight / 2f, roomHeight / 2f);
        transform.position = clampedPosition;
    }

    public void Death()
    {
        // Déclencher l'événement lorsqu'un ennemi est tué
        OnEnemyKilled?.Invoke();
        Destroy(gameObject, 0.01f);
    }

    public void TakeDamage(float damage)
    {
        // Réduire les points de vie du DusmanController en fonction des dégâts
        health -= damage;

        // Vérifier si le DusmanController est mort
        if (health <= 0)
        {
            Death();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.DamagePlayer(1); // Réduire la santé du joueur
        }
    }
}
