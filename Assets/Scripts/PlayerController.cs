using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player health
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    
    public Camera cam;

    public float moveSpeed;
    public Rigidbody2D rb;
    private Vector2 moveDirection;    

    public Animator animator;

    public GameObject bulletPrefab;
    public float bulletSpeed;
    private float lastFire;
    public float fireDelay;

    // EnemyCaster variables
    Vector3 enemyStartingPos = new Vector3(6f, 0f, 0f);
    public GameObject casterPrefab;

    private void Start()
    {
        // Setting the currentHealth to maxHealth on Start()
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        StartCoroutine(SpawnCaster());
    }

    void FixedUpdate()
    {        
        Move();
        // Processing inputs
        ProcessInputs();
        

        animator.SetFloat("Horizontal", Input.GetAxisRaw("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxisRaw("Vertical"));
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);        
    }

    IEnumerator SpawnCaster() 
    {
        yield return new WaitForSeconds(5f);
        GameObject EnemyCaster = Instantiate(casterPrefab, enemyStartingPos, Quaternion.identity);

        
    }

    void Shoot(float x, float y)
    {
        // Rotating the bullet
             
        float zModified = 0;
        
        if (x > 0) zModified = 0f;
        if (x < 0) zModified = 180f;
        if (y > 0) zModified = 90;
        if (y < 0) zModified = 270f;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
       
        bullet.transform.Rotate(0f, 0f, zModified, Space.Self);

        // Calculating the velocity of the bullet based on input

        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
            // x            
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed, 
            // y 
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed, 
            // z 
            0
        );
    }

    void ProcessInputs() 
    {     
        // Processing Horizontal/Vertical inputs (WASD) -> player movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Processing inputs (arrows) and calling Shoot() method
        float shootHorizontal = Input.GetAxis("ShootHorizontal");
        float shootVertical = Input.GetAxis("ShootVertical");
        if ((shootHorizontal != 0 || shootVertical != 0 ) && Time.time > lastFire + fireDelay)
        {
            Shoot(shootHorizontal, shootVertical);                      
            lastFire = Time.time;            
        }

        moveDirection = new Vector2(moveX, moveY).normalized;        
    }

    void Move() 
    {
        rb.AddForce(new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed));
    }

    // TakeDamage() function 
    public void TakeDamage(int damage) 
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CasterBullet")
        {
            TakeDamage(EnemyCasterAI.attackDamage);
            Vector2 kb = transform.position - collision.transform.position;
            rb.AddForce(kb*2, ForceMode2D.Impulse);
            Destroy(collision.gameObject);
        }
    }
}
