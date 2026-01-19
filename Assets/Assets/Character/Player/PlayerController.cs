using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
	private PlayerInput m_playerInput;
	[SerializeField] private Transform m_camera;

	[SerializeField] protected float m_jumpCoyoteTime;
	private float m_timeSinceGrounded = 0.0f;

	[Header("Camera")]
	[SerializeField] private float m_camera_xMaximum;
	[SerializeField] private float m_camera_xMinimum;

	protected override void Start()
	{
		base.Start();
		m_playerInput = GameStateManager.m_instance.m_playerInput;
	}

	protected override void Update()
	{
		HandleCamera();
	}

	protected override void FixedUpdate()
	{
		if (!m_characterController.isGrounded)
			m_timeSinceGrounded += Time.fixedDeltaTime;

		Vector2 moveInput = GetInputActionValue<Vector2>("Move");
		bool jumpInput = GetInputActionButton("Jump");
		bool slideInput = GetInputActionButton("Sprint");

		// slide
		m_current_deceleration = slideInput ? m_decelerationSliding : m_deceleration;

		// land
		if (m_characterController.isGrounded)
		{
			m_jumpsRemaining = m_numJumps;
		}

		// initiate jump
		if (jumpInput && CanJump())
		{
			m_state = CharacterState.Jump;
			m_jumpHeight = 0.0f;
			m_jumpsRemaining--;
			m_jumpMomentum = Vector3.up * m_jumpForce;
		}

		// handle jump mid-air
		if (!m_characterController.isGrounded)
		{
			if (jumpInput && m_state != CharacterState.Fall) // stay in jump state until button released or max height reached
			{
				m_state = m_jumpHeight < m_maxJumpHeight
					? CharacterState.Jump
					: CharacterState.Fall;
			}
			else if (m_jumpHeight >= m_minJumpHeight) // if jump released, start falling
			{
				m_state = CharacterState.Fall;
			}
		}

		Vector3 moveInputRelativeToCamera = ((moveInput.x * m_camera.transform.right) + (moveInput.y * m_camera.transform.forward));

		Movement(moveInputRelativeToCamera);
		Jump();

		Vector3 gravityForce = Vector3.down * m_current_gravity;
		Vector3 velocity = (m_momentum + m_jumpMomentum + gravityForce) * Time.fixedDeltaTime;
		m_characterController.Move(velocity);

		if (moveInput == Vector2.zero)
			m_characterModel.SetRotation(m_characterController.isGrounded);
		else
			m_characterModel.SetRotation(m_characterController.isGrounded, moveInputRelativeToCamera.normalized);
	}

	private bool CanJump()
	{
		return (m_characterController.isGrounded || m_timeSinceGrounded < m_jumpCoyoteTime) && m_jumpsRemaining > 0;
	}

	private void HandleCamera()
	{
		Vector2 lookInput = GetInputActionValue<Vector2>("Look");
		float xRotation = Mathf.Clamp(lookInput.x, m_camera_xMinimum, m_camera_xMaximum);


	}

	public T GetInputActionValue<T>(string inputActionName) where T : struct
	{
		if (m_playerInput == null)
			m_playerInput = GetComponentInParent<PlayerInput>();

		InputAction action = m_playerInput.currentActionMap.FindAction(inputActionName);
		if (action == null)
		{
			Debug.LogWarning("InputAction '" + inputActionName + "' does not exist in player input map.");
			return new T();
		}

		if (!action.enabled)
		{
			// Debug potential here.
			action.Enable();
		}

		return action.ReadValue<T>();
	}

	public bool GetInputActionButton(string inputActionName)
	{
		return GetInputActionValue<float>(inputActionName) != 0.0f;
	}
}