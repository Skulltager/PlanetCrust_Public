using System;
using System.Collections.Generic;

public class EventVariable<TSource, TValue>
{
    private static EqualityComparer<TValue> equalityComparer = EqualityComparer<TValue>.Default;

    protected bool triggerSameValue;
    protected readonly TSource source;
    protected TValue _value;
    public TValue eventStackValue { protected set; get; }

    public virtual TValue value
    {
        get { return _value; }
        set
        {
            if(!triggerSameValue)
            {
                if (equalityComparer.Equals(_value, value))
                    return;
            }
            
            _value = value;

            EventVariableProperties<TSource, TValue> eventProperty = new EventVariableProperties<TSource, TValue>(this, source, _value);
            EventVariableManager.instance.AddEvent(eventProperty);
        }
    }

    public EventVariable(TSource source, TValue startValue, bool triggerSameValue = false)
    {
        this.triggerSameValue = triggerSameValue;
        this._value = startValue;
        this.eventStackValue = startValue;
        this.source = source;
    }

    public void ReplaceOnValueChange(EventDelegateSource callback, EventVariable<TSource, TValue> oldEventVariable, bool callImmedietly = true)
    {
        oldEventVariable.onValueChangeSource -= callback;
        onValueChangeSource += callback;

        if(callImmedietly)
            callback(source, oldEventVariable.eventStackValue, eventStackValue);
    }

    public void ReplaceOnValueChange(EventDelegate callback, EventVariable<TSource, TValue> oldEventVariable, bool callImmedietly = true)
    {
        oldEventVariable.onValueChange -= callback;
        onValueChange += callback;

        if (callImmedietly)
            callback(oldEventVariable.eventStackValue, eventStackValue);
    }

    public delegate void EventDelegate(TValue oldValue, TValue newValue);
    public event EventDelegate onValueChangeImmediate
    {
        add { onValueChange += value; value(default(TValue), eventStackValue); }
        remove { value(eventStackValue, default(TValue)); onValueChange -= value; }
    }

    public delegate void EventDelegateSource(TSource source, TValue oldValue, TValue newValue);
    public event EventDelegateSource onValueChangeImmediateSource
    {
        add { onValueChangeSource += value; value(source, default(TValue), eventStackValue); }
        remove { value(source, eventStackValue, default(TValue)); onValueChangeSource -= value; }
    }

    public event EventDelegate onValueChange;
    public event EventDelegateSource onValueChangeSource;

    public virtual void TriggerOnValueChange(TSource source, TValue newValue)
    {
        TValue oldValue = eventStackValue;
        eventStackValue = newValue;

        if (onValueChangeSource != null)
            onValueChangeSource.Invoke(source, oldValue, newValue);

        if (onValueChange != null)
            onValueChange.Invoke(oldValue, newValue);
    }

    public void ClearBinds()
    {
        if (onValueChangeSource != null)
            foreach (Delegate d in onValueChangeSource.GetInvocationList())
                onValueChangeSource -= (EventDelegateSource)d;
    }

    public override string ToString()
    {
        string valueString = value != null ? value.ToString() : "null";
        return string.Format("EventVariable({0})", valueString);
    }
}
