using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class RatSetupAction : Action
{
	public GameObject RatCollection;
	
    public RatSetupAction()
    {
        actionName = "RatSetupAction";
    }

    public override ActionResult Start(Agent agent, float deltaTime)
    {
		if (RatCollection == null) RatCollection = GameObject.Find(agent.Avatar.name + "Waypoints");		
		
		return ActionResult.SUCCESS;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		if (RatCollection == null) return ActionResult.FAILURE;
		
		agent.actionContext.SetContextItem<float>("foundfound", 0);
		agent.actionContext.SetContextItem<int>("ratwaypointcount", RatCollection.transform.childCount);
		for (int i = 0; i < RatCollection.transform.childCount; i++)
		{
			agent.actionContext.SetContextItem<Vector3>("ratwaypoint" + i, RatCollection.transform.GetChild(i).position);
//			Debug.Log("ratwaypoint" + i + " = " + RatCollection.transform.GetChild(i).position);
		}
				
        return ActionResult.SUCCESS;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
 	     return ActionResult.SUCCESS;
    }
}
