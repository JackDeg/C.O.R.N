using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CORN
{
     class Logr
    {
        public static void Info(string msg)
        {
            Log.Message($"[C.O.R.N]: [INFO]: {msg}");
        }
        public static void Warn(string msg)
        {
            Log.Warning($"[C.O.R.N]: [WARN]: {msg}");
        }
        public static void Severe(string msg)
        {
            Log.Error($"[C.O.R.N]: [SEVERE]: {msg}");
        }
    }
}
