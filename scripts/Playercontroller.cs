using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float Speedslow;
    public float detectionRange = 5f;
    private Rigidbody2D rigidbody2D;
    public TextMeshProUGUI collectedText;
    public static int collectedAmount = 0;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    private float lastFire;
    private float fireDelay = 0.5f;
    public static PlayerController instance;
    private static int stepIndex = 0;
    private Transform target;
    private string initialSceneName = "BasementMain"; // Initial scene name
    private string finalSceneName = "BasementEnd";
    private Vector3 center;

    private void Start()

    {
        // collectedAmount = 0;
        rigidbody2D = GetComponent<Rigidbody2D>();
        center = new Vector3(0, 0, 0);
        transform.position = center + new Vector3(3f, 0, 0); // Start at the rightmost point of the circle
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameObject transitionObject = GameObject.FindGameObjectWithTag("Transition");
        if (transitionObject != null)
        {
            DontDestroyOnLoad(transitionObject);
        }
    }

   private void Update()
{
    if (SceneManager.GetActiveScene().name == initialSceneName)
    {
        ManualMovement();
    }
    else
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("item");
        if (items.Length > 0)
        {
            MoveTowardsItemsAndShootEnemies();
        }
        else
        {
            MoveTowardsTransition();
        }
    }

    collectedText.text = "Items Collected: " + collectedAmount;
}


    private void ManualMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float shootHor = Input.GetAxis("ShootHorizontal");
        float shootVert = Input.GetAxis("ShootVertical");

        if ((shootHor != 0 || shootVert != 0) && Time.time > lastFire + fireDelay)
        {
            Shoot(shootHor, shootVert);
            lastFire = Time.time;
        }

        rigidbody2D.velocity = new Vector3(horizontal * speed, vertical * speed, 0);
    }

    private void MoveTowardsItemsAndShootEnemies()
    {
        // Move towards the nearest item
        GameObject[] items = GameObject.FindGameObjectsWithTag("item");
        if (items.Length > 0)
        {
            GameObject nearestItem = FindNearestItem(items);
            MoveTowardsTarget(nearestItem.transform.position);

            // Check if the player is close enough to the item to collect it
            float distanceToItem = Vector3.Distance(transform.position, nearestItem.transform.position);
            if (distanceToItem < 50f) // Adjust this value as needed for your game
            {
                // Collect the item
                CollectItem(nearestItem);
            }
        }
        else
        {
            // If no more items are left in the room, move towards the transition
            MoveTowardsTransition();
        }

        // Detect and shoot enemies within range
        DetectAndShootEnemiesInRange();
    }

    private GameObject FindNearestItem(GameObject[] items)
    {
        GameObject nearestItem = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject item in items)
        {
            float distanceToItem = Vector3.Distance(currentPosition, item.transform.position);
            if (distanceToItem < shortestDistance)
            {
                shortestDistance = distanceToItem;
                nearestItem = item;
            }
        }

        return nearestItem;
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rigidbody2D.velocity = direction * speed;
    }

    private void CollectItem(GameObject item)
    {
        // Add your item collection logic here
       // Destroy(item); // Example: Destroy the item GameObject
        collectedAmount++; // Example: Increment collected amount
    }

    private void DetectAndShootEnemiesInRange()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.CompareTag("Enemy") || enemyCollider.CompareTag("dusman"))
            {
                Vector3 enemyDirection = (enemyCollider.transform.position - transform.position).normalized;
                if (Time.time > lastFire + fireDelay)
                {
                    Shoot(enemyDirection.x, enemyDirection.y);
                    lastFire = Time.time;
                }
            }
        }
    }

    private void Shoot(float x, float y)
    {
       GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed,
            0
        );
    }


    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Transition")&&collectedAmount>10)
    {
        List<string> path = Graph.path;
        if (stepIndex < path.Count)
        {
            string sceneToLoad = "Basement" + path[stepIndex];
            SceneManager.LoadScene(sceneToLoad);
            stepIndex++;
        }
        else
        {
            Debug.Log("All steps have been loaded.");
            ObjectGenerator objectGenerator = FindObjectOfType<ObjectGenerator>();
            if (objectGenerator != null)
            {
                objectGenerator.GenerateObject();
                Destroy(gameObject);
                SceneManager.LoadSceneAsync(7);
                MoveTowardsCoin();

                // Désactiver le collider de l'objet de transition
                other.enabled = false;
            }
            else
            {
                Debug.LogWarning("ObjectGenerator not found in the scene.");
            }

            // Change player position after touching transition in other scenes
            /*if (SceneManager.GetActiveScene().name != initialSceneName && SceneManager.GetActiveScene().name != finalSceneName)
            {
                transform.position = new Vector3(-12f, 8f, 0f);
            }*/
        }
    }
}


        private void MoveTowardsCoin()
{
    GameObject coinObject = GameObject.FindGameObjectWithTag("bonus");
    if (coinObject != null)
    {
        MoveTowardsTarget(coinObject.transform.position);
    }
    else
    {
        Debug.LogWarning("Coin object not found in the scene.");
    }
}

    public void ApplySlow(float slowAmount)
    {
        // Ralentir la vitesse du joueur en fonction du slowAmount (entre 0 et 1)
        Speedslow = speed * slowAmount;
    }

    private void MoveTowardsTransition()
    {
        GameObject transitionObject = GameObject.FindGameObjectWithTag("Transition");
        if (transitionObject != null)
        {
            MoveTowardsTarget(transitionObject.transform.position);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 centerPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
            transform.position = new Vector3(centerPosition.x, centerPosition.y, 0);
        }

        if (scene.name != initialSceneName && scene.name != finalSceneName)
        {
            GameObject transitionObject = GameObject.FindGameObjectWithTag("Transition");
            if (transitionObject != null)
            {
                target = transitionObject.transform;
                Invoke(nameof(MoveTowardsTransitionObject), 0.1f); // Start moving towards transition after a short delay to ensure the scene is fully loaded
            }
        }
         else if (scene.name == finalSceneName)
        {
            // Si la scène chargée est la scène finale, déplacer le joueur vers l'objet avec le tag "coin"
            MoveTowardsCoin();
        }
    }

    private void MoveTowardsTransitionObject()
    {
        if (target != null)
        {
            MoveTowardsTarget(target.position);
        }
    }
}