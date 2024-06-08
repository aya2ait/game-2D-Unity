using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime;
    public float damage = 1f;  // Dégâts infligés par cette balle

    void Start() 
    {
        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") || col.CompareTag("dusman"))
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                DusmanController dusman = col.GetComponent<DusmanController>();
                if (dusman != null)
                {
                    dusman.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
       
    }
}
