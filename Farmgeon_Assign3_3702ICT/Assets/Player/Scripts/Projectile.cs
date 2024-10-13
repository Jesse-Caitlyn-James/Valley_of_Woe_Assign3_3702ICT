using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100.0f;
    public string type;

    private float life;
    private float damage;
    
    // Start is called before the first frame update
    void Start()
    {
        damage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().damage;
        life = 5.0f;
        Destroy(gameObject, life);
    }

    // Update is called once per frame
    void Update()
    {
        // future position if bullet doesn't hit any colliders
		Vector3 newPos = transform.position + transform.forward * speed * Time.deltaTime;

		// see if bullet hits a collider
		RaycastHit hit;
		if (Physics.Linecast(transform.position, newPos, out hit))
		{
			// Checks if the collider was important and sends a corresponding message
        	if (hit.collider.tag == "Farmland")
	        {
                // Different types of bullets
                switch (type)
                {
                    case "water":
                        hit.collider.SendMessage("AddWater", 25.0f);
                        break;
                    case "fire":
                        hit.collider.SendMessage("AddFire", 20.0f);
                        break;
                    case "earth":
                        hit.collider.SendMessage("AddEarth");
                        break;
                }
	        }

            // Checks what the bullet hits
            if (hit.collider.tag == "Enemy" | hit.collider.tag == "Healer")
            {
                if (type == "water")
                {
                    hit.collider.SendMessage("ApplyHeal", damage/2);
                }
                else
                {
                    hit.collider.SendMessage("ApplyDamge", damage);
                }
            }

            // Boss Damage
            if (hit.collider.tag == "WeakSpot")
            {
                GameObject boss = GameObject.FindGameObjectWithTag("Boss");
                boss.SendMessage("ApplyDamage", damage);
            }

            if (hit.collider.tag == "StartPoint")
            {
                GameObject boss = GameObject.FindGameObjectWithTag("Boss");
                boss.SendMessage("StartFight");
            }

			// Destroys the bullet
			if (hit.collider)
			{
				Destroy(gameObject);
			}
		}
		else
		{
			transform.position = newPos;
		}     
    }
}
