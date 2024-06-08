using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnFinalScene : MonoBehaviour
{
    public string finalSceneName = "Main Menu"; // Nom de la scène finale
    public string debut = "Main End";

    private void OnEnable()
    {
        // S'abonner à l'événement de chargement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Se désabonner de l'événement de chargement de scène
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == finalSceneName || scene.name == debut)
        {
            Destroy(gameObject); // Détruire l'objet si c'est la scène finale
        }
    }
}
