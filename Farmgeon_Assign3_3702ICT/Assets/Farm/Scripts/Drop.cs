using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public string stat;
    public Sprite sprite;

    void Update()
    {
        // Rotates and moves up and down
        transform.Rotate(new Vector3(0, 1.5f, 0));
        transform.Translate(new Vector3(0, Mathf.Sin(transform.rotation.y/150) ,0));
    }

    // Player picks up the loot
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            other.SendMessage("GainStat", stat);

            Destroy(gameObject);
        }
    }
}
