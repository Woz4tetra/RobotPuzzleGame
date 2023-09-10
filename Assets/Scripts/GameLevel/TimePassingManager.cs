using System.Collections.Generic;
using UnityEngine;

public class TimePassingManager : MonoBehaviour
{
    private float levelDuration = 0.0f;
    private int seekIndex = 0;
    private List<SceneInstant> instants = new List<SceneInstant>();

    public float GetLevelDuration()
    {
        return levelDuration;
    }

    public void RecordEvent(SceneInstant instant)
    {
        seekIndex += 1;
        if (0 <= seekIndex && seekIndex < instants.Count - 1)
        {
            instants.RemoveRange(seekIndex, instants.Count - seekIndex);
        }

        instants.Add(instant);
    }

    public void SeekTime(float delta)
    {
        levelDuration += delta;
        if (levelDuration < 0.0f)
        {
            levelDuration = 0.0f;
        }
    }

    public SceneInstant JumpToEvent(int eventDelta)
    {
        if (instants.Count == 0)
        {
            return null;
        }
        int newIndex = eventDelta + seekIndex;
        if (newIndex < 0)
        {
            newIndex = 0;
        }
        else if (newIndex >= instants.Count)
        {
            newIndex = instants.Count - 1;
        }
        seekIndex = newIndex;
        SceneInstant instant = instants[seekIndex];
        levelDuration = instant.levelDuration;
        return instant;
    }
}
