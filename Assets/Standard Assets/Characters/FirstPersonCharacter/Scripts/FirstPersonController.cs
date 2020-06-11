using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed = 5;
        [SerializeField] private float m_RunSpeed = 8;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.7f;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private float _playerSlowdownInterval;
        [SerializeField] private float _targetRunningSpeed;
        [SerializeField] private float _targetWalkingSpeed;
        [SerializeField] private float _targetBobHighCurve;
        [SerializeField] private float _targetRunstep;
        [SerializeField] private bool _isRunningDisabled;
        [SerializeField] private bool _playerSlowingDown;
        
        private Camera m_Camera;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private float m_StepCycle;
        private float m_NextStep;
        private AudioSource m_AudioSource;
        private bool _stopFovKick;
        private bool _startFovKick = true;
        private bool _receiveInput = true;
        private float _mathRunningVelocity;
        private float _mathWalkingVelocity;
        private float _mathBobVelocity;
        private float _mathRunstepVelocity;

        public static event EventHandler<GameObject> FpsControllerAwokeEvent;

        private void Awake()
        {
            OnFpsControllerAwokeEvent();
        }

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            if (m_UseFovKick && _startFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                _startFovKick = false;
                StartCoroutine(m_FovKick.FOVKickUp());
            }

            if (m_UseFovKick && _stopFovKick)
            {
                _stopFovKick = false;
                StopAllCoroutines();
                StartCoroutine(m_FovKick.FOVKickDown());
            }

            if (!_receiveInput) return;

            if (_playerSlowingDown)
            {
                SlowdownPlayer();
            }

            RotateView();

            //if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            //{
            //    StartCoroutine(m_JumpBob.DoBobCycle());
            //    PlayLandingSound();
            //    m_MoveDir.y = 0f;
            //    m_Jumping = false;
            //}
            //if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            //{
            //    m_MoveDir.y = 0f;
            //}
        }

        private void SlowdownPlayer()
        {
            m_RunSpeed = Mathf.SmoothDamp(m_RunSpeed, _targetRunningSpeed, ref _mathRunningVelocity, _playerSlowdownInterval);
            m_WalkSpeed = Mathf.SmoothDamp(m_WalkSpeed, _targetWalkingSpeed, ref _mathWalkingVelocity,
                _playerSlowdownInterval);
            m_RunstepLenghten = Mathf.SmoothDamp(m_RunstepLenghten, _targetRunstep, ref _mathRunstepVelocity,
                _playerSlowdownInterval);

            var currentFrameValue = m_HeadBob.Bobcurve.keys[1].value;
            currentFrameValue = Mathf.SmoothDamp(currentFrameValue, _targetBobHighCurve,
                ref _mathBobVelocity, _playerSlowdownInterval);

            var newFrame = new Keyframe(0.5f, currentFrameValue);
            m_HeadBob.Bobcurve.MoveKey(1, newFrame);
        }

        private void FixedUpdate()
        {
            
            if (!_receiveInput) return;

            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }

        public void StopInput()
        {
            _receiveInput = false;
        }

        public void StopFovKick() => _stopFovKick = true;
        
        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            //PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            //bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running

            speed = _isRunningDisabled ? m_WalkSpeed : m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            //if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            //{
            //    StopAllCoroutines();
            //    StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            //}
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        private void OnFpsControllerAwokeEvent()
        {
            FpsControllerAwokeEvent?.Invoke(this, gameObject);
        }
    }
}
