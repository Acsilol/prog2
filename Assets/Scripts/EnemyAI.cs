using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;
//TEMPORARY


public class EnemyAI : MonoBehaviour
{    
    // Enemy health
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject enemyPrefab;
    bool isDead = false;
    
    public HealthBar healthBar;

    public Transform target;
    public Animator animator;
    CircleCollider2D cc2d;

    private int attackDamage = 10;
    private float attackSpeed = 1f;
    private float canAttack;

    // Follow script variables
    public float speed = 200f;
    public float nextWaypointDistance = 3f;    

    // Follow script variables
    Path path;
    int currentWaypoint = 0;
    public bool reachedEndOfPath;
    Seeker seeker;
    Rigidbody2D rb;

    /*private void Awake()
    {
        target = GetComponent<PlayerController>().GetComponent<Transform>();
    }*/


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Getting rigidbody and seeker(Pathfinder)
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();

        // UpdatePath method gets called first at 0f then repeats after .3f 
        InvokeRepeating("UpdatePath", 0f, .3f);
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

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        target = GameObject.Find("Player").transform;
        if (isDead == false)
        {
            if (path == null)
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
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (currentHealth < 0.0001)
            {
                StartCoroutine(DeathDelay());                                
            }
        }
    }

    IEnumerator DeathDelay()
    {
        animator.SetBool("isDead", true);       
        isDead = true;
        Destroy(cc2d);
        int destroyTime = 3;
        yield return new WaitForSeconds(destroyTime);        
        Destroy(gameObject);
        //SpawnNew();
        
    }

    /*void SpawnNew() 
    {
        Instantiate(enemyPrefab, target.position, target.rotation);
    }*/

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
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
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(attackDamage);
            Vector2 kb = transform.position - other.transform.position;
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(kb*-3, ForceMode2D.Impulse);
            animator.SetBool("isAttacking", true);
            animator.SetBool("isWalking", false);
            canAttack = 0f;           
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", true);
        }
    }
    /*private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (attackSpeed <= canAttack)
            {
                other.gameObject.GetComponent<PlayerController>().TakeDamage(attackDamage);
                          
                // player.GetComponent<Rigidbody2D>().AddForce(kb, ForceMode2D.Impulse);                
                canAttack = 0f;
            }
            else canAttack += Time.deltaTime;
        }        
    }    */
}

