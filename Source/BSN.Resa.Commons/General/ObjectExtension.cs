namespace BSN.Resa.Commons
{
	public static class ObjectExtension
	{
		public static string NullableToString(this object obj)
		{
			if (obj is string)
				if ((string)obj == "")
					return "null";
				else
					return (string)obj;
			if (obj == null)
				return "null";
			return obj.ToString();
		}
        //public static bool HasMethod(this object objectToCheck, string methodName)
        //{
        //    var type = objectToCheck.GetType();
        //    return type.GetMethod(methodName) != null;
        //}
        //public static bool HasProperty(this object obj, string propertyName)
        //{
        //    return obj.GetType().GetProperty(propertyName) != null;
        //}
    }
}