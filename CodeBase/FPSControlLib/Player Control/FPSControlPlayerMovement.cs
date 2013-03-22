using UnityEngine;
using System.Collections;
using FPSControl;


namespace FPSControl
{
    //[RequireComponent(typeof(Rigidbody))]
    //[RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    public class FPSControlPlayerMovement : FPSControlPlayerComponent
    {   
        public KeyCode runKeyLeft = KeyCode.LeftShift;
        public KeyCode runKeyRight = KeyCode.RightShift;
        public KeyCode crouchKeyLeft = KeyCode.LeftControl;
        public KeyCode crouchKeyRight = KeyCode.RightControl;
        public KeyCode jumpKey = KeyCode.Space;

        public float height;
        public Vector3 center { get { return new Vector3(0, height / 2F, 0); } }
        public float crouchHeight = 1.1F;
        public Vector3 crouchedCenter { get { return new Vector3(0, crouchHeight / 2F, 0); } }
        bool _crouched = false;
        public bool isCrouching { get { return _crouched; } }
        public float crouchSpeedModifier = .5F;
        
        public bool isMovingBackwards { get; private set; }
        public bool isStrafing { get; private set; }

        public float jumpForce = .25F;
        //public float airResistance = .75F;
        float _fallVelocity = 0F;
        //float _prevFallVelocity = 0F;
        //float _topFallVelocity = 0F;
        bool _wasGrounded;
        public bool isJumping { get { return _jumping; } }
        bool _jumping = false;
        bool canJump { get { return Player.currentState.canJump && _controller.height == height; } }

        public float gravity = 4F;

        public bool liveEditing = false;

        Vector3 _movement = Vector3.zero;
        Vector3 _movementRate = Vector3.zero;
        Vector3 _movementDelta = Vector3.zero;

        bool _input = false; //was there any user input this frame?

        FPSControlPlayerCamera _cam;
        CharacterController _controller;
        public CharacterController Controller{get{return _controller;}}
        Transform _transform;

        protected float _delta { get { return Time.deltaTime * 60F; } }

        void Awake()
        {
            UpdateVars();
        }

        void Update()
        {
            if (liveEditing)
            {
                UpdateVars();
            }
        }

        void UpdateVars()
        {
            _transform = transform;
            _controller = GetComponent<CharacterController>();
            _controller.height = height;
            _controller.center = center;
        }

        public void Initialize(FPSControlPlayerCamera playerCam, FPSControlPlayer player)
        {
            _wasGrounded = _controller.isGrounded;
            _cam = playerCam;
            Initialize(player);
        }

        public override void DoUpdate()
        {
            
            Vector3 prevMovement = _movement;
            _input = false;//reset our input
            _movementRate = _movement / Time.deltaTime; // rate = distance/time
            Vector3 force = Vector3.zero; //reset for new loop
            
            UpdateMovement();
            UpdateJump();
            UpdateCrouch();

            //this dampening really helps everything flow better.
            float movementDampening = (1 + (.2F * _delta));
            _movement.x /= movementDampening;
            _movement.y = (_movement.y > 0F) ? (_movement.y / movementDampening) : _movement.y;
            _movement.z /= movementDampening;

            //eliminate the floating point in-precision by limiting things to the ten-thousandth
            if (Mathf.Abs(_movement.x) < 0.0001f) _movement.x = 0F;
            if (Mathf.Abs(_movement.y) < 0.0001f) _movement.y = 0F;
            if (Mathf.Abs(_movement.z) < 0.0001f) _movement.z = 0F;

            if (_controller.isGrounded && _fallVelocity <= 0) _fallVelocity = Physics.gravity.y * .0004F;
            else _fallVelocity += (Physics.gravity.y * .0004F) * _delta;

            _wasGrounded = _controller.isGrounded;

            force += _movement * _delta;
            force.y += _fallVelocity * _delta;

            _movementDelta = _movement - prevMovement;

            Vector3 projectedXZ = Vector3.Scale((_transform.localPosition + force), new Vector3(1, 0, 1));
            float projectedY = _transform.localPosition.y + force.y;

            // apply forces
            _controller.Move(force);

            Vector3 actualXZ = Vector3.Scale(_transform.localPosition, new Vector3(1, 0, 1));
            float actualY = _transform.localPosition.y;

            if (projectedXZ != actualXZ) CorrectHorizontalForce(actualXZ - projectedXZ);
            if ((projectedY > actualY) && _movement.y > 0) CorrectUpForce(actualY - projectedY);

            if (!_input) Player.ChangeState("Idle"); //if we have no input this frame, switch back to idle
        }

        void CorrectHorizontalForce(Vector3 impact)
        {
            _movement.x += (impact.x / _delta);
            _movement.z += (impact.z / _delta);
        }

        void CorrectUpForce(float impact)
        {
            _movement.y += (impact / _delta);
        }

        void UpdateMovement()
        {
            if (!Player.currentState.canMove) return;

            float hi = Input.GetAxis("Horizontal");
            float vi = Input.GetAxis("Vertical");
            _input = (hi != 0 || vi != 0);

            float strafeSqr = Mathf.Sqrt(2F) / 2F;
            if (hi == 0 || vi == 0) strafeSqr = 1F; 

            // Running?
            if ((Input.GetKey(runKeyLeft) || Input.GetKey(runKeyRight)) && !_crouched && !_jumping) //you can't run if crouched, or if in mid jump
            {
                Player.ChangeState(Player["Run"]);
            }
            else if(_input) // Walking
            {
                Player.ChangeState(Player["Walk"]);
            }

            //this just allows us to use bigger numbers, so we aren't always dealing in centimeters
            float forwardSpeed = state.forwardSpeed * .01F;
            float strafeSpeed = state.strafeSpeed * .01F;
            float reverseSpeed = state.reverseSpeed * .01F;

            if (vi > 0)
            {
                vi = vi * forwardSpeed;
                isMovingBackwards = false;
            }
            else if (vi < 0)
            {
                vi = vi * reverseSpeed;
                isMovingBackwards = true;
            }

            // Strafing
            if (hi != 0)
            {
                hi = hi * strafeSpeed;
                isStrafing = (vi == 0) ? true : false;
            }

            Vector3 fwd = Vector3.forward * vi;
            Vector3 hor = Vector3.right * hi;

            

            _movement += _transform.TransformDirection(fwd+hor) * (strafeSqr * _delta);
            //_movement *= _delta; //BAD???

            /*DEBUGGING*/
            //reportedMovement = _movement;
            //reportedInput = fwd + hor;
        }
        /*DEBUGGING
        Vector3 reportedMovement;
        Vector3 reportedInput;

        void OnGUI()
        {
            GUILayout.Box(Input.GetAxis("Horizontal") + " " + Input.GetAxis("Vertical"));
            GUILayout.Box("" + reportedInput*100);
            GUILayout.Box("" + reportedMovement*1000);

        }
        */

        void UpdateJump()
        {
            if(_controller.isGrounded && _jumping) _jumping = false;
            
            if (!canJump) return;

            if (Input.GetKeyDown(jumpKey) && _controller.isGrounded)
            {
                //Debug.Log("jump!");
                _input = true;
                _movement += new Vector3(0, jumpForce, 0);
                Player.ChangeState("Jump");
                _jumping = true;
            }
        }

        void UpdateCrouch()
        {
            if (!Player.currentState.canCrouch) return;
            
            _crouched = Input.GetKey(crouchKeyLeft) || Input.GetKey(crouchKeyRight);
            
            if(_crouched)
            {
                //Debug.Log("crouching");
                if (_controller.height == height)
                {
                    _controller.height = crouchHeight;
                    _controller.center = crouchedCenter;
                }

                _movement.x *= crouchSpeedModifier;
                _movement.z *= crouchSpeedModifier;
            }
            else
            {
                if (_controller.height == crouchHeight)
                {
                    _controller.height = height;
                    _controller.center = center;
                }
            }

            _cam.height = _controller.height;
        }
    }
}
