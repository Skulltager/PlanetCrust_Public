public class ControlledEventVariable<TSource, TValue> : EventVariable<TSource, TValue>
{
    public override TValue value
    {
        get { return base.value; }
        set { base.value = valueCheck(value); }
    }

    public delegate TValue ValueCheck(TValue value);
    private readonly ValueCheck valueCheck;

    public ControlledEventVariable(TSource source, TValue startValue, ValueCheck valueCheck, bool triggerSameValue = false)
        : base(source, startValue, triggerSameValue)
    {
        this.valueCheck = valueCheck;
    }
}
