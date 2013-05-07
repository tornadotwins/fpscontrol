using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;
using RAIN.Path;

public class GetNearestRatWaypoint : Action
{
    public GetNearestRatWaypoint()
    {
        actionName = "GetNearestRatWaypoint";
    }
	
    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		int tBestIndex = 0;
		float tBestDistSq = float.MaxValue;
		
		int tRatCount = agent.actionContext.GetContextItem<int>("ratwaypointcount");
		for (int i = 0; i < tRatCount; i++)
		{
			Vector3 tWaypointPosition = agent.actionContext.GetContextItem<Vector3>("ratwaypoint" + i);
			
			float tWaypointDistSq = (tWaypointPosition - agent.Avatar.transform.position).sqrMagnitude;
			if (tWaypointDistSq < tBestDistSq)
			{
				tBestIndex = i;
				tBestDistSq = tWaypointDistSq;
			}
		}
		
		agent.actionContext.SetContextItem<Vector3>("waypointposition", agent.actionContext.GetContextItem<Vector3>("ratwaypoint" + tBestIndex));

        return ActionResult.SUCCESS;
    }
}