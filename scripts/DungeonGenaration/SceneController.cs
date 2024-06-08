using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int totalEnemies = 9; // Le nombre total d'ennemis à tuer
    public float timeLimit = 10f; // Le temps limite en secondes
    private int enemiesKilled = 0; // Compteur pour les ennemis tués
    private float timer = 0f; // Compteur de temps
    private bool gameStarted = false; // Booléen pour vérifier si le jeu a commencé
    public PlayerController player;

    void Start()
    {
        DusmanController.OnEnemyKilled += EnemyKilled; // S'abonner à l'événement OnEnemyKilled
        timer = timeLimit; // Initialiser le minuteur
        gameStarted = true; // Démarrer le jeu
    }

    void Update()
    {
        if (gameStarted)
        {
            timer -= Time.deltaTime; // Décrémenter le minuteur
            if (timer <= 0)
            {
                GameOver(false); // Terminer le jeu si le temps est écoulé
            }
        }
    }

    void EnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled >= totalEnemies)
        {
            GameOver(true); // Terminer le jeu si tous les ennemis sont tués
        }
    }

    void GameOver(bool success)
    {
        gameStarted = false; // Arrêter le jeu
        DusmanController.OnEnemyKilled -= EnemyKilled; // Se désabonner de l'événement

        if (success)
        {
            Debug.Log("Victory! All enemies killed in time.");
            PlayerController.collectedAmount+=10;
            Debug.Log("collected"+PlayerController.collectedAmount);
            
        }
        else
        {
            Debug.Log("Defeat! Time ran out.");
            DestroyAllEnemies(); // Détruire tous les ennemis restants
            
        }
    }

    void DestroyAllEnemies()
    {
        DusmanController[] enemies = FindObjectsOfType<DusmanController>();
        foreach (DusmanController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void OnDestroy()
    {
        DusmanController.OnEnemyKilled -= EnemyKilled; // Se désabonner de l'événement lors de la destruction
    }
}
