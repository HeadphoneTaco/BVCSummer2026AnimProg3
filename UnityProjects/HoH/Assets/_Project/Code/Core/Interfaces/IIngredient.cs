namespace _Project.Code.Core.Interfaces
{
    /// <summary>
    ///     Consumed by ChemistrySystem. Does not expose processing behaviour —
    ///     keep IProcessable separate so the chemistry system has no processing dependency.
    /// </summary>
    public interface IIngredient
    {
        IngredientData GetData();
        bool IsProcessed();
    }
}