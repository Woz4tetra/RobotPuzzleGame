using UnityEngine;

public class PassingTimeManager : MonoBehaviour
{
    private float duration = 0.0f;
    private float savePointDuration = 0.0f;
    private bool timePassing = false;
    void Start()
    {

    }

    void Update()
    {

    }

    public bool IsTimePassing()
    {
        return timePassing;
    }

    public float GetDuration()
    {
        return duration;
    }


    public void SeekTime(float delta)
    {
        duration += delta;
        if (duration < 0.0f)
        {
            duration = 0.0f;
        }
        bool movingIntoFuture = duration >= savePointDuration;
        if (movingIntoFuture)
        {
            savePointDuration = duration;
        }
        timePassing = delta > 0.0f && movingIntoFuture;
    }

    public void ResumeFromSave()
    {
        savePointDuration = duration;
    }
}
