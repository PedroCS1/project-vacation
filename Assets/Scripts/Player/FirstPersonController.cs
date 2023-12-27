using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private bool _IsWalking;
    [SerializeField] private float _WalkSpeed;
    [SerializeField] private float _RunSpeed;
    [SerializeField][Range(0f, 1f)] private float _RunstepLenghten;
    [SerializeField] private float _JumpSpeed;
    [SerializeField] private float _StickToGroundForce;
    [SerializeField] private float _GravityMultiplier;
    [SerializeField] private MouseLook _MouseLook;
    [SerializeField] private bool _UseFovKick;
    [SerializeField] private FOVKick _FovKick = new FOVKick();
    [SerializeField] private bool _UseHeadBob;
    [SerializeField] private CurveControlledBob _HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob _JumpBob = new LerpControlledBob();
    [SerializeField] private float _StepInterval;
    [SerializeField] private AudioClip[] _FootstepSounds;
    [SerializeField] private AudioClip _JumpSound;
    [SerializeField] private AudioClip _LandSound;
    [SerializeField] private GameObject _PauseMenu;
    [SerializeField] private GameObject _PlayerCamera;

    private Camera _Camera;
    private bool _Jump;
    private float _YRotation;
    private Vector2 _Input;
    private Vector3 _MoveDir = Vector3.zero;
    private CharacterController _CharacterController;
    private CollisionFlags _CollisionFlags;
    private bool _PreviouslyGrounded;
    private Vector3 _OriginalCameraPosition;
    private float _StepCycle;
    private float _NextStep;
    private bool _Jumping;
    private bool _PlayerIsMoving = false;

    public bool IsWalking { get => _IsWalking; }
    public bool PlayerIsMoving { get => _PlayerIsMoving; }
    public float RunSpeed { get => _RunSpeed; set => _RunSpeed = value; }

    // Use this for initialization
    private void Start()
    {
        _CharacterController = GetComponent<CharacterController>();
        _Camera = Camera.main;
        _OriginalCameraPosition = _Camera.transform.localPosition;
        _FovKick.Setup(_Camera);
        _HeadBob.Setup(_Camera, _StepInterval);
        _StepCycle = 0f;
        _NextStep = _StepCycle / 2f;
        _Jumping = false;
        _MouseLook.Init(transform, _Camera.transform);
    }


    // Update is called once per frame
    private void Update()
    {
        RotateView();
        // the jump state needs to read here to make sure it is not missed
        if (!_Jump)
        {
            _Jump = Input.GetButtonDown("Jump");
        }

        if (!_PreviouslyGrounded && _CharacterController.isGrounded)
        {
            StartCoroutine(_JumpBob.DoBobCycle());
            PlayLandingSound();
            _MoveDir.y = 0f;
            _Jumping = false;
        }
        if (!_CharacterController.isGrounded && !_Jumping && _PreviouslyGrounded)
        {
            _MoveDir.y = 0f;
        }

        _PreviouslyGrounded = _CharacterController.isGrounded;
    }

    private void PlayLandingSound()
    {
        PlaySound(_LandSound);
        _NextStep = _StepCycle + .5f;
    }


    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * _Input.y + transform.right * _Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, _CharacterController.radius, Vector3.down, out hitInfo,
                           _CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        _MoveDir.x = desiredMove.x * speed;
        _MoveDir.z = desiredMove.z * speed;


        if (_CharacterController.isGrounded)
        {
            _MoveDir.y = -_StickToGroundForce;

            if (_Jump)
            {
                _MoveDir.y = _JumpSpeed;
                PlayJumpSound();
                _Jump = false;
                _Jumping = true;
            }
        }
        else
        {
            _MoveDir += Physics.gravity * _GravityMultiplier * Time.fixedDeltaTime;
        }
        _CollisionFlags = _CharacterController.Move(_MoveDir * Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);

        _MouseLook.UpdateCursorLock();
    }


    private void PlayJumpSound()
    {
        PlaySound(_JumpSound);
    }

    private void ProgressStepCycle(float speed)
    {
        if (_CharacterController.velocity.sqrMagnitude > 0 && (_Input.x != 0 || _Input.y != 0))
        {
            _StepCycle += (_CharacterController.velocity.magnitude + (speed * (IsWalking ? 1f : _RunstepLenghten))) *
                         Time.fixedDeltaTime;
            _PlayerIsMoving = true;
        }
        else
        {
            _PlayerIsMoving = false;
        }

        if (!(_StepCycle > _NextStep))
        {
            return;
        }

        _NextStep = _StepCycle + _StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!_CharacterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, _FootstepSounds.Length);
        PlaySound(_FootstepSounds[n]);
        // move picked sound to index 0 so it's not picked next time
        _FootstepSounds[n] = _FootstepSounds[0];
    }

    private static void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
        {
            ServiceManager.Instance.GameAudio.Play(clip);
        }
    }

    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!_UseHeadBob)
        {
            return;
        }
        if (_CharacterController.velocity.magnitude > 0 && _CharacterController.isGrounded)
        {
            _Camera.transform.localPosition =
                _HeadBob.DoHeadBob(_CharacterController.velocity.magnitude +
                                  (speed * (IsWalking ? 1f : _RunstepLenghten)));
            newCameraPosition = _Camera.transform.localPosition;
            newCameraPosition.y = _Camera.transform.localPosition.y - _JumpBob.Offset();
        }
        else
        {
            newCameraPosition = _Camera.transform.localPosition;
            newCameraPosition.y = _OriginalCameraPosition.y - _JumpBob.Offset();
        }
        _Camera.transform.localPosition = newCameraPosition;
    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool waswalking = IsWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        _IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
        // set the desired speed to be walking or running
        speed = !IsWalking ? _RunSpeed : _WalkSpeed;
        _Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (_Input.sqrMagnitude > 1)
        {
            _Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (IsWalking != waswalking && _UseFovKick && _CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!IsWalking ? _FovKick.FOVKickUp() : _FovKick.FOVKickDown());
        }
    }

    private void RotateView()
    {
        _MouseLook.LookRotation(transform, _Camera.transform);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
}

