// CORN.JobDriver_FillCultureChamber
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

public class JobDriver_FillCultureChamber : JobDriver
{
	private const TargetIndex ChamberInd = TargetIndex.A;

	private const TargetIndex CultureInd = TargetIndex.B;

	private const int Duration = 200;

	protected Building_CultureChamber Chamber => (Building_CultureChamber)job.GetTarget(TargetIndex.A).Thing;

	protected Thing Culture => job.GetTarget(TargetIndex.B).Thing;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		if (pawn.Reserve(Chamber, job, 1, -1, null, errorOnFailed))
		{
			return pawn.Reserve(Culture, job, 1, -1, null, errorOnFailed);
		}
		return false;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
		this.FailOnBurningImmobile(TargetIndex.A);
		AddEndCondition(() => (Chamber.SpaceLeftForCultures > 0) ? JobCondition.Ongoing : JobCondition.Succeeded);
		yield return Toils_General.DoAtomic(delegate
		{
			job.count = Chamber.SpaceLeftForCultures;
		});
		Toil reserveCulture = Toils_Reserve.Reserve(TargetIndex.B);
		yield return reserveCulture;
		yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
		yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
		yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveCulture, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
		yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
			.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
			.WithProgressBarToilDelay(TargetIndex.A);
		Toil toil = new Toil();
		toil.initAction = delegate
		{
			Chamber.AddCulture(Culture);
		};
		toil.defaultCompleteMode = ToilCompleteMode.Instant;
		yield return toil;
	}
}
