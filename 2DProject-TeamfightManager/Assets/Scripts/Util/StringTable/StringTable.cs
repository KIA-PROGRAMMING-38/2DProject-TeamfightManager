using System.Collections.Generic;
using UnityEngine.Experimental.AI;

public static class StringTable
{
	private static DataToStringConverter<int> s_intToStringConverter = new DataToStringConverter<int>();
	private static DataToStringConverter<float> s_floatToStringConverter = new DataToStringConverter<float>();

	public static string GetString(int number)
	{
		return s_intToStringConverter.Convert(number);
	}

	public static string GetString(float number)
	{
		return s_floatToStringConverter.Convert(number);
	}
}

public class DataToStringConverter<T>
{
	private Dictionary<T, string> _container = new Dictionary<T, string>();

	public string Convert(T value)
	{
		string returnValue;
		if( false == _container.TryGetValue(value, out returnValue) )
		{
			returnValue = value.ToString();
			_container.Add(value, returnValue);
		}

		return returnValue;
	}
}