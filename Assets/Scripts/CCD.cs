using System.Collections.Generic;
using UnityEngine;

public class CCD : MonoBehaviour
{
    [Header("Joints (EndEffector es el último)")]
    [SerializeField] private List<Transform> joints = new();

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Algorithm Settings")]
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private int maxIterationsPerFrame = 10;

    private List<Vector3> jointPositions;
    private List<float> boneLengths;

    // +1 porque incluimos la base (this.transform)
    private int TotalCount => joints.Count + 1;
    private int LastIndex => TotalCount - 1;
    private Transform EndEffector => joints[joints.Count - 1];
    private Vector3 BasePosition => transform.position;

    private void Awake() { InitializeBoneLengths(); }

    private void LateUpdate()
    {
        if (joints.Count < 1 || target == null) { return; }

        UpdateJointPositionsFromBase();

        float distanceToTarget = Vector3.Distance(EndEffector.position, target.position);

        if (distanceToTarget > tolerance)
        {
            for (int i = 0; i < maxIterationsPerFrame; i++)
            {
                PerformCCDIteration();

                // check for convergence
                if (Vector3.Distance(EndEffector.position, target.position) <= tolerance) { break; }
            }
        }
    }

    private void InitializeBoneLengths()
    {
        boneLengths = new List<float>();

        // Primer hueso: de la base al primer joint
        if (joints.Count > 0)
        {
            boneLengths.Add(Vector3.Distance(transform.position, joints[0].position));
        }

        // Resto de huesos entre joints
        for (int i = 0; i < joints.Count - 1; i++)
        {
            float length = Vector3.Distance(joints[i].position, joints[i + 1].position);
            boneLengths.Add(length);
        }
    }

    private void UpdateJointPositionsFromBase()
    {
        jointPositions = new() { BasePosition };

        // Recalcular posiciones manteniendo longitudes de huesos
        for (int i = 0; i < joints.Count; i++)
        {
            Vector3 prevPos = jointPositions[i];
            Vector3 currentPos = joints[i].position;
            Vector3 direction = (currentPos - prevPos).normalized;

            // Si la dirección es cero, mantener dirección anterior
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = Vector3.up;
            }

            jointPositions.Add(prevPos + direction * boneLengths[i]);
        }

        SyncTransforms();
    }

    private void PerformCCDIteration()
    {
        // Iterar desde el penúltimo hasta la base (incluida, para rotar desde ella)
        for (int i = LastIndex - 1; i >= 0; i--)
        {
            Vector3 currentJoint = jointPositions[i];

            // Calcular direcciones
            Vector3 toEnd = (jointPositions[LastIndex] - currentJoint).normalized;
            Vector3 toTarget = (target.position - currentJoint).normalized;

            // Calcular rotación
            float dot = Mathf.Clamp(Vector3.Dot(toEnd, toTarget), -1f, 1f);
            float angle = Mathf.Acos(dot);
            Vector3 axis = Vector3.Cross(toEnd, toTarget);

            if (axis.sqrMagnitude > 0.0001f)
            {
                axis.Normalize();
                Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);

                // Rotar todos los joints posteriores (no la base en sí)
                for (int j = i + 1; j < TotalCount; j++)
                {
                    Vector3 offset = jointPositions[j] - jointPositions[i];
                    jointPositions[j] = jointPositions[i] + rotation * offset;
                }
            }
        }

        SyncTransforms();
    }

    private void SyncTransforms()
    {
        // Sincronizar solo los joints (no la base)
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].position = jointPositions[i + 1];
        }
    }
}