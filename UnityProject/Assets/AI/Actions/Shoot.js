import RAIN.Action;
import RAIN.Core;

class Shoot extends RAIN.Action.Action
{
	var ai:ShockTrooperAI;
	
	function newclass()
	{
		actionName = "Shoot";
	}
	
	function Start(agent:Agent, deltaTime:float):ActionResult
	{
		ai = agent.Avatar.GetComponentInChildren(ShockTrooperAI);
		
		return ActionResult.SUCCESS;
	}

	function Execute(agent:Agent, deltaTime:float):ActionResult
	{
		ai.FirePrimaryWeapon();
		
		return ActionResult.SUCCESS;
	}
}
