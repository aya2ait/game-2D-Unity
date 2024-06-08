using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healthAmount = 1f; // Montant de santé à ajouter lorsque le joueur touche cet objet

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.IncrementHealth(healthAmount); // Augmente la santé du joueur en utilisant la méthode du GameController
            Destroy(gameObject); // Détruit cet objet après qu'il a été ramassé par le joueur
        }
    }
}
