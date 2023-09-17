public class Layers
{
    private Layers(string value) { Value = value; }

    public string Value { get; private set; }

    public static Layers Default { get { return new Layers("Default"); } }
    public static Layers Robot { get { return new Layers("Robot"); } }
    public static Layers Interactable { get { return new Layers("Interactable"); } }
    public static Layers Ground { get { return new Layers("Ground"); } }
    public static Layers Switch { get { return new Layers("Switch"); } }

    public override string ToString()
    {
        return Value;
    }
}