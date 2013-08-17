using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{    
    class FPSControlPlayerStats : MonoBehaviour
    {
        public DataController healthData;
        //public DataController staminaData; //we could support multiple data controllers

        public bool loadLevel = true;
        public bool takeCollideDamage = true;
        public float collideThreshold = 7F;
        public float maxHealth = 100F;
        public float defaultDamage = 1F;
        public string deathMusic = "PlayerDeath";
        bool _isAlive = true;

        void Start()
        {
	        healthData.max = maxHealth;
	        healthData.current = healthData.max;
	        healthData.initialized = true;
        }


        //---
        // been hit by something, is it mortal?
        //---
        void OnCollisionEnter( Collision collision )
        {
	        float damage = collision.relativeVelocity.magnitude;
	
	        //--- take collider damage? (fall damage, etc)
	        if( takeCollideDamage && ( damage > collideThreshold ) )
	        {
        //		Debug.Log( "collided with " + collision.collider.name + " with force " + damage );
                            DamageSource damageSource = new DamageSource();
                            damageSource.damageAmount = damage - collideThreshold;
                            damageSource.fromPosition = collision.transform.position;
                            damageSource.appliedToPosition = collision.transform.position;
                            damageSource.sourceObject = collision.gameObject;
                            damageSource.sourceObjectType = DamageSource.DamageSourceObjectType.Obstacle;
                            damageSource.sourceType = DamageSource.DamageSourceType.StaticCollision;
                    
		        ApplyDamage( damageSource);
	        }

		    if (collision.gameObject.tag == "Trap")
	        {
		        //
	        }
        }


        //---
        // 
        //---
        public bool alive{get{return _isAlive;}}

        public void ApplyHealthAdditive(float value)
        {
            healthData.current += value;
            //do death check.
            if (isDead) OnDeath();
    
        }

        public void ApplyHealth(float value)
        {
            healthData.current = value;
           //do death check.
            if (isDead) OnDeath();
        }

        void OnDeath()
        {
            FPSControlPlayerEvents.Death();
            //support legacy
            MessengerControl.Broadcast("FadeIn", deathMusic);
            BroadcastMessage("OnPlayerDeath",SendMessageOptions.DontRequireReceiver);
        }

        //---
        // called via SendMessage from weapons or traps raycasting
        //---
        public void ApplyDamage(DamageSource damageSource )
        {
            //--- get quadrant of Player that was shot
            HurtQuadrant hurtQuad = HurtQuadrant.FRONT;
            Vector3 hitDir = damageSource.appliedToPosition - transform.position;
            Vector3 left = transform.TransformDirection(Vector3.left);
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            float forwardDist = Vector3.Dot(forward, hitDir);
            float sideDist = Vector3.Dot(left, hitDir);

            if (Mathf.Abs(sideDist) < 0.1F)
            {
                if (forwardDist < 0)
                {
                    hurtQuad = HurtQuadrant.BACK;
                }
                else
                {
                    hurtQuad = HurtQuadrant.FRONT;
                }
            }
            else
            {
                if (sideDist >= 0)
                {
                    hurtQuad = HurtQuadrant.LEFT;
                }
                else
                {
                    hurtQuad = HurtQuadrant.RIGHT;
                }
            }
            //this is legacy
            BroadcastMessage("GotHurtQuadrant", (int)hurtQuad);
            
            TakeDamage(damageSource);

            //did we just die?
            if (isDead) OnDeath();
        }

        //---
        // reduce health
        //---
        public void TakeDamage( DamageSource damageSource )
        {
	        healthData.current -= damageSource.damageAmount;
            FPSControlPlayerEvents.ReceiveDamage(damageSource);
        }

        //---
        // is player dead?
        //---
        public bool isDead
        {
	        get
            {
                if( ( healthData.current <= 0 ) && ( _isAlive ) )  _isAlive = false;
		        return !_isAlive;
	        }
        }
    }
}
