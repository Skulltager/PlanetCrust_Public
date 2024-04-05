using UnityEngine;

public abstract class DataDrivenUI<T> : DataDrivenBehaviour<T>
{
    public Canvas canvas => GetComponentInParent<Canvas>();
    public RectTransform rectTransform => transform as RectTransform;
}