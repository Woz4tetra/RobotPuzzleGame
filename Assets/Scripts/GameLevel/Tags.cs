public class Tags
{
    private Tags(string value) { Value = value; }

    public string Value { get; private set; }

    public static Tags Robot { get { return new Tags("Robot"); } }
    public static Tags Interactive { get { return new Tags("Interactive"); } }

    public override string ToString()
    {
        return Value;
    }
}