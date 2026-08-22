namespace _Project.Code.Core.Interfaces
{
   
    public interface IProcessingStrategy
    {
        void Execute(IProcessable target);
    }
}