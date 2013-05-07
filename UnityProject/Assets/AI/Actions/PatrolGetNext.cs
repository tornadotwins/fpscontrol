using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class PatrolGetNext : Action
{
    public PatrolGetNext()
    {
        actionName = "PatrolGetNext";
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		int tPatrolDirection = agent.actionContext.GetContextItem<int>("patroldirection");
        int tPatrolNumber = agent.actionContext.GetContextItem<int>("patrolnumber");
		int tPatrolCount = agent.actionContext.GetContextItem<int>("patrolcount");
		
        tPatrolNumber += tPatrolDirection;
		if (tPatrolNumber < 0 || tPatrolNumber >= tPatrolCount)
			return ActionResult.FAILURE;
		
        agent.actionContext.SetContextItem<int>("patrolnumber", tPatrolNumber);
        agent.actionContext.SetContextItem<Vector3>("patrolposition", agent.actionContext.GetContextItem<Vector3>("patrol" + tPatrolNumber));
		
		return ActionResult.SUCCESS;
    }
}