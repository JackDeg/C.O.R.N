using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace CORN
{
    [StaticConstructorOnStartup]
    public class HediffComp_Dependent : HediffComp
    {   
        public HediffCompProperties_Dependent Props => (HediffCompProperties_Dependent)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            HediffDef dependsOn = Props.dependsOnHediff;
            Hediff hediff = Pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == dependsOn);
            if (hediff == null)
            {
                severityAdjustment -= 0.01f;
            }
        }

        public override bool CompShouldRemove
        {
            get
            {
                if (!base.CompShouldRemove)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
