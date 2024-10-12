using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeFSM : MonoBehaviour
{
  public enum FSMModes{
        None,
        Protect,
        Attack,
        Run,
        Heal,
        Dead,
    }
    public FSMModes currentState;
    public float speed = 10.0f;
    public float range = 20.0f;
    public float health = 150.0f;
    public float damage = 50.0f;
    public GameObject drop;
    
    private Transform playerTransform;
    private NavMeshAgent nav;
    private GameObject[] healers;
    private GameObject currentHealer;
    private float pathTime;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        FindHealer();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMModes.None:
                break;
            case FSMModes.Protect:
                Protect();
                break;
            case FSMModes.Attack:
                Attack();
                break;
            case FSMModes.Run:
                Run();
                break;
            case FSMModes.Heal:
                Heal();
                break;
            case FSMModes.Dead:
                Dead();
                break;
        }

        pathTime += Time.deltaTime;

        if (health < 30.0f)
        {
            currentState = FSMModes.Run;
        }
        if (health <= 0.0f)
        {
            currentState = FSMModes.Dead;
        }
    }

    void FindHealer()
    {
        healers = GameObject.FindGameObjectsWithTag("Healer");
        currentHealer = healers[0];
        foreach (GameObject healer in healers)
        {
            float dist = Vector3.Distance(transform.position, healer.transform.position);
            float currentDist = Vector3.Distance(transform.position, currentHealer.transform.position);
            if (dist < currentDist)
            {
                currentHealer = healer;
            }
        }
    }

    void Protect()
    {

    }

    void Attack()
    {
        if (pathTime > 1.0f)
        {
            nav.SetDestination(playerTransform.position);
            pathTime = 0.0f;
        }
        
        float playerDist = Vector3.Distance(transform.position, playerTransform.position);
        if (playerDist < 5.0f)
        {
            nav.acceleration = 16;
        }
        else
        {
            nav.acceleration = 8;
        }
    }

    void Run()
    {

    }

    void Heal()
    {
        
    }

    void Dead()
    {
        nav.isStopped = true;
        Destroy(gameObject, 2.0f);
        GameObject loot = Instantiate(drop, transform);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.collider.tag == "Player" && currentState == FSMModes.Attack)
        {
            other.collider.SendMessage("ApplyDamage", damage);
        }
    }

    public void ApplyDamge(float amount)
    {
        health -= amount;
    }
}
