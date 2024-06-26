using System;
using System.Collections.Generic;
using System.Linq;

namespace Memoria.Speedrun
{
	public class Split
	{
		public String Name;

		public void AddFieldTransitionCondition(Int32 fromField, Int32 toField, Int32 scenarioCondition = -1)
		{
			conditions.Add(new TriggerCondition()
			{
				triggerType = TriggerType.FIELD_TRANSITION,
				previousId = fromField,
				nextId = toField,
				scenarioCounter = scenarioCondition
			});
		}

		public void AddBattleCondition(TriggerType trigger, Int32 battleId, Int32 scenarioCondition = -1)
		{
			conditions.Add(new TriggerCondition()
			{
				triggerType = trigger,
				previousId = battleId,
				nextId = battleId,
				scenarioCounter = scenarioCondition
			});
		}

		public Boolean IsConditionFulfilled(TriggerType trigger)
		{
			return conditions.Any(c => c.triggerType == trigger && c.IsConditionFulfilled());
		}

		private List<TriggerCondition> conditions = new List<TriggerCondition>();

		private class TriggerCondition
		{
			public TriggerType triggerType;
			public Int32 previousId;
			public Int32 nextId;
			public Int32 scenarioCounter;

			public Boolean IsConditionFulfilled()
			{
				if (scenarioCounter >= 0 && FF9StateSystem.EventState.ScenarioCounter != scenarioCounter)
					return false;
				switch (triggerType)
				{
					case TriggerType.FIELD_TRANSITION:
						return (previousId < 0 || previousId == FF9StateSystem.Common.FF9.previousFldWldMapNo) && (nextId < 0 || nextId == FF9StateSystem.Common.FF9.currentFldWldMapNo);
					case TriggerType.BATTLE_WIN:
					case TriggerType.BATTLE_STOP:
					case TriggerType.BATTLE_END:
						return nextId < 0 || nextId == FF9StateSystem.Common.FF9.btlMapNo;
				}
				return false;
			}
		}

		public enum TriggerType
		{
			FIELD_TRANSITION,
			BATTLE_WIN,
			BATTLE_STOP,
			BATTLE_END
		}
	}
}
