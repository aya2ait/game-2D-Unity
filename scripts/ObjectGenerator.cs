using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public GameObject objectToGenerate; // L'objet à générer
    private bool hasGenerated = false; // Pour garder une trace de si l'objet a déjà été généré
    public PlayerController player; // Référence au script du joueur

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IncrementPlayerCollectedAmount();
            GenerateObject();
        }
    }

    void IncrementPlayerCollectedAmount()
    {
       
            PlayerController.collectedAmount += 1;
            Debug.Log("Collected Amount: " + PlayerController.collectedAmount);
        
    }

    // Méthode appelée lorsque le joueur atteint la condition spécifique
    public void GenerateObject()
    {
        if (!hasGenerated && objectToGenerate != null)
        {
            // Génère l'objet seulement s'il n'a pas déjà été généré
            GameObject generatedObject = Instantiate(objectToGenerate, transform.position, Quaternion.identity);
            generatedObject.SetActive(true);
            hasGenerated = true; // Marque l'objet comme généré
        }
    }
}