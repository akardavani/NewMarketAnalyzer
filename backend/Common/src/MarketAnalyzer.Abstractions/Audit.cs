using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public class Audit
    {
        public long Id { get; set; }
        public int OperatorId { get; set; }
        public AuditType AuditType { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }

        private List<string> _changedColumns;
        public List<string> ChangedColumns
        {
            get => _changedColumns ??= new List<string>();
            set => _changedColumns = value;
        }

        private Dictionary<string, object> _oldValues;
        public Dictionary<string, object> OldValues
        {
            get => _oldValues ??= new Dictionary<string, object>();
            set => _oldValues = value;
        }

        private Dictionary<string, object> _newValues;
        public Dictionary<string, object> NewValues
        {
            get => _newValues ??= new Dictionary<string, object>();
            set => _newValues = value;
        }

        private Dictionary<string, object> _primaryKeyValues;
        public Dictionary<string, object> PrimaryKeyValues
        {
            get => _primaryKeyValues ??= new Dictionary<string, object>();
            set => _primaryKeyValues = value;
        }
    }
}