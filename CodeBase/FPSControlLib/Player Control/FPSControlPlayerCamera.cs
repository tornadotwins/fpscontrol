using UnityEngine;
using System.Collections;
using FPSControl;

namespace FPSControl
{

    [RequireComponent(typeof(Camera))]
    public class FPSControlPlayerCamera : FPSControlPlayerComponent
    {
        //Public Params
        public Vector2 mouseSensitivity = new Vector2(3, 3);
        public bool invertedY = false;
        public Vector2 pitchConstraints = new Vector2(-65, 90);

        public float height { get; set; }
        public Vector3 headPosition = new Vector3(0, -.2F, 0);
        
        //public 
            bool rigid = true;
        //public 
            float bodyFollow = 10F;

        //The Weapon Camera
        public Camera weaponCamera;
        public float weaponCameraZOffset;
        public LayerMask weaponCameraCullingMask;
        public int weaponCameraDepth = 1;
        public float weaponCameraFOV = 35;
        public float weaponCameraNearClip = .1F;
        public float weaponCameraFarClip = 10F;
        public Transform weaponHolder;
        public float trackSpeed = 6;
        public float scopeSpeed = 3;
        //public float blendThreshold = 2;

        
        public Vector3 bobOffset = new Vector3(0, .35F, 0);
        public Vector3 bobAmplitude = new Vector3(0, 1, 0);
        public bool noise = false;
        public Vector3 noiseAmount = new Vector3(.1F, .1F, .1F);
        Vector3 _appliedNoise = Vector3.zero;
        public float noiseUpdateInterval = .3F;
        float _lastNoiseUpdate;
        Vector3 _noiseOffset = Vector3.zero;
        
        //Internal Stuff
        FPSControlPlayerMovement _playerMovement;
        Transform _transform;
        Transform _root;
        Camera _camera;
        Quaternion _startRotation;
        float _startPitch{get{return _startRotation.eulerAngles.x;}}
        float _startYaw{get{return _startRotation.eulerAngles.y;}}
        float _pitch = 0F;
        float _yaw = 0F;
        Quaternion _playerPrevRotation;
        Transform _control;
        Transform _upperBody;
        Oscillator _oscillatorX;
        Oscillator _oscillatorY;
        Oscillator _oscillatorZ;
        bool _scope = false;
        float _scopeFOV = 0;
        public float baseFOV { get; private set; }

        public bool applyChanges = false;

        void Awake()
        {
            applyChanges = false;
            UpdateVars();       
        }

        void Update()
        {
            if (applyChanges)
            {
                applyChanges = false;
                UpdateVars();
            }
        }

        void UpdateVars()
        {
            _oscillatorX = new Oscillator(-bobOffset.x, bobOffset.x);
            _oscillatorY = new Oscillator(-bobOffset.y, bobOffset.y);
            _oscillatorZ = new Oscillator(-bobOffset.z, bobOffset.z);

            _transform = transform;
            _camera = camera;
            baseFOV = _camera.fieldOfView;

            _upperBody = _transform.parent;
            _upperBody.localPosition = new Vector3(0, height, 0);
            _transform.localPosition = Vector3.zero;
        }

        public void Initialize(FPSControlPlayerMovement playerMovement, FPSControlPlayer player)
        {
            _playerMovement = playerMovement;
            Initialize(player);
        }

        override public void Initialize(FPSControlPlayer player)
        {
            base.Initialize(player);

            if (!weaponCamera) weaponCamera = new GameObject("Weapon Camera").AddComponent<Camera>();
            weaponCamera.transform.parent = transform;
            weaponCamera.transform.localPosition = new Vector3(0, 0, weaponCameraZOffset);
            weaponCamera.transform.localRotation = Quaternion.identity;
            weaponCamera.cullingMask = (int)weaponCameraCullingMask;
            weaponCamera.clearFlags = CameraClearFlags.Depth;
            weaponCamera.depth = weaponCameraDepth;
            weaponCamera.fieldOfView = weaponCameraFOV;
            weaponCamera.nearClipPlane = weaponCameraNearClip;
            weaponCamera.farClipPlane = weaponCameraFarClip;

            _root = player.transform;
            _playerPrevRotation = _startRotation = _root.rotation;
            _control = new GameObject("[PLAYER ROTATION CONTROL]").transform;
            _control.gameObject.hideFlags = HideFlags.HideInHierarchy;
            _control.position = _root.position;
            _control.rotation = _root.rotation;
        }

        public void SetVisibility(bool visible)
        {
            weaponCamera.enabled = visible;
        }

        public override void DoUpdate()
        {
            if (FPSControlPlayerData.frozen) return;
            
            //gather input, applying sensitivity and possible inverted Y, for those weiners out there who like to press down to look up.
            Vector2 lookInput = FPSControlInput.GetLookDirection();
            float mouseX = lookInput.x * mouseSensitivity.x;
            float mouseY = lookInput.y * (FPSControlInput.invertedLook ? -1 : 1) * mouseSensitivity.y;

            //legacy:
            //float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity.x;
            //float mouseY = Input.GetAxisRaw("Mouse Y") * ((invertedY) ? -1 : 1) * mouseSensitivity.y;

            //update values
            _yaw += mouseX;
            _pitch += mouseY;
            
            //correct rotational values and apply constraints
            _pitch += (_pitch < -360) ? 360 : 0;
            _pitch += (_pitch > 360) ? -360 : 0;
            _pitch = Mathf.Clamp(_pitch, pitchConstraints.x, pitchConstraints.y);
            _yaw += (_yaw < -360) ? 360 : 0;
            _yaw += (_yaw > 360) ? -360 : 0;
            //Debug.Log(_yaw + " : " + mouseX + ", " + _pitch + " : " + mouseY);

            //Handle Scoping
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, (_scope) ? _scopeFOV : baseFOV, Time.deltaTime * scopeSpeed);
            
            /*
            if (_scope)
            {
                if (Mathf.Abs(_scopeFOV - _camera.fieldOfView) < .01F) _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _scopeFOV, Time.deltaTime * scopeSpeed);
                else _camera.fieldOfView = _scopeFOV;
            }
            else
            {
                if (Mathf.Abs(baseFOV - _camera.fieldOfView) < .01F) _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,baseFOV,Time.deltaTime * scopeSpeed);
                else _camera.fieldOfView = baseFOV;
            }
            */
        }

        Vector3 BobCamera()
        {
            //we normalize our simple oscillator by the magnitude of our controller's velocity. This should result in a bob that is relative to our speed 
            CharacterController controller = _playerMovement.Controller;
            float magnitude = (controller.isGrounded) ? controller.velocity.sqrMagnitude : 0; // if we aren't grounded, no reason to bob

            magnitude = Mathf.Min(magnitude, 2);
            return Vector3.up * _oscillatorY.Normalize(Time.time) * magnitude;
        }

        public void CancelScope()
        {
            _scope = false;
            _camera.fieldOfView = baseFOV;
        }

        public void Scope(bool scope, float fov)
        {
            _scope = scope;
            _scopeFOV = fov;
        }

        override public void DoLateUpdate()
        {
            if (noise)
            {
                _lastNoiseUpdate += Time.deltaTime;
                //Vector3 noiseOffset = new Vector3(0, height, 0);
                
                if (_lastNoiseUpdate >= noiseUpdateInterval)
                {
                    
                    _appliedNoise = new Vector3(((Random.value * 2F) - 1F) * noiseAmount.x, ((Random.value * 2F) - 1F) * noiseAmount.y, ((Random.value * 2F) - 1F) * noiseAmount.z);
                    _lastNoiseUpdate = 0;
                }

                _noiseOffset = Vector3.Lerp(_noiseOffset, _appliedNoise, Time.deltaTime);
            }
            else
            {
                _noiseOffset = Vector3.zero;
            }

            _upperBody.localPosition = Vector3.Lerp(_upperBody.localPosition, new Vector3(0, height, 0) + BobCamera(), Time.deltaTime * 2);//we move the upper body up and down
            _transform.localPosition = Vector3.Lerp(_transform.localPosition, _noiseOffset + headPosition, Time.deltaTime * 2F); //and apply general noise to the camera

            //get rotational values back from our pitch/yaw data
            Quaternion pitch;// = Quaternion.AngleAxis(_pitch - _startPitch, Vector3.left);
            Quaternion yaw = Quaternion.AngleAxis(_yaw - _startYaw, Vector3.up);

            //gotta rotate the character's body
            pitch = Quaternion.AngleAxis(0, -_root.right);

            _root.rotation = pitch * yaw;
            
            //rotate our head
            pitch = Quaternion.AngleAxis(_pitch - _startPitch, -_root.right);
            _transform.rotation = pitch * yaw;

            //we do a lerp here, so the head appears to lead the body.
            //Vector3 prevCtrlRot = _control.eulerAngles;
            _control.rotation = Quaternion.Slerp(_control.rotation, _transform.rotation, Time.deltaTime * trackSpeed); //Quaternion.Lerp(_control.rotation, _transform.rotation, Time.deltaTime * trackSpeed);
            //diff = Vector3.Distance(prevCtrlRot, _control.eulerAngles);
            //if (diff < blendThreshold) _control.rotation = _transform.rotation;

            weaponHolder.rotation = Quaternion.Slerp(weaponHolder.rotation, _control.rotation, Time.deltaTime * trackSpeed);

            _playerPrevRotation = transform.localRotation;
            //Debug.Log(_transform.rotation);
        }
        /*
        float diff;

        void OnGUI()
        {
            GUILayout.Box("" + diff);
        }
        */
    }

    /** Just a simple oscillator */
    class Oscillator
    {
        float min;
        float max;

        static float Mod(float num, float div)
        {
            float ratio = num / div;
            return div * (ratio - Mathf.Floor(ratio));
        }

        public Oscillator(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float Range { get { return max - min; } }
        public float CycleLength { get { return 2 * Range; } }

        public float Normalize(float val)
        {
            float state = Mod(val - min, CycleLength);

            if (state > Range)
                state = CycleLength - state;

            return state + min;
        }
    }
}
