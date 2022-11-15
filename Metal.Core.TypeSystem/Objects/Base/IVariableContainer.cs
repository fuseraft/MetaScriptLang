namespace Metal.Core.TypeSystem.Objects.Base
{
    public interface IVariableContainer
    {
        IDictionary<string, Variable> Variables { get; set; }
    }
}
