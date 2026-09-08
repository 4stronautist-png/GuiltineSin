using System;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Zone.Scripting.Dialogues;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Characters.Components;
using GuiltineSin.Zone.World.Quests.Objectives;
using Yggdrasil.Logging;

namespace GuiltineSin.Zone.World.Quests.Papaya
{
	/// <summary>
	/// Runs Papaya/IPF-style quest objective interactions on top of GuiltineSin's quest core.
	/// </summary>
	public class PapayaQuestRuntime
	{
		private readonly Character _character;
		private readonly QuestComponent _quests;

		public PapayaQuestRuntime(Character character, QuestComponent quests)
		{
			_character = character;
			_quests = quests;
		}

		public bool RequiresPersonalClickSurface(string dialogName, string mapClassName)
		{
			if (string.IsNullOrWhiteSpace(dialogName) || string.IsNullOrWhiteSpace(mapClassName))
				return false;

			return this.TryFindActiveNoDropCollect(dialogName, mapClassName, out _, out _, out _, out _, out _);
		}

		public async Task<bool> TryHandleObjectiveInteractionAsync(string dialogName, Dialog dialog = null)
		{
			var mapClassName = _character?.Map?.ClassName;
			if (string.IsNullOrWhiteSpace(dialogName) || string.IsNullOrWhiteSpace(mapClassName))
				return false;

			if (this.TryFindActiveNoDropCollect(dialogName, mapClassName, out var quest, out var objective, out var progress, out var sourceIndex, out var targetCount))
				return await this.TryCollectNoDropObjectiveAsync(dialogName, quest, objective, progress, sourceIndex, targetCount, dialog);

			return false;
		}

		private bool TryFindActiveNoDropCollect(string dialogName, string mapClassName, out Quest quest, out CollectItemObjective objective, out QuestProgress progress, out int sourceIndex, out int targetCount)
		{
			quest = null;
			objective = null;
			progress = null;
			sourceIndex = -1;
			targetCount = 0;

			foreach (var candidateQuest in _quests.GetList()
				.Where(a => a.InProgress && a.QuestStaticData?.Objectives != null && a.SessionObjectStaticData?.QuestData?.MapPointGroup != null)
				.OrderByDescending(a => a.Tracked)
				.ThenByDescending(a => string.Equals(a.QuestStaticData?.QuestMode, "MAIN", StringComparison.OrdinalIgnoreCase))
				.ThenBy(a => a.Data.Level)
				.ThenBy(a => a.StartTime))
			{
				foreach (var objectiveData in candidateQuest.QuestStaticData.Objectives)
				{
					if (objectiveData == null ||
						!string.Equals(objectiveData.Type, "Collect", StringComparison.OrdinalIgnoreCase) ||
						!IsNone(objectiveData.DropTarget) ||
						!candidateQuest.TryGetProgress(objectiveData.Ident, out var candidateProgress) ||
						candidateProgress.Done ||
						!candidateProgress.Unlocked ||
						candidateProgress.Objective is not CollectItemObjective collectObjective)
						continue;

					var sources = candidateQuest.SessionObjectStaticData.QuestData.MapPointGroup
						.Where(a => !IsNone(a))
						.Take(Math.Max(1, objectiveData.Count))
						.ToList();
					var collectedCount = Math.Max(0, Math.Min(candidateProgress.Count, sources.Count));

					for (var i = collectedCount; i < sources.Count; i++)
					{
						if (!MapPointGroupReferencesDialog(sources[i], mapClassName, dialogName))
							continue;

						quest = candidateQuest;
						objective = collectObjective;
						progress = candidateProgress;
						sourceIndex = i;
						targetCount = Math.Max(1, objectiveData.Count);
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> TryCollectNoDropObjectiveAsync(string dialogName, Quest quest, CollectItemObjective objective, QuestProgress progress, int sourceIndex, int targetCount, Dialog dialog)
		{
			var inventoryCount = _character.Inventory.CountItem(objective.ItemId);
			if (inventoryCount > progress.Count)
				this.SyncCollectProgress(quest, objective.ItemId);

			if (progress.Done || _character.Inventory.CountItem(objective.ItemId) >= objective.TargetCount)
			{
				this.RefreshAfterInteraction();
				return true;
			}

			var collectionKey = $"Clover.PapayaRuntime.Collect.{quest.Data.Id.Value}.{progress.Objective.Ident}.{dialogName}.{sourceIndex}";
			if (_character.Variables.Temp.GetBool(collectionKey, false))
			{
				this.RefreshAfterInteraction();
				return true;
			}

			if (dialog != null)
			{
				var result = await dialog.TimeAction("Collecting", "COLLECT", TimeSpan.FromSeconds(1.2));
				if (result != TimeActionResult.Completed)
				{
					Log.Info("Papaya runtime: collection '{0}' for quest '{1}' by '{2}' ended with {3}.", dialogName, quest.QuestStaticData?.ClassName ?? quest.Data.Id.ToString(), _character.Name, result);
					return false;
				}
			}

			_character.Variables.Temp.SetBool(collectionKey, true);
			_character.AddItem(objective.ItemId, 1, dialogName);
			this.SyncCollectProgress(quest, objective.ItemId);
			this.RefreshAfterInteraction();
			Log.Info(
				"Papaya runtime: collected no-drop objective '{0}' for quest '{1}' by '{2}' ({3}/{4}).",
				dialogName,
				quest.QuestStaticData?.ClassName ?? quest.Data.Id.ToString(),
				_character.Name,
				Math.Min(targetCount, _character.Inventory.CountItem(objective.ItemId)),
				targetCount);
			return true;
		}

		private void SyncCollectProgress(Quest questToSync, int itemId)
		{
			_quests.UpdateObjectives<CollectItemObjective>((quest, objective, progress) =>
			{
				if (quest.Data.Id != questToSync.Data.Id || objective.ItemId != itemId)
					return;

				progress.Count = Math.Min(objective.TargetCount, _character.Inventory.CountItem(objective.ItemId));
				progress.Done = progress.Count >= objective.TargetCount;
			});
		}

		private void RefreshAfterInteraction()
		{
			_quests.UpdateClient();
			_quests.SyncStaticQuestNpcStates();
			_character.RestoreCoreHudState(true, true);
		}

		private static bool MapPointGroupReferencesDialog(string mapPointGroup, string mapClassName, string dialogName)
		{
			if (string.IsNullOrWhiteSpace(mapPointGroup) || string.IsNullOrWhiteSpace(mapClassName) || string.IsNullOrWhiteSpace(dialogName))
				return false;

			var parts = mapPointGroup.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			for (var i = 0; i + 1 < parts.Length; i++)
			{
				if (string.Equals(parts[i], mapClassName, StringComparison.OrdinalIgnoreCase) &&
					string.Equals(parts[i + 1], dialogName, StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}

		private static bool IsNone(string value)
			=> string.IsNullOrWhiteSpace(value) || string.Equals(value, "None", StringComparison.OrdinalIgnoreCase);
	}
}
