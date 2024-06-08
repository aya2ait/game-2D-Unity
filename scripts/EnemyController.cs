using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Wander,
    Follow,
    Die
    
}

public enum EnemyType
{
    Melee,
    Ranged
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 1f;
    GameObject player;
    public EnemyState currState = EnemyState.Idle;
    public EnemyType enemyType;
    public float range;
    public float speed;
    public float attackRange;
    public float bulletSpeed;
    public float coolDown;
    private bool chooseDir = false;
    //private bool dead = false;
    private bool coolDownAttack=false;
    
    public bool notInRoom = false;
    private Vector3 randomDir;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        InitializeEnemy();
        //SetRandomPosition();
    }
    private void InitializeEnemy()
    {
        health = maxHealth;
        currState = EnemyState.Wander;  // Ou tout autre état initial approprié
    }
    private void SetRandomPosition()
    {
        // Initialisation de la caméra et calcul des dimensions
        Camera mainCamera = Camera.main;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        // Définition des limites basées sur la taille de la caméra
        Vector2 positionMin = new Vector2(-width / 2, -height / 2);
        Vector2 positionMax = new Vector2(width / 2, height / 2);

        // Génère une nouvelle position aléatoire à l'intérieur des limites spécifiées
        Vector2 randomPosition = new Vector2(
            Random.Range(positionMin.x, positionMax.x),
            Random.Range(positionMin.y, positionMax.y)
        );

        // Applique la position aléatoire à l'objet
        transform.position = randomPosition;
    }

    // Update is called once per frame
    void Update()
    {
        AvoidOtherEnemies();
        switch (currState)
        {
           // case(EnemyState.Idle):
              // Idle();
              // break;
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Die:
                Death();
                break;
           
        }
        if(!notInRoom){
         if (IsPlayerInRange(range) && currState != EnemyState.Die)
         {
            currState = EnemyState.Follow;
         }
         else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
         {
            currState = EnemyState.Wander;
         }
        
        }
        else
        {
            currState=EnemyState.Idle;
        }
        
    }
    void AvoidOtherEnemies()
    {
    float separationDistance = 2f; // Distance pour maintenir entre les ennemis
    Vector2 separationVector = Vector2.zero;
    int count = 0;

    // Trouver tous les autres ennemis à proximité
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, separationDistance);
    foreach (var hit in hits)
    {
        if (hit != null && hit.gameObject != gameObject && hit.gameObject.CompareTag("Enemy"))
        {
            separationVector += (Vector2)(transform.position - hit.transform.position);
            count++;
        }
    }

    if (count > 0)
    {
        separationVector /= count; // Calculer la moyenne des directions opposées
        separationVector = separationVector.normalized * speed * Time.deltaTime;
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + separationVector, 0.5f);
    }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }
    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(0, 0));
        randomDir = new Vector3(0, 0, Random.Range(0, 0));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        //transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        chooseDir = false;
    }


    void Wander()
    {
        if (!chooseDir)
        {
            StartCoroutine(ChooseDirection());
        }
        transform.position += -transform.right * speed * Time.deltaTime;
        if (IsPlayerInRange(range))
        {
            currState = EnemyState.Follow;
        }
    }

    void Follow()
    {
        Vector3 target = player.transform.position - transform.position;
        float angle = Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
    
    private IEnumerator CoolDown(){
        coolDownAttack=true;
        yield return new WaitForSeconds(coolDown);
        coolDownAttack=false;
    }
    public void Death()
    {
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        Destroy(gameObject,0.01f);
    }
    public void TakeDamage(float damage)
    {
    health -= damage;
    Debug.Log($"{gameObject.name} health: {health}");
    if (health <= 0)
    {
        Death();
    }
    }
    



}