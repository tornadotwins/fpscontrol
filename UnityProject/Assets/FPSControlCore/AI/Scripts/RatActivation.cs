using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.BehaviorTrees;

public class RatActivation : BTActivationManager
{
	public GameObject RatCollection;
	
	public override void InitBehavior(Agent actor)
	{
		actor.actionContext.AddContextItem<int>("ratwaypointcount", RatCollection.transform.childCount);
		for (int i = 0; i < RatCollection.transform.childCount; i++)
			actor.actionContext.AddContextItem<Vector3>("ratwaypoint" + i, RatCollection.transform.GetChild(i).position);
	}

	protected override void PreAction(Agent actor, float deltaTime)
	{
	}

}
