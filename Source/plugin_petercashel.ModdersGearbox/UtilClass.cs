using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
	public class UtilClass
	{
	    public static string modName = "Modder's Gearbox";
	    public const string modId = "petercashel.ModdersGearbox";

        public static void WriteLine(string s)
		{
            Debug.LogWarning(UtilClass.modName + ": " + s);
		}

		public static void WriteLine(int x)
		{
            Debug.LogWarning(UtilClass.modName + ": " + x);
		}

		public static void WriteLine(float f)
		{
            Debug.LogWarning(UtilClass.modName + ": " + f);
		}

		public static void WriteLine(bool b)
		{
            Debug.LogWarning(UtilClass.modName + ": " + b);
		}

		internal static void WriteLine(Exception ex)
		{
            Debug.LogError(UtilClass.modName + ": " + ex.Message);
            Debug.LogError(ex.StackTrace);
		}

	}
}
