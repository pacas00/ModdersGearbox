using System;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
    public class UtilClass
    {
        #region  Fields and Properties

        public const string modId = "petercashel.ModdersGearbox";

        public static string modName = "Modder's Gearbox";

        #endregion

        #region  Methods

        public static void WriteLine(string s)
        {
            Debug.LogWarning(modName + ": " + s);
        }

        public static void WriteLine(int x)
        {
            Debug.LogWarning(modName + ": " + x);
        }

        public static void WriteLine(float f)
        {
            Debug.LogWarning(modName + ": " + f);
        }

        public static void WriteLine(bool b)
        {
            Debug.LogWarning(modName + ": " + b);
        }

        internal static void WriteLine(Exception ex)
        {
            Debug.LogError(modName + ": " + ex.Message);
            Debug.LogError(ex.StackTrace);
        }

        #endregion
    }
}