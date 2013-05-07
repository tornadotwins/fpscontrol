#pragma strict

import RAIN.Core;

var _ai : RAINAgent;
var _dataController : DataController;
var _hungry: float;

function Start () {

	_ai = gameObject.GetComponent.<RAINAgent>();
	_dataController = gameObject.GetComponent.<DataController>();
	
	_ai.Agent.actionContext.SetContextItem.<float>("hungry", 15f);
	_ai.Agent.actionContext.SetContextItem.<float>("dead", 0f);
	_ai.Agent.actionContext.SetContextItem.<float>("foundfood", 0f);
	_ai.Agent.actionContext.SetContextItem.<float>("gothit", 0f);
}

function Update () {

	_ai.Agent.actionContext.SetContextItem.<float>("health", _dataController.current);
	_ai.Agent.actionContext.SetContextItem.<float>("currenttime", Time.time);
	_hungry = _ai.Agent.actionContext.GetContextItem.<float>("hungry");
}

function ApplyDamage(damageSource : DamageSource)
{
	if ((damageSource.sourceType == DamageSource.DamageSourceType.GunFire) &&
	    (damageSource.sourceObjectType != DamageSource.DamageSourceObjectType.Obstacle))
    {
		_ai.Agent.actionContext.SetContextItem.<float>("gothit", 1.0f);
		_ai.Agent.actionContext.SetContextItem.<GameObject>("enemytarget", damageSource.sourceObject);
	}
	
	_dataController.current -= damageSource.damageAmount;
}