namespace BSN.Resa.Core.Commons
{
    public class PropertyAssignment
    {
        public string PropertyName { get; set; }

        public string Value { get; set; }

        public PropertyAssignment(string propertyName, string value)
        {
            PropertyName = propertyName;
            Value = value;
        }
    }
}
