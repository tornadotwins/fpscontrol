using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class ShockTrooperSetupAction : Action
{
	public GameObject PatrolCollection;

	public ShockTrooperSetupAction()
    {
        actionName = "ShockTrooperSetupAction";
    }

    public override ActionResult Start(Agent agent, float deltaTime)
    {
		if (PatrolCollection == null) PatrolCollection = GameObject.Find(agent.Avatar.name + "Waypoints");
		
        return ActionResult.SUCCESS;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		if (PatrolCollection == null) return ActionResult.FAILURE;
		
		agent.actionContext.SetContextItem<int>("patrolcount", PatrolCollection.transform.childCount);
		
		List<Transform> tChildren = new List<Transform>();
		for (int i = 0; i < PatrolCollection.transform.childCount; i++)
		{
			int j;
			for (j = 0; j < tChildren.Count; j++)
			{
				if (PatrolCollection.transform.GetChild(i).name.CompareTo(tChildren[j].name) < 0)
					break;
			}
			
			tChildren.Insert(j, PatrolCollection.transform.GetChild(i));
		}
		
		for (int i = 0; i < tChildren.Count; i++)
			agent.actionContext.SetContextItem<Vector3>("patrol" + i, tChildren[i].position);

		return ActionResult.SUCCESS;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
 	     return ActionResult.SUCCESS;
    }
}