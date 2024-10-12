using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crop : MonoBehaviour
{
    public bool isPlanted = false;
    public string crop = "Carrot";
    
    private float water = 50.0f;
    private float growth = 0.0f;
    private float mood = 50.0f;
    private Transform player;

    private GameObject canvas;
    private Image waterMask;
    private Image moodMask;
    private Image growthMask;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canvas = transform.Find("Canvas").gameObject;
        waterMask = transform.Find("Canvas").Find("Water").Find("Image").gameObject.GetComponent<Image>();
        moodMask = transform.Find("Canvas").Find("Mood").Find("Image").gameObject.GetComponent<Image>();
        growthMask = transform.Find("Canvas").Find("Growth").Find("Image").gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (water > 0)
        {
            water -= Time.deltaTime/1;
        }
        if (mood > 0)
        {
            mood -= (water/100 + 1) * Time.deltaTime;
        }
        if (growth < 100)
        {
            growth += (water + mood)/100 * Time.deltaTime;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < 5.0f)
        {
            canvas.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            canvas.GetComponent<Canvas>().enabled = false;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        Quaternion rotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, 0, player.transform.position.z) - transform.position);
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, rotation, Time.deltaTime * 10); 
        waterMask.fillAmount = water / 100;
        moodMask.fillAmount = mood / 100;
        growthMask.fillAmount = growth / 100;
    }

    public void AddWater(float amount)
    {
        water += amount;
    }
}
