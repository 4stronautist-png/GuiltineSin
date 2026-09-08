using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors.Monsters;
using Yggdrasil.Geometry;
using Yggdrasil.Geometry.Shapes;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Skills.Helpers
{
	public static class SkillRangePreviewHelper
	{
		/// <summary>
		/// Draws a debug outline of a skill's damage area. Characters need
		/// the "GuiltineSin.RangePreview" temp flag; non-Companion mobs always show.
		/// Pass an explicit duration when the skill's ShootTime covers the
		/// entire skill rather than a single projectile.
		/// </summary>
		public static void ShowRangePreview(ICombatEntity caster, Skill skill, IShapeF area, TimeSpan? duration = null)
		{
			if (caster is not Character character)
			{
				if (caster is not Mob || caster is Companion)
					return;
			}
			else if (!character.Variables.Temp.GetBool("GuiltineSin.RangePreview"))
			{
				return;
			}

			var effectiveDuration = duration ?? (skill.Data.ShootTime < SkillConstants.MaxShootTimeForPreview
				? skill.Data.ShootTime
				: SkillConstants.DefaultDebugShapeDuration);

			Debug.ShowShape(caster.Map, area, effectiveDuration);
		}

		/// <summary>
		/// Returns a Donut if innerRange > 0, otherwise a CircleF.
		/// </summary>
		public static IShapeF GetPreviewArea(Position position, float range, float innerRange = 0)
		{
			if (innerRange > 0)
				return new Donut(position, range, innerRange);
			return new CircleF(position, range);
		}

		/// <summary>
		/// Same as GetPreviewArea but floors the range to the caster's body
		/// radius, matching SplashDamage's effective hit area.
		/// </summary>
		public static IShapeF GetPreviewArea(ICombatEntity caster, Position position, float range, float innerRange = 0)
		{
			range = Math.Max(range, SizeTypeRadius.GetRadius(caster.EffectiveSize));
			return GetPreviewArea(position, range, innerRange);
		}
	}
}
