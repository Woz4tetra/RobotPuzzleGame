using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathTracker : MonoBehaviour
{
    private Rigidbody body;
    private bool wasTimePassing = false;
    private float prevFrozenTime = 0.0f;
    private bool isFrozen = false;
    [SerializeField] private PassingTimeManager passingTimeManager;
    [SerializeField] private bool isMainController = false;

    List<ObjectKinematicProperties> path = new List<ObjectKinematicProperties>();
    List<float> timestamps = new List<float>();

    void Start()
    {
        body = GetComponent<Rigidbody>();
        SetObjectFreeze(true);
    }

    void Update()
    {
        bool isTimePassing = passingTimeManager.IsTimePassing();
        if (!isFrozen)
        {
            AddInstant(Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one), body.velocity, body.angularVelocity);
        }

        float managerDuration = passingTimeManager.GetDuration();
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
                ClearInstantsAfter(managerDuration);
            }
        }
        wasTimePassing = isTimePassing;
    }

    void SetObjectFreeze(bool isFrozen)
    {
        this.isFrozen = isFrozen;
        if (!isMainController)
        {
            body.isKinematic = isFrozen;
        }
    }

    ObjectKinematicProperties GetNearestTemporyInstant(float managerDuration)
    {
        int nearestIndex = NearestSearch.findSortedClosest(timestamps.ToArray(), managerDuration);
        if (nearestIndex == -1)
        {
            return null;
        }
        else
        {
            return path[nearestIndex];
        }
    }

    void ApplyNearestTemporalVelocity(float managerDuration)
    {
        ObjectKinematicProperties nearestInstant = GetNearestTemporyInstant(managerDuration);
        if (nearestInstant == null)
        {
            return;
        }
        body.velocity = nearestInstant.velocity;
        body.angularVelocity = nearestInstant.angularVelocity;
    }

    void ApplyNearestTemporalPose(float managerDuration)
    {
        ObjectKinematicProperties nearestInstant = GetNearestTemporyInstant(managerDuration);
        if (nearestInstant == null)
        {
            return;
        }
        transform.position = nearestInstant.pose.GetT();
        transform.rotation = nearestInstant.pose.GetR();
    }

    void ClearInstantsAfter(float managerDuration)
    {
        int nearestIndex = NearestSearch.findSortedClosest(timestamps.ToArray(), managerDuration);
        if (nearestIndex == -1)
        {
            return;
        }
        else
        {
            path.RemoveRange(nearestIndex, path.Count - nearestIndex);
            timestamps.RemoveRange(nearestIndex, timestamps.Count - nearestIndex);
        }
    }

    void AddInstant(Matrix4x4 pose, Vector3 velocity, Vector3 angularVelocity)
    {
        ObjectKinematicProperties poseTimeStamped = new ObjectKinematicProperties
        {
            pose = pose,
            velocity = velocity,
            angularVelocity = angularVelocity
        };
        path.Add(poseTimeStamped);
        timestamps.Add(passingTimeManager.GetDuration());
    }

    List<Vector3> getPositions()
    {
        return path.ConvertAll(pose => pose.pose.GetT());
    }

    void OnDrawGizmos()
    {
        ReadOnlySpan<Vector3> positions = getPositions().ToArray();
        Gizmos.DrawLineStrip(positions, false);
    }
}
