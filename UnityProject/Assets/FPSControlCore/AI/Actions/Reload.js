import RAIN.Action;
import RAIN.Core;

class Reload extends RAIN.Action.Action
{
	var ai:ShockTrooperAI;
	
	function newclass()
	{
		actionName = "Reload";
	}
	
	function Start(agent:Agent, deltaTime:float):ActionResult
	{
		ai = agent.Avatar.GetComponentInChildren(ShockTrooperAI);
		
		return ActionResult.SUCCESS;
	}

	function Execute(agent:Agent, deltaTime:float):ActionResult
	{
		ai.ReloadPrimaryWeapon();

		return ActionResult.SUCCESS;
	}
}
