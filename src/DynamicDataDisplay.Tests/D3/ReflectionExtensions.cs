using System.Reflection;

namespace DynamicDataDisplay.Test
{
	internal static class ReflectionExtensions
	{
		public static T CallPrivateMethod<T>(this object obj, string methodName, params object[] parameters)
		{
			object value = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, parameters);

			return (T)value;
		}
	}
}
