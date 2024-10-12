using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CropFSM : MonoBehaviour
{
    public enum FSMModes{
        None,
        RunPlayer,
        RunExit,
        Attack,
        Hide,
        Dead,
    }
    public FSMModes currentState;
    public float speed = 10.0f;
    public float range = 20.0f;
    public float health = 100.0f;
    public float damage = 50.0f;
    public float mood;
    public Transform[] escapePoints;
    public Transform[] hidePoints;
    public GameObject drop;
    
    private Transform playerTransform;
    private NavMeshAgent nav;
    private Transform exit;
    private float pathTime;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        exit = GameObject.FindGameObjectWithTag("Exit").transform;
        nav = GetComponent<NavMeshAgent>();

        if (mood > 50.0f)
        {
            currentState = FSMModes.RunPlayer;
        }
        else
        {
            currentState = FSMModes.Attack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMModes.None:
                break;
            case FSMModes.RunPlayer:
                RunPlayer();
                break;
            case FSMModes.RunExit:
                RunExit();
                break;
            case FSMModes.Attack:
                Attack();
                break;
            case FSMModes.Hide:
                Hide();
                break;
            case FSMModes.Dead:
                Dead();
                break;
        }

        pathTime += Time.deltaTime;

        if (health < 30.0f)
        {
            currentState = FSMModes.Hide;
        }
        if (health <= 0.0f)
        {
            currentState = FSMModes.Dead;
        }
    }

    void RunPlayer()
    {
        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            bool locationSet = false;
            foreach (Transform location in escapePoints)
            {
                RaycastHit playerSee;
                if (Physics.Linecast(location.position, playerTransform.position, out playerSee))
                {
                    if (playerSee.collider.tag != "Player")
                    {
                        nav.SetDestination(location.position);
                        locationSet = true;
                        break;
                    }
                }
            }
            if (!locationSet)
            {
                nav.SetDestination(exit.position);
            }
        }

        float playerDist = Vector3.Distance(playerTransform.position, transform.position);
        RaycastHit hit;
        if (Physics.Linecast(transform.position, playerTransform.position, out hit))
        {
            if (hit.collider.tag != "Player")
            {
                currentState = FSMModes.RunExit;
            }
        }
        if (playerDist > range)
        {
            currentState = FSMModes.RunExit;
        }
    }

    void RunExit()
    {
        if (pathTime > 1.0f)
        {
            nav.SetDestination(exit.position);
            pathTime = 0.0f;
        }

        float playerDist = Vector3.Distance(playerTransform.position, transform.position);
        if (playerDist < range)
        {
            currentState = FSMModes.RunPlayer;
        }
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

    void Hide()
    {
        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            if (nav.remainingDistance > 2.0f)
            {
                bool locationSet = false;
                foreach (Transform location in hidePoints)
                {
                    RaycastHit playerSee;
                    if (Physics.Linecast(location.position, playerTransform.position, out playerSee))
                    {
                        if (playerSee.collider.tag != "Player")
                        {
                            nav.SetDestination(location.position);
                            locationSet = true;
                            break;
                        }
                    }
                }
                if (!locationSet)
                {
                    nav.SetDestination(exit.position);
                }
            }
            else
            {
                RaycastHit playerSee;
                if (Physics.Linecast(transform.position, playerTransform.position, out playerSee))
                {
                    if (playerSee.collider.tag != "Player")
                    {
                        nav.SetDestination(exit.position);
                    }
                }
            }
        }
    }

    void Dead()
    {
        GameObject loot = Instantiate(drop, transform);
        nav.isStopped = true;
        Destroy(gameObject, 2.0f);
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
