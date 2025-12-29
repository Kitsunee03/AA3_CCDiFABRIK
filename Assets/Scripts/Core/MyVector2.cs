using UnityEngine;

public class MyVector2
{
    public float x, y;

    public MyVector2(float p_x, float p_y)
    {
        x = p_x;
        y = p_y;
    }

    // Direcciones estáticas para 2D
    public static MyVector2 right { get { return new(1, 0); } }
    public static MyVector2 left { get { return new(-1, 0); } }
    public static MyVector2 up { get { return new(0, 1); } }
    public static MyVector2 down { get { return new(0, -1); } }
    public static MyVector2 zero { get { return new(0, 0); } }
    public static MyVector2 one { get { return new(1, 1); } }

    // Distancia entre dos vectores
    public static float Distance(MyVector2 p_a, MyVector2 p_b)
    {
        MyVector2 diff = p_a - p_b;
        return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
    }

    // Vector normalizado
    public MyVector2 normalized
    {
        get
        {
            float mag = Mathf.Sqrt(x * x + y * y);
            if (mag > 0) { return new(x / mag, y / mag); }

            return new(0, 0);
        }
    }

    // Magnitude (longitud) del vector
    public float magnitude
    {
        get { return Mathf.Sqrt(x * x + y * y); }
    }

    // Magnitude al cuadrado (más eficiente para comparaciones)
    public float sqrMagnitude
    {
        get { return x * x + y * y; }
    }

    // Operaciones básicas de vectores
    public static MyVector2 operator +(MyVector2 p_a, MyVector2 p_b)
    {
        return new(p_a.x + p_b.x, p_a.y + p_b.y);
    }

    public static MyVector2 operator -(MyVector2 p_a, MyVector2 p_b)
    {
        return new(p_a.x - p_b.x, p_a.y - p_b.y);
    }

    public static MyVector2 operator -(MyVector2 p_a)
    {
        return new(-p_a.x, -p_a.y);
    }

    public static MyVector2 operator *(MyVector2 p_a, float p_scalar)
    {
        return new(p_a.x * p_scalar, p_a.y * p_scalar);
    }

    public static MyVector2 operator *(float p_scalar, MyVector2 p_a)
    {
        return new(p_a.x * p_scalar, p_a.y * p_scalar);
    }

    public static MyVector2 operator /(MyVector2 p_a, float p_scalar)
    {
        return new(p_a.x / p_scalar, p_a.y / p_scalar);
    }

    // Producto punto (dot product)
    public static float Dot(MyVector2 p_a, MyVector2 p_b)
    {
        return p_a.x * p_b.x + p_a.y * p_b.y;
    }

    // Cross product en 2D (devuelve un escalar, equivalente al componente Z del cross en 3D)
    public static float Cross(MyVector2 p_a, MyVector2 p_b)
    {
        return p_a.x * p_b.y - p_a.y * p_b.x;
    }

    // Ángulo entre dos vectores en grados
    public static float Angle(MyVector2 p_from, MyVector2 p_to)
    {
        float denominator = Mathf.Sqrt(p_from.sqrMagnitude * p_to.sqrMagnitude);
        if (denominator < 1e-15f) return 0f;

        float dot = Mathf.Clamp(Dot(p_from, p_to) / denominator, -1f, 1f);
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }

    // Ángulo con signo entre dos vectores en grados
    public static float SignedAngle(MyVector2 p_from, MyVector2 p_to)
    {
        float angle = Angle(p_from, p_to);
        float cross = Cross(p_from, p_to);
        return angle * Mathf.Sign(cross);
    }

    // Rotar vector por un ángulo en grados
    public MyVector2 Rotate(float p_angleDegrees)
    {
        float rad = p_angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new MyVector2(x * cos - y * sin, x * sin + y * cos);
    }

    // Perpendicular (rotación de 90 grados en sentido antihorario)
    public MyVector2 Perpendicular()
    {
        return new MyVector2(-y, x);
    }

    // Lerp entre dos vectores
    public static MyVector2 Lerp(MyVector2 p_a, MyVector2 p_b, float t)
    {
        t = Mathf.Clamp01(t);
        return new MyVector2(p_a.x + (p_b.x - p_a.x) * t, p_a.y + (p_b.y - p_a.y) * t);
    }

    // Reflexión de un vector respecto a una normal
    public static MyVector2 Reflect(MyVector2 p_inDirection, MyVector2 p_inNormal)
    {
        float dot = Dot(p_inDirection, p_inNormal);
        return p_inDirection - p_inNormal * 2f * dot;
    }

    // Conversiones implícitas con Vector2 de Unity
    public static implicit operator Vector2(MyVector2 p_v) { return new(p_v.x, p_v.y); }

    public static implicit operator MyVector2(Vector2 p_v) { return new(p_v.x, p_v.y); }

    // Conversión a Vector3 (z = 0)
    public static implicit operator Vector3(MyVector2 p_v) { return new(p_v.x, p_v.y, 0); }

    public override string ToString()
    {
        return $"({x:F2}, {y:F2})";
    }
}