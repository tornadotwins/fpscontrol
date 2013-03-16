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
        public FPSControlWeaponPathDefinition definition = new FPSControlWeaponPathDefinition();

        public Transform origin;
        public Material material;

        private LineRenderer lineRenderer;
        

        public override void Initialize(FPSControlWeapon parent)
        {
            base.Initialize(parent);
            Debug.Log(parent);
            lineRenderer = transform.GetComponent<LineRenderer>();
            lineRenderer.material = material;
            lineRenderer.SetColors(definition.lineColor, definition.lineColor);
            if (definition.isPreFire) //We are going to do an arch
            {
                
            }
            else //We are doing a strait line
            {
                
            }
        }

        private void Update()
        {
            if (definition.render && definition.isPreFire && Weapon.canUse && currentState == Weapon.idleState)
            {
                RenderArch();
            }
            else if (definition.render && !definition.isPreFire && currentState == Weapon.idleState)
            {
                lineRenderer.SetVertexCount(0);
            }
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
            if (Physics.Raycast(origin.position, origin.forward, out hit, definition.maxDistance))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                Vector3 endPos = origin.position + (origin.forward * definition.maxDistance);
                lineRenderer.SetPosition(1, hit.point);
            }            
        }

        private void RenderOnce()
        {
            RenderLine();            
            StartCoroutine(FadeOutLine(RenderDone));
        }

        private void RenderDone()
        {
            lineRenderer.SetVertexCount(0);
            SetAlpha(1);
        }

        System.Collections.IEnumerator FadeOutLine(System.Action cbFunc)
        {
            float startTime = Time.time;
            while (true)
            {
                float n = Mathf.Clamp01(definition.fadeOutTime - (Time.time - startTime));
                SetAlpha(n);
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
            Color newColor = lineRenderer.material.color;
            newColor.a = alpha;
            lineRenderer.material.color = newColor;
        }

        private void RenderArch()
        {

        }

    }

}
