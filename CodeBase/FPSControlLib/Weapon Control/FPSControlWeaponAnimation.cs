using System;
using System.Collections;
using FPSControl.Data;
using UnityEngine;

namespace FPSControl
{
    public enum FiringPatternType
    {
        OncePerAnimation, TimedPattern
    }
    
    [RequireComponent(typeof(Animation))]
    public class FPSControlWeaponAnimation : FPSControlWeaponComponent
    {
        //General/Shared Animation States
        //[HideInInspector]
        public string ACTIVATE = "Activate";
        [HideInInspector]
        public string DEACTIVATE = "Deactivate";
        [HideInInspector]
        public string RUN = "Run";
        [HideInInspector]
        public string WALK = "Walk";
        [HideInInspector]
        public string IDLE = "Idle";
        //Ranged
        [HideInInspector]
        public string SCOPE_IO = "Scope IO";
        [HideInInspector]
        public string SCOPE_LOOP = "Scope Loop";
        [HideInInspector]
        public string FIRE = "Fire";
        [HideInInspector]
        public string RELOAD = "Reload";
        [HideInInspector]
        public string EMPTY = "Empty";
        //Melee
        [HideInInspector]
        public string CHARGE = "Charge";
        [HideInInspector]
        public string ATTACK = "Attack";
        [HideInInspector]
        public string DEFEND_ENTER = "Defend Enter";
        [HideInInspector]
        public string DEFEND_LOOP = "Defend Loop";
        [HideInInspector]
        public string DEFEND_EXIT = "Defend Exit";
        
        
        Animation _animation;

        public FiringPatternType patternType = FiringPatternType.OncePerAnimation;
        public bool blend;
        bool _patternComplete = false;
        public FalloffData firingPattern;

        public System.Action animationCompleteCallback;

        void Awake()
        {
            _animation = animation;
            //LAYERING SETUP
            if (_animation[ACTIVATE] != null)
            {
                _animation[ACTIVATE].layer = 0;
                _animation[ACTIVATE].wrapMode = WrapMode.ClampForever;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + ACTIVATE);
            }

            if (_animation[DEACTIVATE] != null)
            {
                _animation[DEACTIVATE].layer = 0;
                _animation[DEACTIVATE].wrapMode = WrapMode.ClampForever;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + DEACTIVATE);
            }

            if (_animation[RUN] != null)
            {
                _animation[RUN].layer = 0;
                _animation[RUN].wrapMode = WrapMode.Loop;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + RUN);
            }

            if (_animation[WALK] != null)
            {
                _animation[WALK].layer = 0;
                _animation[WALK].wrapMode = WrapMode.Loop;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + WALK);
            }

            if (_animation[IDLE] != null)
            {
                _animation[IDLE].layer = 0;
                _animation[IDLE].wrapMode = WrapMode.Loop;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + IDLE);
            }

            if (_animation[FIRE] != null)
            {
                _animation[FIRE].layer = 0;
                _animation[FIRE].wrapMode = WrapMode.ClampForever;
            }
            if (_animation[SCOPE_IO] != null)
            {
                _animation[SCOPE_IO].layer = 0;
            }
            if (_animation[SCOPE_LOOP] != null)
            {
                _animation[SCOPE_LOOP].layer = 0;
            }
            if (_animation[RELOAD] != null)
            {
                _animation[RELOAD].layer = 0;
                _animation[RELOAD].wrapMode = WrapMode.ClampForever;
            }
            if (_animation[EMPTY] != null)
            {
                _animation[EMPTY].layer = 0;
                _animation[EMPTY].wrapMode = WrapMode.ClampForever;
            }
            
            if (_animation[CHARGE] != null) _animation[CHARGE].layer = 0;
            if (_animation[ATTACK] != null) _animation[ATTACK].layer = 0;
            if (_animation[DEFEND_ENTER] != null) _animation[DEFEND_ENTER].layer = 0;
            if (_animation[DEFEND_LOOP] != null) _animation[DEFEND_LOOP].layer = 0;
            if (_animation[DEFEND_EXIT] != null) _animation[DEFEND_EXIT].layer = 0;
        }

        void Play(string stateName)
        {
            _animation.Play(stateName);
        }

        #region Invoke States

        public void Activate()
        {
            _animation.CrossFade(ACTIVATE);
        }

        public void Deactivate()
        {
            _animation.CrossFade(DEACTIVATE);
        }

        public void Idle()
        {
            _animation.CrossFade(IDLE);//,.05F);
        }

        public void Walk()
        {
            _animation.CrossFade(WALK);
        }

        public void Run()
        {
            _animation.CrossFade(RUN);
        }

        public void Fire()
        {
            //Debug.Log("fire anim");
            if(patternType == FiringPatternType.OncePerAnimation)
			{
				_animation[FIRE].time = 0;//.wrapMode = WrapMode.ClampForever;
            	_animation.CrossFade(FIRE,.05F);
			}
			else
			{
				StartCoroutine("DoFiringPattern");
			}
            //Debug.Log("....");
        }

        IEnumerator DoFiringPattern()
		{
			_animation[FIRE].wrapMode = WrapMode.Once;
			_patternComplete = false;
			//yield return 0;
			for(int i = 0; i < firingPattern.Length; i++)
			{
				_animation[FIRE].time = 0;//.wrapMode = WrapMode.ClampForever;
            	_animation.CrossFade(FIRE,.05F);
				
				if(i < firingPattern.Length-1)
				{
					FalloffPoint currentKey = firingPattern[i];
					FalloffPoint nextKey = firingPattern[i+1];
					
					//calculate the time between animations
					float timeBetween = (nextKey.location * firingPattern.distance) - (currentKey.location * firingPattern.distance);
					//wait for the time between
					yield return new WaitForSeconds(timeBetween);
				}
			}
			
			_animation[FIRE].wrapMode = WrapMode.ClampForever;
			_patternComplete = true;
		}

        public void Reload()
        {
            _animation.CrossFade(RELOAD);
        }

        public void Empty()
        {
            _animation[EMPTY].time = 0;
            _animation.CrossFade(EMPTY);
        }

        public void ScopeIn()
        {
            _animation[SCOPE_IO].time = 1F;
            _animation.CrossFade(SCOPE_IO);
        }

        public void ScopeOut()
        {
            _animation[SCOPE_IO].time = -1F;
            _animation.CrossFade(SCOPE_IO);
        }

        void ScopeIdle()
        {
            _animation.Play(SCOPE_LOOP);
        }

        public void Attack()
        {
            _animation[ATTACK].time = 0;//.wrapMode = WrapMode.ClampForever;
            _animation.CrossFade(ATTACK,.05F);
        }

        public void Charge()
        {
            _animation.CrossFade(CHARGE);
        }

        public void DefendIn()
        {
            _animation.CrossFade(DEFEND_ENTER);
        }

        public void DefendOut()
        {
            _animation.CrossFade(DEFEND_EXIT);
        }

        void DefendLoop()
        {
            _animation.Play(DEFEND_LOOP);
        }

        #endregion // Invoke States

        #region Animation Events

        public void AnimationEvent_ActivateComplete()
        {
            //_animation[ACTIVATE].wrapMode = WrapMode.Once;
            //Debug.Log("Received animation event!");
            DoCallBack();
        }

        public void AnimationEvent_DeactivateComplete()
        {
            DoCallBack();
        }

        public void AnimationEvent_ScopeInComplete()
        {
            ScopeIdle();
        }

        public void AnimationEvent_DefendInComplete()
        {
            DefendLoop();
        }

        public void AnimationEvent_FireComplete()
        {
           // _animation[FIRE].wrapMode = WrapMode.Once;
            if(patternType == FiringPatternType.TimedPattern)
			{
				if(!_patternComplete) return;
			}
			
			DoCallBack();
			
        }

        public void AnimationEvent_ReloadComplete()
        {
            DoCallBack();
        }

        public void AnimationEvent_EmptyComplete()
        {
            Debug.Log("empty complete");
            DoCallBack();
        }

        #endregion // Animation Events

        void DoCallBack()
        {
            //Debug.Log("animation callback");
            if (animationCompleteCallback != null) animationCompleteCallback();
            else Debug.LogWarning("No callback provided!");
            animationCompleteCallback = null;
        }
    }
}
