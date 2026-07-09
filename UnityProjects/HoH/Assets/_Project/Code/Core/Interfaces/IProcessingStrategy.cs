namespace _Project.Code.Core.Interfaces
{
    /// <summary>
    ///     Implemented by each processing strategy (Organic, Inorganic, Solvent).
    ///     ProcessingSystem selects the correct strategy at runtime and calls Execute().
    /// </summary>
    public interface IProcessingStrategy
    {
        void Execute(IProcessable target);
    }
}