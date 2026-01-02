using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{
    [Header("Joints")]
    [SerializeField] private List<Transform> joints = new();

    [Header("Target")]
    private MyVector2 targetPosition;
    private bool hasTarget = false;

    [Header("Algorithm Settings")]
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private int maxIterationsPerFrame = 100;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float jointRadius = 0.1f;

    private List<MyVector2> jointPositions;
    private List<float> boneLengths;
    private IKCollisionHandler collisionHandler;

    public int IterationsThisFrame { get; private set; }

    private int TotalCount => joints.Count + 1; // includes base

    private int LastIndex => TotalCount - 1;
    private MyVector2 BasePosition => new(transform.position.x, transform.position.y);

    private void Awake()
    {
        collisionHandler = new(collisionLayer, jointRadius);
        InitializeBoneLengths();
    }

    private void LateUpdate()
    {
        if (joints.Count < 1 || !hasTarget) { return; }

        UpdateJointPositionsFromBase();

        float distanceToTarget = MyVector2.Distance(jointPositions[LastIndex], targetPosition);

        if (distanceToTarget > tolerance)
        {
            IterationsThisFrame = 0;
            for (int i = 0; i < maxIterationsPerFrame; i++)
            {
                Forward();
                Backward();
                IterationsThisFrame++;
                if (MyVector2.Distance(jointPositions[LastIndex], targetPosition) <= tolerance) { break; }
            }

            SyncTransforms();
        }
        else
        {
            IterationsThisFrame = 0;
        }
    }

    public void SetTarget(MyVector2 p_position)
    {
        targetPosition = p_position;
        hasTarget = true;
    }

    private void InitializeBoneLengths()
    {
        boneLengths = new();

        if (joints.Count > 0)
        {
            MyVector2 basePos = new(transform.position.x, transform.position.y);
            MyVector2 firstJoint = new(joints[0].position.x, joints[0].position.y);
            boneLengths.Add(MyVector2.Distance(basePos, firstJoint));
        }

        for (int i = 0; i < joints.Count - 1; i++)
        {
            MyVector2 joint1 = new(joints[i].position.x, joints[i].position.y);
            MyVector2 joint2 = new(joints[i + 1].position.x, joints[i + 1].position.y);
            boneLengths.Add(MyVector2.Distance(joint1, joint2));
        }
    }

    private void UpdateJointPositionsFromBase()
    {
        jointPositions = new() { BasePosition };

        for (int i = 0; i < joints.Count; i++)
        {
            MyVector2 prevPos = jointPositions[i];
            MyVector2 currentPos = new(joints[i].position.x, joints[i].position.y);
            MyVector2 direction = (currentPos - prevPos).normalized;

            if (direction.sqrMagnitude < 0.0001f) { direction = MyVector2.up; }

            MyVector2 targetPos = prevPos + direction * boneLengths[i];
            jointPositions.Add(collisionHandler.GetValidPosition(prevPos, targetPos));
        }
    }

    private void Forward()
    {
        if (collisionHandler.IsInsideCollider(targetPosition))
        {
            jointPositions[LastIndex] = collisionHandler.FindNearestValidPosition(jointPositions[LastIndex], targetPosition);
        }
        else { jointPositions[LastIndex] = targetPosition; }

        for (int i = LastIndex - 1; i >= 0; i--)
        {
            float boneLength = boneLengths[i];
            MyVector2 direction = (jointPositions[i] - jointPositions[i + 1]).normalized;

            if (direction.sqrMagnitude < 0.0001f) { direction = MyVector2.up; }

            MyVector2 newPos = jointPositions[i + 1] + direction * boneLength;
            jointPositions[i] = collisionHandler.GetValidPosition(jointPositions[i + 1], newPos);
        }
    }

    private void Backward()
    {
        jointPositions[0] = BasePosition;

        for (int i = 1; i < TotalCount; i++)
        {
            float boneLength = boneLengths[i - 1];
            MyVector2 direction = (jointPositions[i] - jointPositions[i - 1]).normalized;

            if (direction.sqrMagnitude < 0.0001f) { direction = MyVector2.up; }

            MyVector2 newPos = jointPositions[i - 1] + direction * boneLength;
            jointPositions[i] = collisionHandler.GetValidPosition(jointPositions[i - 1], newPos);
        }
    }

    private void SyncTransforms()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            MyVector2 pos = jointPositions[i + 1];
            joints[i].position = new(pos.x, pos.y, joints[i].position.z);
        }
    }

    public float GetDistanceToTarget()
    {
        if (!hasTarget) return -1f;
        return MyVector2.Distance(jointPositions[LastIndex], targetPosition);
    }
}