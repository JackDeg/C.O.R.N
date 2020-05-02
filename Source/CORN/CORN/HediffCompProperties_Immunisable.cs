using Verse;

public class HediffCompProperties_Immunisable : HediffCompProperties
{
	public float immunityPerDayNotSick;

	public float immunityPerDaySick;

	public float severityPerDayNotImmune;

	public float severityPerDayImmune;

	public FloatRange severityPerDayNotImmuneRandomFactor = new FloatRange(1f, 1f);

	public HediffCompProperties_Immunisable()
	{
		compClass = typeof(HediffComp_Immunisable);
	}
}
