using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 10f;
    public float speed = 1f;
    public float minDist = 3f;
    public float maxDist = 10f;
    public Transform target;
    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;

        if (target == null) {
            if (GameObject.FindWithTag("Player")!=null)
            {
                target = GameObject.FindWithTag("Player").GetComponent<Transform>();
            }
        }
    }

    void Update() 
    {
        if (target == null)
            return;
        
        transform.LookAt(target);
        float distance = Vector3.Distance(transform.position,target.position);

        if (distance > minDist && distance < maxDist)	
            transform.position += transform.forward * speed * Time.deltaTime;	
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public int TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
            player.UpdateXp(10);
            Destroy(gameObject);
            return 1;
        }
        return 0;
    }
}
