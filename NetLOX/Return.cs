public class Return : Exception {
    public Return(object value) : base(){
        this._value = value;
    }

    public object Value {
        get => _value;
    }

    private readonly object _value;
}