using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CORN
{
    [StaticConstructorOnStartup]
    public static class CornMod
    {
        static CornMod()
        {
            Logr.Info("C.O.R.N loaded.");
        }
    }
}
