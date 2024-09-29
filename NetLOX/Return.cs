public class Return(object? value) : Exception() {
    public object? Value {
        get => _value;
    }

    private readonly object? _value = value;
}