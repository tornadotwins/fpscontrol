using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.BehaviorTrees;
using RAIN.Path;

public class ShockTrooperActivation : BTActivationManager
{
	public GameObject PatrolCollection;
	public Transform[] hitTorsoBackTransforms = new Transform[0];
	public Transform[] hitTorsoFrontTransforms = new Transform[0];
	public Transform[] hitTorsoRightTransforms = new Transform[0];
	public Transform[] hitTorsoLeftTransforms = new Transform[0];
	public Transform[] fireLoopTransforms = new Transform[0];
	public Transform[] reloadTransforms = new Transform[0];
	public Transform[] aimLoopTransforms = new Transform[0];
	
	public override void InitBehavior(Agent actor)
	{
		actor.actionContext.AddContextItem<int>("patrolcount", PatrolCollection.transform.childCount);
		
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
			actor.actionContext.AddContextItem<Vector3>("patrol" + i, tChildren[i].position);
		
		for (int i = 0; i < hitTorsoBackTransforms.Length; i++)
		{
			if (hitTorsoBackTransforms[i] != null) actor.Avatar.animation["HitTorsoBack"].AddMixingTransform(hitTorsoBackTransforms[i]);
		}
		
		for (int i = 0; i < hitTorsoFrontTransforms.Length; i++)
		{
			if (hitTorsoFrontTransforms[i] != null) actor.Avatar.animation["HitTorsoFront"].AddMixingTransform(hitTorsoFrontTransforms[i]);
		}
		
		for (int i = 0; i < hitTorsoRightTransforms.Length; i++)
		{
			if (hitTorsoRightTransforms[i] != null) actor.Avatar.animation["HitTorsoRight"].AddMixingTransform(hitTorsoRightTransforms[i]);
		}
		
		for (int i = 0; i < hitTorsoLeftTransforms.Length; i++)
		{
			if (hitTorsoLeftTransforms[i] != null) actor.Avatar.animation["HitTorsoLeft"].AddMixingTransform(hitTorsoLeftTransforms[i]);
		}
		
		for (int i = 0; i < fireLoopTransforms.Length; i++)
		{
			if (fireLoopTransforms[i] != null) actor.Avatar.animation["FireLoop"].AddMixingTransform(fireLoopTransforms[i], true);
		}
		
		
		for (int i = 0; i < reloadTransforms.Length; i++)
		{
			if (reloadTransforms[i] != null) actor.Avatar.animation["Reload"].AddMixingTransform(reloadTransforms[i]);
		}
		
		for (int i = 0; i < aimLoopTransforms.Length; i++)
		{
			if (aimLoopTransforms[i] != null) actor.Avatar.animation["AimLoop"].AddMixingTransform(aimLoopTransforms[i]);
		}
	}

	protected override void PreAction(Agent actor, float deltaTime)
	{
	}
}
