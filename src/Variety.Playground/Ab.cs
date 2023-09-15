namespace @Variety
{
    abstract partial record Abc
    {
        public abstract TVisitorResult Accept<TVisitorResult>(Visitor<TVisitorResult> visitor);

        partial record B : global::@Variety.@Abc
        {
            public override TVisitorResult Accept<TVisitorResult>(Visitor<TVisitorResult> visitor) { return visitor.Visit(this); }
        }
        public abstract class Visitor<TVisitorResult>
        {
            public abstract TVisitorResult Visit(global::@Variety.@Abc.@B b);
        }
    }
}