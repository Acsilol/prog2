using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;


public class EnemyCasterAI : MonoBehaviour
{
    // Enemy health
    public int maxHealth = 100;
    public int currentHealth;   
    bool isDead = false;

    public HealthBar healthBar;

    public Transform target;
    public Animator animator;    

    public static int attackDamage = 80;
    public float nextFire;
    public float attackSpeed = 0.7f;
    
    private float attackRange = 30f;
    private bool inRange = false;
    public GameObject enemyBullet;

    // Follow script variables
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    // Follow script variables
    //Path path;
    //int currentWaypoint = 0;
    //public bool reachedEndOfPath;
    //Seeker seeker;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        nextFire = Time.time;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Getting rigidbody and seeker(Pathfinder)
        //seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        // UpdatePath method gets called first at 0f then repeats after .5f 
        //InvokeRepeating("UpdatePath", 0f, .3f);
    }

    private void Update()
    {
        // Animating the AI movement
        Vector2 movement_vector = new Vector2(rb.velocity.x, rb.velocity.y);
        if (movement_vector != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("Horizontal", movement_vector.y);
            animator.SetFloat("Vertical", movement_vector.x);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }    

    // Update is called once per frame

    void FixedUpdate()
    {
        if (isDead == false)
        {
            if (currentHealth < 0.0001)
            {
                StartCoroutine(DeathDelay());                
            }
            inRange = Vector2.Distance(transform.position, target.position) < attackRange;

            if (currentHealth < maxHealth * 0.4 && isDead == false)
            {
                Enrage();
            }

            if (inRange)
            {
                Shoot();
            }
            /*if (path == null)
            {
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }*/

            // Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            //Vector2 force = direction * speed * Time.deltaTime;

            //rb.AddForce(force);

            //float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            /*if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }*/

            
        }
    }

    IEnumerator DeathDelay()
    {
        animator.SetBool("isDead", true);
        animator.SetBool("isEnraged", false);
        isDead = true;
        int destroyTime = 3;
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(BulletController.damage);
            Vector2 kb = new Vector2(10f, 10f);
            Destroy(collision.gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    void Shoot() 
    {
        if (Time.time > nextFire)
        {
            Instantiate(enemyBullet, transform.position, Quaternion.identity);
            nextFire = Time.time + attackSpeed;
        }
    }

    void Enrage() 
    {
        animator.SetBool("isEnraged", true);
        attackDamage = 40;
        attackSpeed = 0.1f;
    }
    
}

