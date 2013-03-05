using RAIN.Core;
using UnityEngine;


//---
//
//---
namespace FPSControl
{
	public class RatAI : MonoBehaviour
	{
		RAINAgent		_ai;
		DataController	_dataController;
		float				_hungry;


		public void Start ()
		{
			_ai = gameObject.GetComponent<RAINAgent>();
			_dataController = gameObject.GetComponent<DataController>();
	
			_ai.Agent.actionContext.SetContextItem<float>("hungry", 15f);
			_ai.Agent.actionContext.SetContextItem<float>("dead", 0f);
			_ai.Agent.actionContext.SetContextItem<float>("foundfood", 0f);
		}
		

		public void Update ()
		{
			_ai.Agent.actionContext.SetContextItem<float>("health", _dataController.current);
			_ai.Agent.actionContext.SetContextItem<float>("currenttime", Time.time);
		}

		public void ApplyDamage(DamageSource damageSource )
		{
			if ((damageSource.sourceType == DamageSource.DamageSourceType.GunFire) &&
			    (damageSource.sourceObjectType != DamageSource.DamageSourceObjectType.Obstacle))
		    {
				_ai.Agent.actionContext.SetContextItem<float>("gothit", 1.0f);
				_ai.Agent.actionContext.SetContextItem<GameObject>("enemytarget", damageSource.sourceObject);
			}
	
			_dataController.current -= damageSource.damageAmount;
		}
	}
}