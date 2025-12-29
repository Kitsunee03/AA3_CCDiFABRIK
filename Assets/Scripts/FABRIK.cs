using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{
    [Header("Joints (EndEffector es el último)")]
    [SerializeField] private List<Transform> joints = new();

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Algorithm Settings")]
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private int maxIterationsPerFrame = 10;

    private List<MyVector2> jointPositions;
    private List<float> boneLengths;

    // +1 because we include the base (this.transform)
    private int TotalCount => joints.Count + 1;
    private int LastIndex => TotalCount - 1;
    private MyVector2 BasePosition => new(transform.position.x, transform.position.y);

    private void Awake() { InitializeBoneLengths(); }

    private void LateUpdate()
    {
        if (joints.Count < 1 || target == null) { return; }

        UpdateJointPositionsFromBase();

        MyVector2 targetPos = new(target.position.x, target.position.y);
        float distanceToTarget = MyVector2.Distance(jointPositions[LastIndex], targetPos);

        if (distanceToTarget > tolerance)
        {
            for (int i = 0; i < maxIterationsPerFrame; i++)
            {
                Forward();
                Backward();

                // Check for convergence
                targetPos = new(target.position.x, target.position.y);
                if (MyVector2.Distance(jointPositions[LastIndex], targetPos) <= tolerance) { break; }
            }

            SyncTransforms();
        }
    }

    private void InitializeBoneLengths()
    {
        boneLengths = new();

        // first bone: from base to first joint
        if (joints.Count > 0)
        {
            MyVector2 basePos = new(transform.position.x, transform.position.y);
            MyVector2 firstJoint = new(joints[0].position.x, joints[0].position.y);
            boneLengths.Add(MyVector2.Distance(basePos, firstJoint));
        }

        // bone lengths between joints
        for (int i = 0; i < joints.Count - 1; i++)
        {
            MyVector2 joint1 = new(joints[i].position.x, joints[i].position.y);
            MyVector2 joint2 = new(joints[i + 1].position.x, joints[i + 1].position.y);
            float length = MyVector2.Distance(joint1, joint2);
            boneLengths.Add(length);
        }
    }

    private void UpdateJointPositionsFromBase()
    {
        jointPositions = new() { BasePosition };

        // Add current positions of the joints
        for (int i = 0; i < joints.Count; i++)
        {
            jointPositions.Add(new(joints[i].position.x, joints[i].position.y));
        }
    }

    private void Forward()
    {
        // move the EndEffector to the target
        jointPositions[LastIndex] = new(target.position.x, target.position.y);

        // iterate backward (from the penultimate to the base, without moving it)
        for (int i = LastIndex - 1; i >= 0; i--)
        {
            float boneLength = boneLengths[i];
            MyVector2 direction = (jointPositions[i] - jointPositions[i + 1]).normalized;

            // if the direction is zero, keep a default direction
            if (direction.sqrMagnitude < 0.0001f) { direction = MyVector2.up; }

            jointPositions[i] = jointPositions[i + 1] + direction * boneLength;
        }
    }

    private void Backward()
    {
        // fix the base position
        jointPositions[0] = BasePosition;

        // iterate forward from the base to the end effector
        for (int i = 1; i < TotalCount; i++)
        {
            float boneLength = boneLengths[i - 1];
            MyVector2 direction = (jointPositions[i] - jointPositions[i - 1]).normalized;

            // if the direction is zero, keep a default direction
            if (direction.sqrMagnitude < 0.0001f) { direction = MyVector2.up; }

            jointPositions[i] = jointPositions[i - 1] + direction * boneLength;
        }
    }

    private void SyncTransforms()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            MyVector2 pos = jointPositions[i + 1];
            joints[i].position = new Vector3(pos.x, pos.y, joints[i].position.z);
        }
    }
}