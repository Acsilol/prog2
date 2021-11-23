using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime;    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeathDelay());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Optimizing bullets (gets deleted after .5 second)
    IEnumerator DeathDelay() 
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {        
        Destroy(gameObject);
        //GetComponent<PlayerController>().TakeDamage(20);
    }

}
