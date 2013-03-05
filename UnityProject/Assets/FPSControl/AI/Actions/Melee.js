import RAIN.Action;
import RAIN.Core;

class Melee extends RAIN.Action.Action
{
	var ai:ShockTrooperAI;
	
	function newclass()
	{
		actionName = "Melee";
	}
	
	function Start(agent:Agent, deltaTime:float):ActionResult
	{
		ai = agent.Avatar.GetComponentInChildren(ShockTrooperAI);
		
        return ActionResult.SUCCESS;
	}

	function Execute(agent:Agent, deltaTime:float):ActionResult
	{
		ai.MeleeAttack();
		
        return ActionResult.SUCCESS;
	}
}