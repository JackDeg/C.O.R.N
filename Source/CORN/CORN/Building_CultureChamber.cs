// CORN.Building_CultureChamber
using CORN;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

[StaticConstructorOnStartup]
public class Building_CultureChamber : Building
{
	private int cultureCount;

	private float progressInt;

	private Material barFilledCachedMat;

	public const int MaxCapacity = 5;

	private const int BaseFermentationDuration = 360000;

	public const float MinIdealTemperature = 21f;

	private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);

	private static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);

	private static readonly Color BarFermentedColor = new Color(0.9f, 0.85f, 0.2f);

	private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

	public float Progress
	{
		get
		{
			return progressInt;
		}
		set
		{
			if (value != progressInt)
			{
				progressInt = value;
				barFilledCachedMat = null;
			}
		}
	}

	private Material BarFilledMat
	{
		get
		{
			if (barFilledCachedMat == null)
			{
				barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(BarZeroProgressColor, BarFermentedColor, Progress));
			}
			return barFilledCachedMat;
		}
	}

	public int SpaceLeftForCultures
	{
		get
		{
			if (Mouldy)
			{
				return 0;
			}
			return 5 - cultureCount;
		}
	}

	private bool Empty => cultureCount <= 0;

	public bool Mouldy
	{
		get
		{
			if (!Empty)
			{
				return Progress >= 1f;
			}
			return false;
		}
	}

	private float CurrentTempProgressSpeedFactor
	{
		get
		{
			CompProperties_TemperatureRuinable compProperties = def.GetCompProperties<CompProperties_TemperatureRuinable>();
			float ambientTemperature = base.AmbientTemperature;
			if (ambientTemperature < compProperties.minSafeTemperature)
			{
				return 0.1f;
			}
			if (ambientTemperature < 7f)
			{
				return GenMath.LerpDouble(compProperties.minSafeTemperature, 7f, 0.1f, 1f, ambientTemperature);
			}
			return 1f;
		}
	}

	private float ProgressPerTickAtCurrentTemp => 2.77777781E-06f * CurrentTempProgressSpeedFactor;

	private int EstimatedTicksLeft => Mathf.Max(Mathf.RoundToInt((1f - Progress) / ProgressPerTickAtCurrentTemp), 0);

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref cultureCount, "cultureCount", 0);
		Scribe_Values.Look(ref progressInt, "progress", 0f);
	}

	public override void TickRare()
	{
		base.TickRare();
		if (!Empty)
		{
			Progress = Mathf.Min(Progress + 250f * ProgressPerTickAtCurrentTemp, 1f);
		}
	}

	public void AddCulture(int count)
	{
		GetComp<CompTemperatureRuinable>().Reset();
		if (Mouldy)
		{
			Logr.Warn("Tried to add cultures to a chamber full of them. Colonists should take the mouldy ones first.");
			return;
		}
		int num = Mathf.Min(count, 5 - cultureCount);
		if (num > 0)
		{
			Progress = GenMath.WeightedAverage(0f, num, Progress, cultureCount);
			cultureCount += num;
		}
	}

	protected override void ReceiveCompSignal(string signal)
	{
		if (signal == "RuinedByTemperature")
		{
			Reset();
		}
	}

	private void Reset()
	{
		cultureCount = 0;
		Progress = 0f;
	}

	public void AddCulture(Thing CornCultureMedium)
	{
		int num = Mathf.Min(CornCultureMedium.stackCount, 5 - cultureCount);
		if (num > 0)
		{
			AddCulture(num);
			CornCultureMedium.SplitOff(num).Destroy();
		}
	}

	public override string GetInspectString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(base.GetInspectString());
		if (stringBuilder.Length != 0)
		{
			stringBuilder.AppendLine();
		}
		CompTemperatureRuinable comp = GetComp<CompTemperatureRuinable>();
		if (!Empty && !comp.Ruined)
		{
			if(Mouldy)
			{
				stringBuilder.AppendLine("ContainsBeer".Translate(cultureCount, 5));
			}
			else
			{
				stringBuilder.AppendLine("ContainsWort".Translate(cultureCount, 5));
			}
		}
		if (!Empty)
		{
			if (Mouldy)
			{
				stringBuilder.AppendLine("Fermented".Translate());
			}
			else
			{
				stringBuilder.AppendLine("FermentationProgress".Translate(Progress.ToStringPercent(), EstimatedTicksLeft.ToStringTicksToPeriod()));
				if (CurrentTempProgressSpeedFactor != 1f)
				{
					stringBuilder.AppendLine("FermentationBarrelOutOfIdealTemperature".Translate(CurrentTempProgressSpeedFactor.ToStringPercent()));
				}
			}
		}
		stringBuilder.AppendLine("Temperature".Translate() + ": " + base.AmbientTemperature.ToStringTemperature("F0"));
		stringBuilder.AppendLine("IdealFermentingTemperature".Translate() + ": " + 7f.ToStringTemperature("F0") + " ~ " + comp.Props.maxSafeTemperature.ToStringTemperature("F0"));
		return stringBuilder.ToString().TrimEndNewlines();
	}

	public Thing TakeOutCulture()
	{
		if (!Mouldy)
		{
			Logr.Warn("Tried to get culture but it's not yet mouldy.");
			return null;
		}
		Thing thing = ThingMaker.MakeThing(MyDefOf.MouldyCultureMedium);
		thing.stackCount = cultureCount;
		Reset();
		return thing;
	}

	public override void Draw()
	{
		base.Draw();
		if (!Empty)
		{
			Vector3 drawPos = DrawPos;
			drawPos.y += 0.0454545468f;
			drawPos.z += 0.25f;
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			r.center = drawPos;
			r.size = BarSize;
			r.fillPercent = (float)cultureCount / 25f;
			r.filledMat = BarFilledMat;
			r.unfilledMat = BarUnfilledMat;
			r.margin = 0.1f;
			r.rotation = Rot4.North;
			GenDraw.DrawFillableBar(r);
		}
	}

	public override IEnumerable<Gizmo> GetGizmos()
	{
		foreach (Gizmo gizmo in base.GetGizmos())
		{
			yield return gizmo;
		}
		if (Prefs.DevMode && !Empty)
		{
			Command_Action command_Action = new Command_Action();
			command_Action.defaultLabel = "Debug: Set progress to 1";
			command_Action.action = delegate
			{
				Progress = 1f;
			};
			yield return command_Action;
		}
	}
}
