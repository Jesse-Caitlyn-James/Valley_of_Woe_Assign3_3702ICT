using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeProj : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // future position if bullet doesn't hit any colliders
		Vector3 newPos = transform.position + transform.forward * 10.0f * Time.deltaTime;
        Quaternion playerRotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, Time.deltaTime * 100f); 

		// see if bullet hits a collider
		RaycastHit hit;
		if (Physics.Linecast(transform.position, newPos, out hit))
		{
            if (hit.collider.tag == "Player")
            {
                hit.collider.SendMessage("ApplyDamage", 20f);
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
