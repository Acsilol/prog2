using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;
//TEMPORARY


public class Enemy : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Transform target;
    public Animator animator;
    BoxCollider2D bx2d;

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

    public GameObject pythonPrefab;
    private Vector3 spawnPos;

    public ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        // Getting rigidbody and seeker(Pathfinder)
        ps = GetComponent<ParticleSystem>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        bx2d = GetComponent<BoxCollider2D>();
        ps.playOnAwake = false;

        // UpdatePath method gets called first at 0f then repeats after .3f 
        InvokeRepeating("UpdatePath", 0f, .3f);
    }

    private void Update()
    {
        // Animating the AI movement
        Vector2 movement_vector = new Vector2(rb.velocity.x, rb.velocity.y);
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
        target = GameObject.Find("Player2").transform;
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
    }



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
            SpawnEnemy(2);
            Destroy(collision.gameObject);
            GameObject.Find("Player2").GetComponent<PlayerControl>().killCounter += 1;
            StartCoroutine(ParticlePlay());
            Destroy(gameObject);
        }
    }

    IEnumerator ParticlePlay() 
    {
        ps.Play();        
        yield return new WaitForSeconds(3f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerControl>().TakeDamage(50);
            Vector2 kb = transform.position - other.transform.position;
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(kb * -3, ForceMode2D.Impulse);
            SpawnEnemy(20);
            Destroy(gameObject);
        }
    }

    public void SpawnEnemy(int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            spawnPos = new Vector3(Random.Range(70, -70), Random.Range(40, -40), target.position.z);
            GameObject enemy = Instantiate(pythonPrefab, spawnPos, Quaternion.identity);
            enemy.name = "Python";
        }        
    }

}