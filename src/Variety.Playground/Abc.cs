namespace Variety.Playground;

[Vary]
public partial record Abc<T> { public sealed partial record B; }

public sealed class D<T> : Abc<T>.Visitor<string>
{
    public override string Visit(Abc<T>.B b)
    {
        throw new NotImplementedException();
    }
}