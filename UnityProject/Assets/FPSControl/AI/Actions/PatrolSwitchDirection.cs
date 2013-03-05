using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class PatrolSwitchDirection : Action
{
    public PatrolSwitchDirection()
    {
        actionName = "PatrolSwitchDirection";
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		int tWaypointDirection = agent.actionContext.GetContextItem<int>("patroldirection");
		tWaypointDirection *= -1;
		agent.actionContext.SetContextItem<int>("patroldirection", tWaypointDirection);
		
        return ActionResult.SUCCESS;
    }
}