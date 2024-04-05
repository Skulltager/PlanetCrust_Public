
using UnityEngine;

public class TransformCalculationHelper : MonoBehaviour
{
    public static TransformCalculationHelper instance { private set; get; }

    [SerializeField] private new Transform transform = default;
    public Transform Transform => transform;

    public void Initialize()
    {
        instance = this;
    }
}