/*
jmpp: Done!
*/
using System;
using UnityEngine;


namespace FPSControl
{
    //! Abstract description of a damage source.
    /*!
    Provides an abstract description of a source that inflicts damage on some target, so that users of this information can determine how to
    properly respond to this damage.
    */
    public class DamageSource
    {
        /*!
        Possible types of damage sources.
        */
        public enum DamageSourceType
        {
            /*!
            %Damage caused by collisions with static objects, e.g. walls.
            */
            StaticCollision,
            /*!
            %Damage caused by falling.
            */
            Fall,
            /*!
            %Damage caused by gunfire received.
            */
            GunFire,
            /*!
            %Damage caused by the action of a Melèe weapon.
            */
            MeleeAttack,
            /*!
            %Damage caused by the action of a turret.
            */
            AutoTurret,
            /*!
            %Damage caused by a source of heat, such as fire.
            */
            Heat,
            /*!
            %Damage caused by a source of very low temperatures.
            */
            Cold
        }
  
        /*!
        Possible types of objects inflicting the different types of damage described by the #DamageSourceType enumeration.
        */      
        public enum DamageSourceObjectType
        {
            /*!
            A damage-inflicting object controlled by any form of AI.
            */
            AI,
            /*!
            Represents the damage caused by the Player object.
            */
            Player,
            /*!
            Represents the damage caused by obstacle objects.
            */
            Obstacle
        }

        /*!
        The damage source type, as described by the #DamageSourceType options.
        */
        public DamageSourceType sourceType;
        /*!
        The object inflicting #sourceType damage, as described by the #DamageSourceObjectType options.
        */
        public DamageSourceObjectType sourceObjectType;
        /*!
        The gameObject causing the damage described by #sourceType and #sourceObjectType.
        */
        public GameObject sourceObject;
        /*!
        The ammount of damage inflicted by the #sourceObject.
        */
        public float damageAmount;
        /*!
        The origin position of the damage.
        */
        public Vector3 fromPosition;
        /*!
        Position of the inflicted damage.
        */
        public Vector3 appliedToPosition;
        /*!
        The <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider.html">collider</a> that was hit by the damage source.
        */
        public Collider hitCollider;
    }
}
