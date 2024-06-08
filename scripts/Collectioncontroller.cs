using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionController : MonoBehaviour
{
    private Vector2 roomSize = new Vector2(17f, 8f); // Taille de la pièce

    void Start()
    {
        // Générer une position aléatoire à l'intérieur de la pièce
        float randomX = Random.Range(-roomSize.x / 2f, roomSize.x / 2f);
        float randomY = Random.Range(-roomSize.y / 2f, roomSize.y / 2f);

        // Appliquer la position aléatoire à l'objet
        transform.position = new Vector2(randomX, randomY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController.collectedAmount++;
            Destroy(gameObject);  // Détruire l'objet lorsqu'il est collecté par le joueur
        }
    }
}
