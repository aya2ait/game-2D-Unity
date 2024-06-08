using UnityEngine;

public class Triangle : MonoBehaviour
{
    // Cette méthode est appelée lorsque l'objet entre en collision avec un autre objet
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie si l'objet en collision est le joueur
        if (collision.CompareTag("Player"))
        {
            // Réduit la valeur de collectedAmount du joueur
            PlayerController.collectedAmount--;

            // Détruit l'objet coin
            Destroy(gameObject);
        }
    }
}
