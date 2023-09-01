using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingTimeManager : MonoBehaviour
{
    private float duration = 0.0f;
    private bool timePassing = false;
    void Start()
    {

    }

    void Update()
    {
        if (timePassing)
        {
            duration += Time.deltaTime;
        }
    }

    public bool isTimePassing()
    {
        return timePassing;
    }

    public float getDuration()
    {
        return duration;
    }

    public void setTimePassing(bool isTimePassing)
    {
        this.timePassing = isTimePassing;
    }
}
