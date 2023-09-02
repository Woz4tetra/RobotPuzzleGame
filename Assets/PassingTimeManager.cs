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

    public bool IsTimePassing()
    {
        return timePassing;
    }

    public float GetDuration()
    {
        return duration;
    }

    public void setTimePassing(bool isTimePassing)
    {
        timePassing = isTimePassing;
    }

    public void seekTime(float delta)
    {
        duration += delta;
        if (duration < 0.0f)
        {
            duration = 0.0f;
        }
    }
}
