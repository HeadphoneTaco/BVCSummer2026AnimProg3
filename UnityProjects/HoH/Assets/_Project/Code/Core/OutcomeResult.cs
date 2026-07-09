using _Project.Code.Core.Enums;

namespace _Project.Code.Core
{
    /// <summary>
    ///     Immutable data object carrying the result of a chemistry combination.
    ///     Passed as the payload of ChemistrySystem.OnCombinationResolved.
    /// </summary>
    public class OutcomeResult
    {
        public OutcomeResult(OutcomeType outcomeType, string resultName)
        {
            OutcomeType = outcomeType;
            ResultName = resultName;
        }

        public OutcomeType OutcomeType { get; }
        public string ResultName { get; }

        public override string ToString()
        {
            return $"[{OutcomeType}] {ResultName}";
        }
    }
}