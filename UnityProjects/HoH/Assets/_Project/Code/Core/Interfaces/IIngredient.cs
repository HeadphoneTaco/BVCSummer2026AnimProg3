namespace _Project.Code.Core.Interfaces
{
    public interface IIngredient
    {
        IngredientData GetData();
        bool IsProcessed();
    }
}