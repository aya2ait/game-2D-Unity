using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Transform respawnPoint;

    private static float health = 6;
    private static int maxHealth = 6;
    private static float moveSpeed = 5f;
    private static float fireRate = 0.5f;

    public static float Health { get => health; set => health = value; }
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public static float FireRate { get => fireRate; set => fireRate = value; }

    public TMP_Text healthText;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = respawnPoint.position;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static void ResetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    void Update()
    {
        healthText.text = "Health: " + health;
    }

    public static void DamagePlayer(int damage)
    {
        health -= damage;
        if (health == 0)
        {
            KillPlayer();
        }
    }

    public static void HealPlayer(float healAmount)
    {
        health = Mathf.Min(maxHealth, health + healAmount);
    }

    public static void IncrementHealth(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    private static void KillPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            SceneManager.LoadSceneAsync(7);
        }
    }
}
