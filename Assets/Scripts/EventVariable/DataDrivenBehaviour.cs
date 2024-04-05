using UnityEngine;

public abstract class DataDrivenBehaviour<T> : MonoBehaviour
{
    private readonly EventVariable<DataDrivenBehaviour<T>, T> _data;
    public T data
    {
        set { _data.value = value; }
        get { return _data.eventStackValue; }
    }

    protected DataDrivenBehaviour()
    {
        _data = new EventVariable<DataDrivenBehaviour<T>, T>(this, default(T));
    }

    private void OnEnable()
    {
        _data.onValueChangeImmediate += OnValueChanged_Data;
        OnEnableSub();
    }

    private void OnDisable()
    {
        _data.onValueChangeImmediate -= OnValueChanged_Data;
        OnDisableSub();
    }

    protected virtual void OnEnableSub() { }
    protected virtual void OnDisableSub() { }

    protected abstract void OnValueChanged_Data(T oldValue, T newValue);
}