using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class HistoryManager
{
    private bool wasTimePassing = false;
    private float prevFrozenTime = 0.0f;
    private bool isFrozen = false;
    private readonly Rigidbody body;
    private readonly TimePassingManager timePassingManager;
    private readonly Transform parentTransform;
    private bool isActivelyControlled = false;
    private float prevDuration = 0.0f;
    private float timeFrontier = 0.0f;

    List<ObjectInstant> path = new List<ObjectInstant>();
    List<float> timestamps = new List<float>();
    ObjectInstant firstInstant = null;

    public HistoryManager(Rigidbody body, Transform parentTransform, TimePassingManager timePassingManager, bool isActive)
    {
        this.body = body;
        this.parentTransform = parentTransform;
        this.timePassingManager = timePassingManager;
        SetActiveControl(isActive);
        SetObjectFreeze(true);
        SetFirstInstant();
    }


    private void SetFirstInstant()
    {
        firstInstant = MakeInstant();
    }

    public void Update()
    {
        bool isTimePassing = IsTimePassing();
        if (!isFrozen)
        {
            AddInstant(MakeInstant());
        }

        float managerDuration = timePassingManager.GetDuration();
        if (!isTimePassing)
        {
            if (prevFrozenTime != managerDuration)
            {
                prevFrozenTime = managerDuration;
                ApplyNearestTemporalPose(managerDuration);
            }
        }

        if (wasTimePassing != isTimePassing)
        {
            if (!isTimePassing)
            {
                SetObjectFreeze(true);
                prevFrozenTime = managerDuration;
            }
            else
            {
                SetObjectFreeze(false);
                ApplyNearestTemporalVelocity(managerDuration);
                ClearInstantsAfterTime(managerDuration);
            }
        }
        wasTimePassing = isTimePassing;
    }

    public void SetActiveControl(bool isActive)
    {
        isActivelyControlled = isActive;
        if (isActivelyControlled)
        {
            body.isKinematic = false;
        }
    }

    void SetObjectFreeze(bool isFrozen)
    {
        this.isFrozen = isFrozen;
        if (!isActivelyControlled)
        {
            body.isKinematic = isFrozen;
        }
    }

    ObjectInstant GetNearestInstant(float managerDuration)
    {
        (int, int) indices = NearestSearch.findSortedClosest(timestamps.ToArray(), managerDuration);

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
                float interval = (managerDuration - startTime) / (timestamps[indices.Item2] - startTime);
                return ObjectInstant.Slerp(path[indices.Item1], path[indices.Item2], interval);
            }
        }
    }

    void ApplyNearestTemporalVelocity(float managerDuration)
    {
        ObjectInstant nearestInstant = GetNearestInstant(managerDuration);
        if (nearestInstant == null)
        {
            return;
        }
        body.velocity = nearestInstant.velocity;
        body.angularVelocity = nearestInstant.angularVelocity;
    }

    void ApplyNearestTemporalPose(float managerDuration)
    {
        ObjectInstant nearestInstant = GetNearestInstant(managerDuration);
        if (nearestInstant == null)
        {
            return;
        }
        parentTransform.position = nearestInstant.pose.GetT();
        parentTransform.rotation = nearestInstant.pose.GetR();
    }

    void ClearInstantsAfterTime(float managerDuration)
    {
        (int, int) indices = NearestSearch.findSortedClosest(timestamps.ToArray(), managerDuration);
        if (indices.Item1 == -1 && indices.Item2 == -1)
        {
            return;
        }
        else
        {
            int nearestIndex = Math.Max(indices.Item1, indices.Item2);
            ClearInstantsAtAndAfterIndex(nearestIndex);
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
    ObjectInstant MakeInstant()
    {
        return new ObjectInstant
        {
            pose = Matrix4x4.TRS(parentTransform.position, parentTransform.rotation, Vector3.one),
            velocity = body.velocity,
            angularVelocity = body.angularVelocity
        };

    }
    void AddInstant(ObjectInstant instant)
    {
        path.Add(instant);
        timestamps.Add(timePassingManager.GetDuration());
    }

    public List<Vector3> getPositions()
    {
        return path.ConvertAll(pose => pose.pose.GetT());
    }

    bool IsTimePassing()
    {
        float currentDuration = timePassingManager.GetDuration();
        float delta = currentDuration - prevDuration;
        prevDuration = currentDuration;
        bool movingIntoFuture = timePassingManager.GetDuration() >= timeFrontier;
        if (movingIntoFuture)
        {
            timeFrontier = currentDuration;
        }
        return delta > 0.0f && movingIntoFuture;
    }


    public void NewMotionCallback()
    {
        RecordTimeFrontier();
    }

    private void RecordTimeFrontier()
    {
        timeFrontier = timePassingManager.GetDuration();
    }
}
