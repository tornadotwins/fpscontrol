using UnityEngine;
using System.Collections;
using System;
using FPSControl;
using FPSControl.States;
using FPSControl.States.Weapon;
using FPSControl.Data;

namespace FPSControl.Definitions
{

    [System.Serializable]
    public class FPSControlWeaponDefinitions
    {
        public FPSControlWeaponDefinition weaponDefinition;
        public FPSControlRangeWeaponDefinition rangeDefinition;
        public FPSControlWeaponAnimationDefinition animationDefinition;
        public FPSControlWeaponParticlesDefinition particlesDefinition;
        public FPSControlWeaponPathDefinition pathDefinition;
        public FPSControlWeaponSoundDefinition soundDefinition;

        public FPSControlWeaponDefinitions(FPSControlWeapon inputWeapon)
        {
            if (inputWeapon.GetType() == typeof(FPSControlRangedWeapon))
            {
                rangeDefinition = ((FPSControlRangedWeapon)inputWeapon).rangeDefinition;
                pathDefinition = ((FPSControlRangedWeapon)inputWeapon).weaponPath.definition;
            }
            weaponDefinition = inputWeapon.definition;
            animationDefinition = inputWeapon.weaponAnimation.definition;
            particlesDefinition = inputWeapon.weaponParticles.definition;
            soundDefinition = inputWeapon.weaponSound.definition;
        }

        public static void LoadDefintionsIntoWeapon(FPSControlWeaponDefinitions defintions, ref FPSControlWeapon inputWeapon)
        {
            if (inputWeapon.GetType() == typeof(FPSControlRangedWeapon))
            {
                ((FPSControlRangedWeapon)inputWeapon).rangeDefinition = defintions.rangeDefinition;
                ((FPSControlRangedWeapon)inputWeapon).weaponPath.definition = defintions.pathDefinition;
            }
            inputWeapon.definition = defintions.weaponDefinition;
            inputWeapon.weaponAnimation.definition = defintions.animationDefinition;
            inputWeapon.weaponParticles.definition = defintions.particlesDefinition;
            inputWeapon.weaponSound.definition = defintions.soundDefinition;
        }

        public FPSControlWeaponDefinitions() { }

    }

}
