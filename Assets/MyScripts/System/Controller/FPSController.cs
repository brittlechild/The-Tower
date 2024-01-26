using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class FPSController : MonoBehaviour
{
    public bool canMove;

    public static FPSController Instance { get; private set; }

    [SerializeField] private bool isWalking;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField][Range(0f, 1f)] private float runstepLengthen;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float stickToGroundForce;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private MouseLook mouseLook;

    private Camera playerCamera;
    private bool isJumping;
    private Vector2 input;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private bool previouslyGrounded;
    private float stepCycle;
    private float nextStep;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canMove = true;

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        mouseLook.Init(transform, playerCamera.transform);
        stepCycle = 0f;
        nextStep = stepCycle / 2f;
    }

    // Subscribe to and unsubscribe from inventory events
    private void OnEnable()
    {
        EventBus.Instance.onGameplayPaused += () => canMove = false;
        EventBus.Instance.onGameplayResumed += () => canMove = true;
    }

    private void OnDisable()
    {
        EventBus.Instance.onGameplayPaused -= () => canMove = false;
        EventBus.Instance.onGameplayResumed -= () => canMove = true;
    }

    private void Update()
    {
        RotateView();

        // Check for jump input
        if (!isJumping)
        {
            isJumping = Input.GetButtonDown("Jump");
        }

        // Handle character grounding status
        if (!previouslyGrounded && characterController.isGrounded)
        {
            //PlayLandingSound();
            moveDirection.y = 0f;
            isJumping = false;
        }

        if (!characterController.isGrounded && !isJumping && previouslyGrounded)
        {
            moveDirection.y = 0f;
        }

        previouslyGrounded = characterController.isGrounded;
    }
    private void FixedUpdate()
    {
        // Check if movement is allowed
        if (canMove == false) return;

        float speed;
        GetInput(out speed);

        // Calculate desired movement direction
        Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                           characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        // Update movement direction based on input and ground conditions
        moveDirection.x = desiredMove.x * speed;
        moveDirection.z = desiredMove.z * speed;

        if (characterController.isGrounded)
        {
            // Apply stick to ground force and handle jumping
            moveDirection.y = -stickToGroundForce;

            if (isJumping)
            {
                StartCoroutine(JumpCoroutine());
                isJumping = false;
            }
        }
        else
        {
            // Apply gravity when not grounded
            moveDirection += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }

        // Move the character controller
        characterController.Move(moveDirection * Time.fixedDeltaTime);

        // Update mouse look and cursor lock
        mouseLook.UpdateCursorLock();
    }

    private IEnumerator JumpCoroutine()
    {
        float timeInAir = 0f;

        while (timeInAir < 0.5f)  // Adjust the time as needed for a satisfactory jump height
        {
            moveDirection.y = jumpSpeed;
            timeInAir += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        moveDirection.y = 0f;  // Ensure that the jump force is reset after the coroutine
    }

    //private void PlayJumpSound()
    //{
    //    audioSource.PlayOneShot(audioSource.clip);
    //}

    //private void PlayLandingSound()
    //{
    //    audioSource.PlayOneShot(audioSource.clip);
    //    nextStep = stepCycle + 0.5f;
    //}

    // Get player input and calculate speed
    private void GetInput(out float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool wasWalking = isWalking;

        isWalking = !Input.GetKey(KeyCode.LeftShift);

        speed = isWalking ? walkSpeed : runSpeed;
        input = new Vector2(horizontal, vertical);

        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }

        if (isWalking != wasWalking && characterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
        }
    }

    // Rotate the player's view using mouse input
    private void RotateView()
    {
        //if (canMove == false) return;
        mouseLook.LookRotation(transform, playerCamera.transform);
    }
}
