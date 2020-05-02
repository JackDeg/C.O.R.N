using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CORN
{
    [DefOf]
    public static class MyDefOf
    {
        public static ThingDef CultureChamber;
        public static ThingDef CornCultureMedium;
        public static ThingDef MouldyCultureMedium;
        public static JobDef FillCultureChamber;
        public static JobDef TakeCultureOutOfCultureChamber;
        static MyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));
        }
    }
}
