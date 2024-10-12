using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crop : MonoBehaviour
{
    public bool isPlanted = false;
    public GameObject[] cropList;

    public Image moodIndicator;
    
    private float water = 50.0f;
    private float growth = 0.0f;
    private float mood = 50.0f;
    private string stat;
    private bool isFertilized = false;
    private float fertilizeTime;
    private float elapsedTime;
    private Transform player;
    private GameObject currentCrop;

    private GameObject canvas;
    private Image waterMask;
    private Image moodMask;
    private Image growthMask;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        elapsedTime = 0.0f;

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
            water -= Time.deltaTime/2;
        }
        if (mood > 0)
        {
            float playerDist = Vector3.Distance(player.position, transform.position)/10;
            mood -= (water/100 + 1) * Time.deltaTime/2 * playerDist;
        }
        if (growth < 100)
        {
            elapsedTime = 0.0f;
            if (isFertilized)
            {
                growth += (water + mood)/100 * Time.deltaTime * 1.5f;
            }
            else
            {
                growth += (water + mood)/100 * Time.deltaTime;
            }
        }

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < 5.0f & isPlanted)
        {
            canvas.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            canvas.GetComponent<Canvas>().enabled = false;
        }

        UpdateUI();
        UpdateOverfeed();
        elapsedTime += Time.deltaTime;
        fertilizeTime -= Time.deltaTime;

        if (fertilizeTime < 0.0f & isFertilized)
        {
            isFertilized = false;
        }
    }

    void UpdateUI()
    {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, 0, player.transform.position.z) - transform.position);
            canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, rotation, Time.deltaTime * 10); 
            waterMask.fillAmount = water / 100;
            moodMask.fillAmount = mood / 100;
            growthMask.fillAmount = growth / 100;
    }

    void UpdateOverfeed()
    {
        if (growth > 100)
        {
            elapsedTime += Time.deltaTime;
            moodIndicator.enabled = true;
            if (elapsedTime > 30.0f)
            {
                isPlanted = false;
                GameObject plantNPC = Instantiate(currentCrop, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.rotation);
                // plantNPC.SendMessage("mood="+mood);
            }
        }
    }

    public void AddWater(float amount)
    {
        if (water < 100.0f)
        {
            water += amount;
            if (water > 100.0f)
            {
                water = 100.0f;
            }
        }
        else
        {
            mood -= amount/2;
        }
    }

    public void AddFire(float amount)
    {
        mood -= amount;
        water -= amount * 1.5f;
    }

    public void AddEarth()
    {
        isFertilized = true;
        fertilizeTime = 30.0f;
    }

    public void Interact(int seed)
    {
        if (growth >= 100.0f)
        {
            isPlanted = false;
            moodIndicator.enabled = false;
            player.SendMessage("GainStat", stat);
        }
        else if (!isPlanted)
        {
            GameObject cropObject = cropList[seed-1];
            switch (seed)
            {
                case 1:
                    stat = "strength";
                    break;
                case 2:
                    stat = "magic";
                    break;
                case 3:
                    stat = "vitality";
                    break;
            }
            Plant(cropObject);
        }
    }

    public void Plant(GameObject seed)
    {
        currentCrop = seed;
        isPlanted = true;
        growth = 0.0f;
        mood = 50.0f;
        water = 25.0f;
    }
}

