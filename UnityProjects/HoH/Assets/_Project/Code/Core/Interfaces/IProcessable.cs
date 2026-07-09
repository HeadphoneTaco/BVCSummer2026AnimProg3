namespace _Project.Code.Core.Interfaces
{
    /// <summary>
    ///     Consumed by ProcessingSystem and processing strategies only.
    ///     Kept separate from IIngredient — the chemistry system never needs this.
    /// </summary>
    public interface IProcessable
    {
        void Process();
    }
}