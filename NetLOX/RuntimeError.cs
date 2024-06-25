class RunTimeError : Exception {
    public readonly Token token;

    public RunTimeError(Token token, string message) : base(message) {
        this.token = token;
    }
}