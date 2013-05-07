using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;
using RAIN.Path;

public class PatrolFindNearest : Action
{
    public PatrolFindNearest()
    {
        actionName = "PatrolFindNearest";
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		float tMinDistSq = float.MaxValue;
		int tMinPatrol = -1;
		
		int tPatrolCount = agent.actionContext.GetContextItem<int>("patrolcount");
		for (int i = 0; i < tPatrolCount; i++)
		{
			Vector3 tPatrolPos = agent.actionContext.GetContextItem<Vector3>("patrol" + i);
			
			float tDistSq = (tPatrolPos - agent.Avatar.transform.position).sqrMagnitude;
			if (tDistSq < tMinDistSq)
			{
				tMinDistSq = tDistSq;
				tMinPatrol = i;
			}
		}
		
		agent.actionContext.SetContextItem<int>("patroldirection", 1);
		agent.actionContext.SetContextItem<int>("patrolnumber", tMinPatrol);
		agent.actionContext.SetContextItem<Vector3>("patrolposition", agent.actionContext.GetContextItem<Vector3>("patrol" + tMinPatrol));

        return ActionResult.SUCCESS;
    }
}