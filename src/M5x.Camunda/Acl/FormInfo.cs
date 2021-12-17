namespace M5x.Camunda.Acl;

public class FormInfo
{
    public string ContextPath;

    /// <summary>
    ///     The form key for the process definition.
    /// </summary>
    public string Key;

    public override string ToString()
    {
        return Key;
    }
}