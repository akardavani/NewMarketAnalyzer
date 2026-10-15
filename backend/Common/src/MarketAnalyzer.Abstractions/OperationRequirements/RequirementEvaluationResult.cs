namespace MarketAnalyzer.Abstractions
{
    public class RequirementEvaluationResult
    {
        private RequirementEvaluationResult()
        {

        }

        /// <summary>
        /// موفق/ناموفق
        /// </summary>
        public bool Succeeded { get; private set; }
        /// <summary>
        /// شرح خطا
        /// </summary>
        public string Error { get; private set; }

        public static RequirementEvaluationResult Success()
        {
            return new RequirementEvaluationResult() { Succeeded = true };
        }

        public static RequirementEvaluationResult Fail(string error)
        {
            return new RequirementEvaluationResult()
            {
                Succeeded = false,
                Error = error
            };
        }
    }
}
