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

    public bool IsAtFrontier()
    {
        if (instants.Count == 0)
        {
            return true;
        }
        return seekIndex == instants.Count - 1;
    }

    public void RecordEvent(SceneInstant instant)
    {
        if (instants.Count > 0)
        {
            SceneInstant lastInstant = instants[instants.Count - 1];
            if (Mathf.Abs(instant.levelDuration - lastInstant.levelDuration) < 0.1f)
            {
                return;
            }
        }
        instants.Add(instant);
        seekIndex = instants.Count - 1;
    }

    public void Unfreeze()
    {
        float clearAfterTime = levelDuration;
        instants.RemoveAll(instant => instant.levelDuration > clearAfterTime);
    }

    public void MoveByDelta(float delta)
    {
        SeekTime(delta + GetLevelDuration());
    }

    private void SeekTime(float goal)
    {
        levelDuration = goal;
        if (levelDuration < 0.0f)
        {
            levelDuration = 0.0f;
        }
    }

    public void JumpToInstant(SceneInstant instant)
    {
        SeekTime(instant.levelDuration);
        seekIndex = instants.IndexOf(instant);
        Debug.Log($"Jumping to instant {seekIndex} at {levelDuration}");
    }

    public SceneInstant GetInstant(int eventDelta)
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
        if (newIndex == seekIndex)
        {
            return null;
        }
        Debug.Log($"Seek index is {seekIndex}. Requesting instant at {newIndex}");
        return instants[newIndex];
    }
}
