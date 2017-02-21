using System;

namespace Ywdsoft.Utility.DB
{
    internal static class ConvertExt
    {
        internal static object Convert(this object value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType == typeof(string))
                return value.ToString();

            Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (value.GetType() == type)
            {
                return value;
            }

            if (type == typeof(Guid) && value.GetType() == typeof(string))
            {
                return new Guid(value.ToString());
            }
            if (type == typeof(DateTime))
            {
                DateTime result;
                if (DateTime.TryParse(value.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            return System.Convert.ChangeType(value, type);
        }
    }
}
