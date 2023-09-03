using UnityEngine;

public class TimePassingManager : MonoBehaviour
{
    private float duration = 0.0f;
    void Start()
    {

    }

    void Update()
    {

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

    }
}
