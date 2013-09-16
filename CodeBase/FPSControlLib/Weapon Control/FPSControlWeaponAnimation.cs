using System;
using System.Collections;
using FPSControl.Data;
using UnityEngine;
using FPSControl.Definitions;

namespace FPSControl
{
    public enum FiringPatternType
    {
        OncePerAnimation, TimedPattern
    }
    
    [RequireComponent(typeof(Animation))]
    public class FPSControlWeaponAnimation : FPSControlWeaponComponent
    {
        [HideInInspector]
        public FPSControlWeaponAnimationDefinition definition = new FPSControlWeaponAnimationDefinition();
        [HideInInspector]
        public FalloffData firingPattern = new FalloffData();
        [HideInInspector]
        private Animation _animation;
        [HideInInspector]
        private bool _patternComplete = false;

        [HideInInspector]
        public System.Action animationCompleteCallback;

        void Awake()
        {
            _animation = animation;
            
            //LAYERING SETUP
            if (_animation[definition.ACTIVATE] != null)
            {
                _animation[definition.ACTIVATE].layer = 0;
                _animation[definition.ACTIVATE].wrapMode = WrapMode.ClampForever;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + definition.ACTIVATE);
            }

            if (_animation[definition.DEACTIVATE] != null)
            {
                _animation[definition.DEACTIVATE].layer = 0;
                _animation[definition.DEACTIVATE].wrapMode = WrapMode.ClampForever;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + definition.DEACTIVATE);
            }

            if (_animation[definition.RUN] != null)
            {
                _animation[definition.RUN].layer = 0;
                _animation[definition.RUN].wrapMode = WrapMode.Loop;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + definition.RUN);
            }

            if (_animation[definition.WALK] != null)
            {
                _animation[definition.WALK].layer = 0;
                _animation[definition.WALK].wrapMode = WrapMode.Loop;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + definition.WALK);
            }

            if (_animation[definition.IDLE] != null)
            {
                _animation[definition.IDLE].layer = 0;
                _animation[definition.IDLE].wrapMode = WrapMode.Loop;
            }
            else
            {
                Debug.LogWarning(gameObject.name + " missing Animation: " + definition.IDLE);
            }

            if (_animation[definition.FIRE] != null)
            {
                _animation[definition.FIRE].layer = 0;
                _animation[definition.FIRE].wrapMode = WrapMode.ClampForever;
            }
            if (_animation[definition.SCOPE_IO] != null)
            {
                _animation[definition.SCOPE_IO].layer = 0;
            }
            if (_animation[definition.SCOPE_LOOP] != null)
            {
                _animation[definition.SCOPE_LOOP].layer = 0;
            }
            if (_animation[definition.RELOAD] != null)
            {
                _animation[definition.RELOAD].layer = 0;
                _animation[definition.RELOAD].wrapMode = WrapMode.ClampForever;
            }
            if (_animation[definition.EMPTY] != null)
            {
                _animation[definition.EMPTY].layer = 0;
                _animation[definition.EMPTY].wrapMode = WrapMode.ClampForever;
            }

            if (_animation[definition.CHARGE] != null) _animation[definition.CHARGE].layer = 0;
            if (_animation[definition.ATTACK] != null) _animation[definition.ATTACK].layer = 0;
            if (_animation[definition.DEFEND_ENTER] != null) _animation[definition.DEFEND_ENTER].layer = 0;
            if (_animation[definition.DEFEND_LOOP] != null) _animation[definition.DEFEND_LOOP].layer = 0;
            if (_animation[definition.DEFEND_EXIT] != null) _animation[definition.DEFEND_EXIT].layer = 0;
        }

        void Play(string stateName)
        {
            if (_animation[stateName] == null) return; 
            _animation.Play(stateName);
        }

        #region Invoke States

        public void Activate()
        {
            //Debug.Log(_animation);
            if (_animation[definition.ACTIVATE] == null)
            {
                AnimationEvent_ActivateComplete();
            }
            else
            {
                _animation.CrossFade(definition.ACTIVATE);
            }
        }

        public void Deactivate()
        {
            if (_animation[definition.DEACTIVATE] == null)
            {
                AnimationEvent_DeactivateComplete();
            }
            else
            {
                _animation.CrossFade(definition.DEACTIVATE);
            }
        }

        public void Idle()
        {
            if (_animation[definition.IDLE] == null) return;
            _animation.CrossFade(definition.IDLE);
        }

        public void Walk()
        {
            if (_animation[definition.WALK] == null) return;
            _animation.CrossFade(definition.WALK);
        }

        public void Run()
        {
            if (_animation[definition.RUN] == null) return;
            _animation.CrossFade(definition.RUN);
        }

        public void Fire()
        {
            if (_animation[definition.FIRE] == null)
            {
                AnimationEvent_FireComplete();
            }
            else
            {
                if (definition.patternType == FiringPatternType.OncePerAnimation)
                {
                    _animation[definition.FIRE].time = 0;//.wrapMode = WrapMode.ClampForever;
                    _animation.CrossFade(definition.FIRE, .05F);
                }
                else
                {
                    StartCoroutine("DoFiringPattern");
                }
            }
        }

        IEnumerator DoFiringPattern()
		{
            if (_animation[definition.FIRE] != null)
            {
                _animation[definition.FIRE].wrapMode = WrapMode.Once;
                _patternComplete = false;
                //yield return 0;
                for (int i = 0; i < firingPattern.Length; i++)
                {
                    _animation[definition.FIRE].time = 0;//.wrapMode = WrapMode.ClampForever;
                    _animation.CrossFade(definition.FIRE, .05F);

                    if (i < firingPattern.Length - 1)
                    {
                        FalloffPoint currentKey = firingPattern[i];
                        FalloffPoint nextKey = firingPattern[i + 1];

                        //calculate the time between animations
                        float timeBetween = (nextKey.location * firingPattern.distance) - (currentKey.location * firingPattern.distance);
                        //wait for the time between
                        yield return new WaitForSeconds(timeBetween);
                    }
                }

                _animation[definition.FIRE].wrapMode = WrapMode.ClampForever;
                _patternComplete = true;
            }
		}

        public void Reload()
        {
            if (_animation[definition.RELOAD] == null)
            {
                AnimationEvent_ReloadComplete();
            }
            else
            {
                _animation.CrossFade(definition.RELOAD);
            }
        }

        public void Empty()
        {
            if (_animation[definition.EMPTY] == null)
            {
                AnimationEvent_EmptyComplete();
            }
            else
            {
                _animation[definition.EMPTY].time = 0;
                _animation.CrossFade(definition.EMPTY);
            }
        }

        public void ScopeIn()
        {
            if (_animation[definition.SCOPE_IO] == null) return;
            _animation[definition.SCOPE_IO].time = 1F;
            _animation.CrossFade(definition.SCOPE_IO);
        }

        public void ScopeOut()
        {
            if (_animation[definition.SCOPE_IO] == null) return;
            _animation[definition.SCOPE_IO].time = -1F;
            _animation.CrossFade(definition.SCOPE_IO);
        }

        void ScopeIdle()
        {
            if (_animation[definition.SCOPE_LOOP] == null) return;
            _animation.Play(definition.SCOPE_LOOP);
        }

        public void Attack()
        {
            if (_animation[definition.ATTACK] == null) return;
            _animation[definition.ATTACK].time = 0;
            _animation.CrossFade(definition.ATTACK, .05F);
        }

        public void Charge()
        {
            if (_animation[definition.CHARGE] == null) return;
            _animation.CrossFade(definition.CHARGE);
        }

        public void DefendIn()
        {
            if (_animation[definition.DEFEND_ENTER] == null)
            {
                AnimationEvent_DefendInComplete();
            }
            else
            {
                _animation.CrossFade(definition.DEFEND_ENTER);
            }            
        }

        public void DefendOut()
        {
            if (_animation[definition.DEFEND_EXIT] == null) return;
            _animation.CrossFade(definition.DEFEND_EXIT);
        }

        void DefendLoop()
        {
            if (_animation[definition.DEFEND_LOOP] == null) return;
            _animation.Play(definition.DEFEND_LOOP);
        }

        #endregion // Invoke States

        #region Animation Events

        public void AnimationEvent_ActivateComplete()
        {
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
            if (definition.patternType == FiringPatternType.TimedPattern)
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
            //Debug.Log("empty complete");
            DoCallBack();
        }

        #endregion 

        void DoCallBack()
        {
            if (animationCompleteCallback != null) animationCompleteCallback();
            else Debug.LogWarning("No callback provided!");
            animationCompleteCallback = null;
        }
    }
}
