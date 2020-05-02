using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CORN
{
    public class HediffCompProperties_Dependent : HediffCompProperties
    {
		public HediffDef dependsOnHediff;
		public HediffCompProperties_Dependent()
		{
			compClass = typeof(HediffComp_Dependent);
		}
	}
}
