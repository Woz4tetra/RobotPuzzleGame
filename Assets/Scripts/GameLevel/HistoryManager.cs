using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class HistoryManager
{
    List<ObjectInstant> path = new List<ObjectInstant>();
    List<float> timestamps = new List<float>();

    public HistoryManager()
    {

    }


    public void RecordEvent(ObjectInstant instant, float levelDuration)
    {
        AddInstant(instant, levelDuration);
    }

    public ObjectInstant JumpToInstant(float levelDuration)
    {
        return GetNearestInstant(levelDuration);
    }

    public ObjectInstant UnfreezeObject(float levelDuration)
    {
        ObjectInstant instant = GetNearestInstant(levelDuration);
        ClearInstantsAfterTime(levelDuration);
        return instant;
    }

    ObjectInstant GetNearestInstant(float levelDuration)
    {
        (int, int) indices = NearestSearch.findSortedClosest(timestamps.ToArray(), levelDuration);

        if (indices.Item1 == -1 && indices.Item2 == -1)
        {
            return null;
        }
        else
        {
            if (indices.Item1 == -1)
            {
                return path[indices.Item2];
            }
            else if (indices.Item2 == -1)
            {
                return path[indices.Item1];
            }
            else
            {
                float startTime = timestamps[indices.Item1];
                float interval = (levelDuration - startTime) / (timestamps[indices.Item2] - startTime);
                return ObjectInstant.Slerp(path[indices.Item1], path[indices.Item2], interval);
            }
        }
    }

    void ClearInstantsAfterTime(float levelDuration)
    {
        (int, int) indices = NearestSearch.findSortedClosest(timestamps.ToArray(), levelDuration);
        if (indices.Item1 == -1 && indices.Item2 == -1)
        {
            return;
        }
        else
        {
            int nearestIndex = Math.Max(indices.Item1, indices.Item2);
            if (timestamps[nearestIndex] > levelDuration)
            {
                ClearInstantsAtAndAfterIndex(nearestIndex);
            }
        }
    }

    void ClearInstantsAtAndAfterIndex(int index)
    {
        if (index < 0 || index >= path.Count)
        {
            return;
        }
        Assert.IsTrue(path.Count == timestamps.Count);
        path.RemoveRange(index, path.Count - index);
        timestamps.RemoveRange(index, timestamps.Count - index);
    }

    void AddInstant(ObjectInstant instant, float levelDuration)
    {
        path.Add(instant);
        timestamps.Add(levelDuration);
    }

    public List<Vector3> getPositions()
    {
        return path.ConvertAll(pose => pose.pose.GetT());
    }
}
