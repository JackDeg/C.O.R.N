// CORN.WorkGiver_TakeCultureOutOfCultureChamber
using CORN;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

public class WorkGiver_TakeCultureOutOfCultureChamber : WorkGiver_Scanner
{
	public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MyDefOf.CultureChamber);

	public override PathEndMode PathEndMode => PathEndMode.Touch;

	public override bool ShouldSkip(Pawn pawn, bool forced = false)
	{
		List<Thing> list = pawn.Map.listerThings.ThingsOfDef(MyDefOf.CultureChamber);
		for (int i = 0; i < list.Count; i++)
		{
			if (((Building_CultureChamber)list[i]).Mouldy)
			{
				return false;
			}
		}
		return true;
	}

	public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		Building_CultureChamber building_CultureChamber = t as Building_CultureChamber;
		if (building_CultureChamber == null || !building_CultureChamber.Mouldy)
		{
			return false;
		}
		if (t.IsBurning())
		{
			return false;
		}
		if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
		{
			return false;
		}
		return true;
	}

	public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		return JobMaker.MakeJob(MyDefOf.TakeCultureOutOfCultureChamber, t);
	}
}
