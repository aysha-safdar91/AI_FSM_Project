﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float HealthPoints
    {
        get { return healthPoints; }
        set
        {
            healthPoints = value;
            if (healthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    [SerializeField]
    public float healthPoints = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
