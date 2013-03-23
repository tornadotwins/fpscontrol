using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;
using FPSControl.Data;
using FPSControl.Definitions;

namespace FPSControl
{
    [RequireComponent (typeof(LineRenderer))]
    public class FPSControlWeaponPath : FPSControlWeaponComponent
    {
        [HideInInspector]
        public FPSControlWeaponPathDefinition definition = new FPSControlWeaponPathDefinition();
        [HideInInspector]
        public Transform origin;
        [HideInInspector]
        public Material material;
        public float lineRendererScaleFactor = 1;

        [HideInInspector]
        public Vector3 shootVelocity { get { return _startingVelocity; } }

        [HideInInspector]
        private LineRenderer lineRenderer;
        [HideInInspector]
        private Vector3 _startingVelocity;
        [HideInInspector]
        private Transform interactionManager;

        [HideInInspector]
        public override void Initialize(FPSControlWeapon parent)
        {
            base.Initialize(parent);            
            lineRenderer = transform.GetComponent<LineRenderer>();
            lineRenderer.material = material;
            lineRenderer.useWorldSpace = !definition.isPreFire;
            StartCoroutine(FinishInitialize());
        }

        private System.Collections.IEnumerator FinishInitialize()
        {
            yield return 0; //Wait one frame because Player refrence doent happen till end of frame
            interactionManager = Weapon.Parent.Player.interactionManager.transform;
        }

        [HideInInspector]
        WeaponState lastState;
        private void Update()
        {
            if (definition.render && definition.isPreFire)
            {
                if (currentState == Weapon.idleState)
                {
                    if (lastState == Weapon.fireState)
                    {
                        //StartCoroutine(FadeLine(null, true));
                    }
                    _startingVelocity = (origin.forward * (definition.leavingForce));
                    RenderArch();
                }
                else if (currentState == Weapon.fireState)
                {
                    if (lastState == Weapon.idleState)
                    {
                       //StartCoroutine(FadeLine(null, false));
                        lineRenderer.SetVertexCount(0);
                    }
                }                
            }
            else if (definition.render && !definition.isPreFire && currentState == Weapon.idleState)
            {
                lineRenderer.SetVertexCount(0);
            }
            lastState = currentState;
        }

        public void Fire()
        {
            if (!definition.isPreFire && definition.render) //Make sure the weapon dosent do an arch type and that we are ataully going to render
            {
                if (currentState == Weapon.idleState) //If we are in idle state that means that this is the first time
                {                    
                    if (!definition.consistentRender) //If we are not doing a consistant render then we will only render once
                    {
                        RenderOnce();
                    }                    
                }
                else if (currentState == Weapon.fireState) //If we are in the firing state
                {
                    if (definition.consistentRender) //If we are not doing a consistant render then we will only render once
                    {
                        RenderLine();
                    } 
                }
            }
        }
        
        private void RenderLine()
        {
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, origin.position);
            RaycastHit hit;
            Ray ray = new Ray(interactionManager.position, interactionManager.forward);
            if (Physics.Raycast(ray, out hit, definition.maxDistance, ((FPSControlRangedWeapon)Weapon).gunDamageLayers.value))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                Vector3 endPos = interactionManager.position + (interactionManager.forward * definition.maxDistance);
                lineRenderer.SetPosition(1, hit.point);
            }
        }

        private void RenderOnce()
        {
            RenderLine();            
            StartCoroutine(FadeLine(RenderDone));
        }

        private void RenderDone()
        {
            lineRenderer.SetVertexCount(0);
            SetAlpha(1);
        }

        System.Collections.IEnumerator FadeLine(System.Action cbFunc, bool fadeOut = true)
        {
            float startTime = Time.time;
            while (true)
            {
                float n = Mathf.Clamp01(definition.fadeOutTime - (Time.time - startTime));
                if (fadeOut)
                {
                    SetAlpha(n);
                }
                else
                {
                    SetAlpha(1 - n);
                }                
                if (n == 0)
                {
                    if (cbFunc != null) cbFunc();
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void SetAlpha(float alpha)
        {
            if (!lineRenderer.material.HasProperty("_Color")) return;
            Color newColor = lineRenderer.material.color;
            newColor.a = alpha;
            lineRenderer.material.color = newColor;
        }

        private void RenderArch()
        {
            lineRenderer.SetVertexCount((int)(definition.maxTimeDistance * 60));
            Vector3 previousPosition = origin.position;
            float totalDistance = 0;
            for (int i = 0; i < (int)(definition.maxTimeDistance * 60); i++)
            {
                Vector3 currentPosition = GetTrajectoryPoint(origin.position, _startingVelocity, i, 1, Physics.gravity);
                Vector3 direction = currentPosition - previousPosition;
                direction.Normalize();

                float distance = Vector3.Distance(currentPosition, previousPosition);
                totalDistance += distance;

                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(previousPosition, direction, out hitInfo, distance, ((FPSControlRangedWeapon)Weapon).gunDamageLayers.value))
                {
                    lineRenderer.SetPosition(i, hitInfo.point);
                    lineRenderer.SetVertexCount(i);
                    break;
                }

                previousPosition = currentPosition;
                currentPosition = transform.InverseTransformPoint(currentPosition);
                lineRenderer.SetPosition(i, currentPosition);
            }
            Vector2 newScale = material.mainTextureScale;
            newScale.x = totalDistance * lineRendererScaleFactor;
            material.mainTextureScale = newScale;
        }

        Vector3 GetTrajectoryPoint(Vector3 startingPosition, Vector3 initialVelocity, float timestep, float lob, Vector3 gravity)
        {
            float physicsTimestep = Time.fixedDeltaTime;
            Vector3 stepVelocity = initialVelocity * physicsTimestep;

            //Gravity is already in meters per second, so we need meters per second per second
            Vector3 stepGravity = gravity * physicsTimestep * physicsTimestep;

            return startingPosition + (timestep * stepVelocity) + (((timestep * timestep + timestep) * stepGravity) / 2.0f);
        }

        public static Vector3 GetTrajectoryVelocity(Vector3 startingPosition, Vector3 targetPosition, float lob, Vector3 gravity)
        {
            float physicsTimestep = Time.fixedDeltaTime;
            float timestepsPerSecond = Mathf.Ceil(1f / physicsTimestep);

            //By default we set n so our projectile will reach our target point in 1 second
            float n = lob * timestepsPerSecond;

            Vector3 a = physicsTimestep * physicsTimestep * gravity;
            Vector3 p = targetPosition;
            Vector3 s = startingPosition;

            Vector3 velocity = (s + (((n * n + n) * a) / 2f) - p) * -1 / n;

            //This will give us velocity per timestep. The physics engine expects velocity in terms of meters per second
            velocity /= physicsTimestep;
            return velocity;
        }

    }

}
