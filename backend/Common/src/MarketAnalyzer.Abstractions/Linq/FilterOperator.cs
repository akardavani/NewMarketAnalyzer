namespace System.Linq
{
    public enum FilterOperator
    {
        /// <summary>
        /// Equals
        /// </summary>
        Eq = 1,

        /// <summary>
        /// Not Equals
        /// </summary>
        Neq,

        /// <summary>
        /// Greater than
        /// </summary>
        Gt,

        /// <summary>
        /// Less than
        /// </summary>
        Lt,

        /// <summary>
        /// Less than or equal to
        /// </summary>
        Lte,

        /// <summary>
        /// Greater than or equal to
        /// </summary>
        Gte,

        /// <summary>
        /// Between
        /// </summary>
        Between,

        /// <summary>
        /// In
        /// </summary>
        In,

        /// <summary>
        /// Contain (only for string)
        /// </summary>
        Contains,

        /// <summary>
        /// Starts with (only for string)
        /// </summary>
        StartsWith,

        /// <summary>
        /// Ends with (only for string)
        /// </summary>
        EndsWith,

        /// <summary>
        /// Is null
        /// </summary>
        IsNull,

        /// <summary>
        /// Is not null
        /// </summary>
        IsNotNull
    }
}
