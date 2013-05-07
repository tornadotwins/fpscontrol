//import Aether;
//import Legion.Core;
import RAIN.Core;

var _gunLogic : AIGunLogic;
//var _rival : Rival;
var _dataController : DataController;
var _player : GameObject;
var meleeDamage : float = 10.0f;
var hideWhenDead : boolean = false;
var _ai : RAINAgent;

function Start()
{
	_player = GameObject.Find("Player");
	_ai = gameObject.GetComponent("RAINAgent");
	_dataController = gameObject.GetComponent("DataController");
}

function Update()
{	
	_ai.Agent.actionContext.SetContextItem.<float>("health", _dataController.current);
	_ai.Agent.actionContext.SetContextItem.<float>("ammo", _gunLogic.dataController.current);
	
	var shootAt : GameObject = _ai.Agent.actionContext.GetContextItem.<GameObject>("enemytarget");
	if (shootAt != null)
	{
		_gunLogic._1stShootTo = shootAt.transform;
	}
}

function ApplyDamage(damageSource : DamageSource)
{
	if ((damageSource.sourceType == DamageSource.DamageSourceType.GunFire) &&
	    (damageSource.sourceObjectType != DamageSource.DamageSourceObjectType.Obstacle))
    {
		_ai.Agent.actionContext.SetContextItem.<float>("gothit", 1.0f);

		transform.Rotate(Vector3.up, -45f);
		var testforward:Vector3 = transform.forward;
		var testright:Vector3 = transform.right;
		transform.Rotate(Vector3.up, 45f);		
		
		if (Vector3.Dot(testforward, damageSource.fromPosition) > 0)
		{
			if (Vector3.Dot(testright, damageSource.fromPosition) > 0)
				_ai.Agent.actionContext.SetContextItem.<float>("hitdir", 1f); //left
			else
				_ai.Agent.actionContext.SetContextItem.<float>("hitdir", 4f); //front
		}
		else
		{
			if (Vector3.Dot(testright, damageSource.fromPosition) > 0)
				_ai.Agent.actionContext.SetContextItem.<float>("hitdir", 2f); //back
			else
				_ai.Agent.actionContext.SetContextItem.<float>("hitdir", 3f); //right
		}
		
		_ai.Agent.actionContext.SetContextItem.<GameObject>("enemytarget", damageSource.sourceObject);
	}
	_dataController.current -= damageSource.damageAmount;
}

//@Aether.MessageHandler("FirePrimaryWeapon")
function FirePrimaryWeapon()
{
	_gunLogic.FirePrimary("fire-", "empty-");
}
		
//@Aether.MessageHandler("FirePrimaryWeaponStop")
function FirePrimaryWeaponStop()
{
	_gunLogic.FirePrimaryStop("fire-");
}	

//@Aether.MessageHandler("ReloadPrimaryWeapon")
function ReloadPrimaryWeapon()
{
	_gunLogic.ReloadWeapon();
}

//@Aether.MessageHandler("Die")
function Die()
{
	if (hideWhenDead)
	{
		gameObject.SetActive(false);
	}
	else
	{
		var colliders : Collider[] = gameObject.GetComponentsInChildren.<Collider>();
		for (var i:int = 0; i < colliders.Length; i++)
		{
			if ((colliders[i] != null) && !colliders[i].isTrigger )
			{
				if( typeof( colliders[i] ) == CharacterController )
				{
					colliders[i].gameObject.SetActive(false);
				}
				else
				{
					GameObject.DestroyImmediate(colliders[i]);
				}
			}
		}		
	}
}

//@Aether.MessageHandler("MeleeAttack")
function MeleeAttack()
{
	var target : GameObject = _ai.Agent.actionContext.GetContextItem.<GameObject>("meleetarget");
	var damageSource : DamageSource = new DamageSource();
	damageSource.appliedToPosition = target.transform.position;
	damageSource.damageAmount = meleeDamage;
	damageSource.fromPosition = _ai.Agent.Avatar.gameObject.transform.position;
	damageSource.sourceObject = _ai.Agent.Avatar.gameObject;
	damageSource.sourceObjectType = DamageSource.DamageSourceObjectType.AI;
	damageSource.sourceType = DamageSource.DamageSourceType.MeleeAttack;
	
	target.SendMessage("ApplyDamage", damageSource, SendMessageOptions.DontRequireReceiver);
}
