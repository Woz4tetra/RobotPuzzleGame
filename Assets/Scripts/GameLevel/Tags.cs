public class Tags
{
    private Tags(string value) { Value = value; }

    public string Value { get; private set; }

    public static Tags Robot { get { return new Tags("Robot"); } }
    public static Tags Interactive { get { return new Tags("Interactive"); } }
    public static Tags DialogTrigger { get { return new Tags("DialogTrigger"); } }

    public override string ToString()
    {
        return Value;
    }
}