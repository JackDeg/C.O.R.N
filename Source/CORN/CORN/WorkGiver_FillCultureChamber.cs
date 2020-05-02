// CORN.WorkGiver_FillCultureChamber
using CORN;
using RimWorld;
using System;
using Verse;
using Verse.AI;

public class WorkGiver_FillCultureChamber : WorkGiver_Scanner
{
	private static string TemperatureTrans;

	private static string NoWortTrans;

	public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MyDefOf.CultureChamber);

	public override PathEndMode PathEndMode => PathEndMode.Touch;

	public static void ResetStaticData()
	{
		TemperatureTrans = "BadTemperature".Translate().ToLower();
		NoWortTrans = "NoWort".Translate();
	}

	public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		Building_CultureChamber building_CultureChamber = t as Building_CultureChamber;
		if (building_CultureChamber == null || building_CultureChamber.Mouldy || building_CultureChamber.SpaceLeftForCultures <= 0)
		{
			return false;
		}
		float ambientTemperature = building_CultureChamber.AmbientTemperature;
		CompProperties_TemperatureRuinable compProperties = building_CultureChamber.def.GetCompProperties<CompProperties_TemperatureRuinable>();
		if (ambientTemperature < compProperties.minSafeTemperature + 2f || ambientTemperature > compProperties.maxSafeTemperature - 2f)
		{
			JobFailReason.Is(TemperatureTrans);
			return false;
		}
		if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
		{
			return false;
		}
		if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
		{
			return false;
		}
		if (FindCulture(pawn, building_CultureChamber) == null)
		{
			JobFailReason.Is(NoWortTrans);
			return false;
		}
		if (t.IsBurning())
		{
			return false;
		}
		return true;
	}

	public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		Building_CultureChamber chamber = (Building_CultureChamber)t;
		Thing t2 = FindCulture(pawn, chamber);
		return JobMaker.MakeJob(MyDefOf.FillCultureChamber, t, t2);
	}

	private Thing FindCulture(Pawn pawn, Building_CultureChamber chamber)
	{
		Predicate<Thing> validator = (Thing x) => (!x.IsForbidden(pawn) && pawn.CanReserve(x)) ? true : false;
		return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(MyDefOf.CornCultureMedium), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator);
	}
}
