using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EnemyCasterAI : MonoBehaviour
{
    // Enemy health
    public int maxHealth = 100;
    public int currentHealth;   
    bool isDead = false;

    public HealthBar healthBar;

    public Transform target;
    public Animator animator;    

    public static int attackDamage = 5;
    public float nextFire;
    public float attackSpeed = 0.7f;
    
    private float attackRange = 30f;
    private bool inRange = false;
    public GameObject enemyBullet;
    public GameObject enemyPrefab;

    // Follow script variables
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Rigidbody2D rb;
    CircleCollider2D cc2d;

    private int shotCounter = 0;
    private int maxShotCounter = 30;
    private bool oneTimeSpawn = false;

    //DIALOG
    DialogueTrigger dialogueTrigger;

    public Slider slider;


    // Start is called before the first frame update
    void Start()
    {
        nextFire = 30f;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        
        rb = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();        
        dialogueTrigger = GetComponent<DialogueTrigger>();
        
        StartCoroutine(StartScaling());
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
        target = GameObject.Find("Player").transform;
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
        }
    }

    // When the CasterPinguin is spawned scales it up slowly, disables its hp + collider while
    IEnumerator StartScaling() 
    {        
        GameObject casterPenguin = GameObject.Find("LinuxPenguin");
        slider.gameObject.SetActive(false);
        cc2d.enabled = false;
        casterPenguin.transform.localScale = new Vector3(0f, 0f, 0f);
        for (int i = 1; i < 400; i++)
        {
            yield return new WaitForSeconds(.05f);
            casterPenguin.GetComponent<Transform>().localScale = new Vector3(((float)i)/100, ((float)i) / 100, ((float)i) / 100);
        }
        yield return new WaitForSeconds(5f);        
        cc2d.enabled = true;
        slider.gameObject.SetActive(true);
        nextFire = Time.time;
    }

    IEnumerator DeathDelay()
    {
        animator.SetBool("isDead", true);
        animator.SetBool("isEnraged", false);
        isDead = true;
        Destroy(cc2d);
        int destroyTime = 3;
        yield return new WaitForSeconds(destroyTime);
        GameObject.Find("SceneManager").GetComponent<SceneLoader>().isLevel1Complete = true;
        Destroy(GameObject.Find("Enemy(Clone)"));
        Destroy(gameObject);        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(GameObject.Find("Player").GetComponent<PlayerController>().damage);
            Vector2 kb = new Vector2(10f, 10f);
            Destroy(collision.gameObject);
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= (int)damage;
        healthBar.SetHealth(currentHealth);        
    }

    void Shoot() 
    {        
        if (Time.time > nextFire)
        {
            Instantiate(enemyBullet, transform.position, Quaternion.identity);            
            nextFire = Time.time + attackSpeed;
            shotCounter++;
        }
        if (shotCounter == maxShotCounter)
        {
            SpawnNewEnemy();
            shotCounter = 0;
        }
    }

    void SpawnNewEnemy()
    {
        
        StartCoroutine(SpawnEnemyCoroutine());
    }
       

    IEnumerator SpawnEnemyCoroutine() 
    {
        dialogueTrigger.TriggerDialogue();        
        Animator dialogue = GameObject.Find("DialogueBox").GetComponent<Animator>();
        yield return new WaitForSeconds(5);        
        Instantiate(enemyPrefab, transform.position, transform.rotation);             
        yield return new WaitForSeconds(3);
        dialogue.SetBool("isOpen", false);
    }

    void Enrage() 
    {
        animator.SetBool("isEnraged", true);
        attackDamage = 15;
        attackSpeed = 0.5f;        
        if (oneTimeSpawn == false)
        {
            SpawnNewEnemy();
            oneTimeSpawn = true;
        }
    }
    
}

