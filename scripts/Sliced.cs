using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliced : MonoBehaviour
{
    // Déclarer le composant Rigidbody2D
    private Rigidbody2D rb;
    private float roomWidth = 22f;
    private float roomHeight = 10f;

    void Start()
    {
        // Obtenir le composant Rigidbody2D attaché à l'objet
        rb = GetComponent<Rigidbody2D>();
        
        // Limiter la position initiale de l'objet aux limites de la salle
        ClampPosition();
    }

    void Update()
    {
        // Pas besoin de limiter la position ici, car elle est déjà limitée à la position initiale
    }

    void ClampPosition()
    {
        // Limiter la position du DusmanController aux limites de la salle
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -roomWidth / 2f, roomWidth / 2f);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -roomHeight / 2f, roomHeight / 2f);
        transform.position = clampedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  
        {
            PlayerController.collectedAmount++;
            Destroy(gameObject);
        }
    }
}
