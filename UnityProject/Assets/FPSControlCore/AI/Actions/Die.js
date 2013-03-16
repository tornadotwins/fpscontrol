import RAIN.Action;
import RAIN.Core;

class Die extends RAIN.Action.Action
{
	var ai:ShockTrooperAI;
	
	function newclass()
	{
		actionName = "Die";
	}
	
	function Start(agent:Agent, deltaTime:float):ActionResult
	{
		ai = agent.Avatar.GetComponentInChildren(ShockTrooperAI);
		
        return ActionResult.SUCCESS;
	}

	function Execute(agent:Agent, deltaTime:float):ActionResult
	{
		ai.Die();
		
        return ActionResult.SUCCESS;
	}
}