using System.Collections.Generic;

namespace System.Linq
{
    public class FilterRule
    {
        /// <summary>
        /// The name of the field that the filter applies to.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The condition type of <see cref="ConditionType"/>
        /// </summary>
        public ConditionType Condition { get; set; }

        /// <summary>
        /// The filter operator of <see cref="FilterOperator"/>
        /// </summary>
        public FilterOperator Operator { get; set; } = FilterOperator.Eq;

        /// <summary>
        /// Gets or sets nested filter rules.
        /// </summary>
        public List<FilterRule> Rules { get; set; }

        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Apply not operator to rule.
        /// </summary>
        public bool Not { get; set; }
    }
}
