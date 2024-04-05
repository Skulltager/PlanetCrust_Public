public class EventVariableProperties<TSource, TValue> : EventVariableBase
{
    public EventVariable<TSource, TValue> eventVariable;
    public TSource source;
    public TValue newValue;

    public EventVariableProperties(EventVariable<TSource, TValue> eventVariable, TSource source, TValue newValue)
    {
        this.eventVariable = eventVariable;
        this.source = source;
        this.newValue = newValue;
    }

    public override void Trigger()
    {
        eventVariable.TriggerOnValueChange(source, newValue);
    }
}