using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100.0f;
    public string type;
    // Start is called before the first frame update
    void Start()
    {
        
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

            if (hit.collider.tag == "Enemy" | hit.collider.tag == "Healer")
            {
                if (type == "water")
                {
                    hit.collider.SendMessage("Heal", 10.0f);
                }
                else
                {
                    hit.collider.SendMessage("ApplyDamge", 25.0f);
                }
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
