using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class ValidationModel
    {
        public bool HasError => _errors.Any();
        public IReadOnlyDictionary<string, string[]> Errors => _errors.ToDictionary(p => p.Key, p => p.Value.ToArray());

        public ValidationModel()
        {
            _errors = new Dictionary<string, List<string>>();
        }

        public void AddError(string key, string message)
        {
            if (!_errors.ContainsKey(key))
            {
                _errors.Add(key, new List<string>() { message });
            }
            else
            {
                _errors[key].Add(message);
            }
        }

        public override string ToString()
        {
            IEnumerable<string> errors = _errors.Select(p => $"{p.Key}: [{string.Join(",", p.Value)}]");

            return string.Join(",\r\n", errors);
        }

        private Dictionary<string, List<string>> _errors;
    }
}
