using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;
using RAIN.Path;

public class GetRandomRatWaypoint : Action
{
    public GetRandomRatWaypoint()
    {
        actionName = "GetRandomRatWaypoint";
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		int tRandomCount = agent.actionContext.GetContextItem<int>("ratwaypointcount");
		
        int tBestIndex = UnityEngine.Random.Range(0, tRandomCount);
		agent.actionContext.SetContextItem<Vector3>("waypointposition", agent.actionContext.GetContextItem<Vector3>("ratwaypoint" + tBestIndex));
		
//		Debug.Log("Choosing waypointposition = " + agent.actionContext.GetContextItem<Vector3>("ratwaypoint" + tBestIndex));
		
        return ActionResult.SUCCESS;
    }
}