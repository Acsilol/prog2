using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
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
    public float damage = 25;

    public int killCounter = 0;
    public Text text;
    public Image victory;


    private void Start()
    {
        // Setting the currentHealth to maxHealth on Start()
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        GameObject.Find("Python").GetComponent<Enemy>().SpawnEnemy(10);
    }

    void FixedUpdate()
    {
        if (currentHealth < 0.0001)
        {
            SceneManager.LoadScene("LevelManager");
        }

        if (killCounter == 30)
        {
            Destroy(GameObject.Find("Python"));
            StartCoroutine(EndLevel());

        }
        // Processing inputs
        ProcessInputs();
        Move();

        text.text = killCounter.ToString();

        animator.SetFloat("Horizontal", Input.GetAxisRaw("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxisRaw("Vertical"));
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
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
        if ((shootHorizontal != 0 || shootVertical != 0) && Time.time > lastFire + fireDelay)
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
            rb.AddForce(kb * 2, ForceMode2D.Impulse);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator EndLevel()
    {
        victory.GetComponent<Transform>().localPosition = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(5f);         
    }

}
