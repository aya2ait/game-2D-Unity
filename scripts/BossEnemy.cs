using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    void Start()
    {
        InvokeRepeating("ToggleVisibility", 0f, 2f); // Appelle ToggleVisibility toutes les 2 secondes
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.DamagePlayer(1); // Réduire la santé du joueur
        }
    }

    // Méthode pour alterner la visibilité de l'ennemi
    void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf); // Alterne l'état actif de l'ennemi
    }
}
