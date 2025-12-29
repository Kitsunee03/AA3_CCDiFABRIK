using UnityEngine;

public class IKCollisionHandler
{
    private readonly LayerMask collisionLayer;
    private readonly float jointRadius;

    public IKCollisionHandler(LayerMask p_collisionLayer, float p_jointRadius)
    {
        collisionLayer = p_collisionLayer;
        jointRadius = p_jointRadius;
    }

    public bool CheckSegmentCollision(MyVector2 p_start, MyVector2 p_end, out MyVector2 p_validPosition)
    {
        p_validPosition = p_end;

        MyVector2 direction = p_end - p_start;
        float distance = direction.magnitude;

        if (distance < 0.001f) { return false; }

        RaycastHit2D hit = Physics2D.CircleCast(p_start, jointRadius, direction.normalized, distance, collisionLayer);

        if (hit.collider != null)
        {
            float safeDistance = Mathf.Max(0, hit.distance - jointRadius * 0.5f);
            p_validPosition = p_start + direction.normalized * safeDistance;
            return true;
        }

        if (Physics2D.OverlapCircle(p_end, jointRadius, collisionLayer))
        {
            p_validPosition = FindNearestValidPosition(p_start, p_end);
            return true;
        }

        return false;
    }

    public MyVector2 FindNearestValidPosition(MyVector2 p_start, MyVector2 p_targetEnd)
    {
        MyVector2 direction = (p_targetEnd - p_start).normalized;
        float maxDistance = MyVector2.Distance(p_start, p_targetEnd);

        float low = 0;
        float high = maxDistance;
        MyVector2 bestPos = p_start;

        for (int i = 0; i < 10; i++)
        {
            float mid = (low + high) * 0.5f;
            MyVector2 testPos = p_start + direction * mid;

            if (!Physics2D.OverlapCircle(testPos, jointRadius, collisionLayer))
            {
                bestPos = testPos;
                low = mid;
            }
            else { high = mid; }
        }

        return bestPos;
    }

    public bool HasCollision(MyVector2 p_start, MyVector2 p_end)
    {
        MyVector2 direction = p_end - p_start;
        float distance = direction.magnitude;

        if (distance > 0.001f)
        {
            RaycastHit2D hit = Physics2D.CircleCast(p_start, jointRadius, direction.normalized, distance, collisionLayer);
            if (hit.collider != null) { return true; }
        }

        if (Physics2D.OverlapCircle(p_end, jointRadius, collisionLayer)) { return true; }

        return false;
    }

    public bool IsInsideCollider(MyVector2 p_position)
    {
        return Physics2D.OverlapCircle(p_position, jointRadius, collisionLayer);
    }

    public MyVector2 GetValidPosition(MyVector2 p_prevPos, MyVector2 p_targetPos)
    {
        if (CheckSegmentCollision(p_prevPos, p_targetPos, out MyVector2 validPos)) { return validPos; }
        return p_targetPos;
    }
}
