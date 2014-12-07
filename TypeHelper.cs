using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


//	TypeHelper.cs
//	Author: Lu Zexi
//	2014-12-07



//type helper
public static class TypeHelper
{
	public static IEnumerable<Type> GetAllTypes()
	{
		return from asm in AppDomain.CurrentDomain.GetAssemblies()
				from type in asm.GetTypes()
				select type;
	}

	//get type by type name in string .
	public static Type GetType(string typeName, bool enforceExisting = false)
	{
		if (typeName == null)
		{
			if (enforceExisting)
			{
				throw new ArgumentNullException("typeName cannot be null.");
			}
			else
			{
				return null;
			}
		}
		if (typeName == "")
		{
			if (enforceExisting)
			{
				throw new ArgumentException("typeName cannot be empty.");
			}
			else
			{
				return null;
			}
		}

		var types = from asm in AppDomain.CurrentDomain.GetAssemblies()
				let type = asm.GetType(typeName)
				where type != null
				select type;

		if (enforceExisting)
		{
			return types.First();
		}
		else
		{
			return types.FirstOrDefault();
		}
	}
}