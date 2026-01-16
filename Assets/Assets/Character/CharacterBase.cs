using UnityEngine;

public enum CharacterState
{
	Idle,
	Walk,
	Jump,
	Fall
}

public class CharacterBase : MonoBehaviour
{
	protected CharacterState m_state;
	protected CharacterController m_characterController;
	protected CharacterModel m_characterModel;

	[SerializeField] protected float m_health = 100.0f;
	[SerializeField] protected float m_maxHealth = 100.0f;

	[Header("Movement")]
	protected Vector3 m_momentum;
	[SerializeField] protected float m_acceleration;
	[SerializeField] protected float m_deceleration;
	[SerializeField] protected float m_decelerationSliding;
	[SerializeField] protected float m_airAcceleration;
	[SerializeField] protected float m_airDeceleration;
	[SerializeField] protected float m_topSpeed;
	[SerializeField] protected float m_maxSpeed;
	[SerializeField] protected float m_gravity = 9.81f;
	[SerializeField] protected float m_gravityFallMultiplier = 2;

	protected float m_current_acceleration;
	protected float m_current_deceleration;
	protected float m_current_airAcceleration;
	protected float m_current_airDeceleration;
	protected float m_current_gravity;

	[Header("Jump")]
	protected float m_jumpHeight = 0.0f;
	protected Vector3 m_jumpMomentum;
	[SerializeField] protected float m_minJumpHeight;
	[SerializeField] protected float m_maxJumpHeight;
	[SerializeField] protected float m_jumpForce;
	[SerializeField] protected float m_jump_deceleration;
	[SerializeField] protected int m_numJumps = 1;

	//
	protected float m_jumpsRemaining = 1;

	protected virtual void Start()
	{
		m_characterController = GetComponent<CharacterController>();
		m_characterModel = GetComponentInChildren<CharacterModel>();

		m_jumpsRemaining = m_numJumps;

		m_current_acceleration = m_acceleration;
		m_current_deceleration = m_deceleration;
		m_current_airAcceleration = m_airAcceleration;
		m_current_airDeceleration = m_airDeceleration;
		m_current_gravity = m_gravity;

	}

	protected virtual void Update()
	{

	}

	protected virtual void FixedUpdate()
	{

	}

	public void Damage(float amount)
	{
		m_health = Mathf.Clamp(m_health - amount, 0.0f, m_maxHealth);
	}

	public void SetHealth(float value)
	{
		m_health = Mathf.Clamp(value, 0.0f, m_maxHealth);
	}

	public Vector3 GetMomentum()
	{
		return m_momentum;
	}

	protected void Movement(Vector3 moveDirection)
	{
		Vector3 targetVelocity = moveDirection * m_topSpeed;

		// Get acceleration / deceleration value (ground or air)
		float acceleration = m_characterController.isGrounded ? m_current_acceleration : m_current_airAcceleration;
		float deceleration = m_characterController.isGrounded ? m_current_deceleration : m_current_airDeceleration;

		// Get momentum change value
		float maxDistanceDelta = moveDirection != Vector3.zero
			? acceleration
			: deceleration;

		m_momentum = Vector3.MoveTowards(m_momentum, targetVelocity, maxDistanceDelta * Time.fixedDeltaTime);
	}

	protected void Jump()
	{
		float targetForce = 0.0f;
		float jumpForceApplied = m_jumpForce * Time.fixedDeltaTime;
		float acceleration = m_current_acceleration * Time.fixedDeltaTime;

		if (m_state == CharacterState.Jump)
			m_jumpHeight = Mathf.MoveTowards(m_jumpHeight, m_maxJumpHeight, jumpForceApplied); // Track height gained during jump

		m_jumpMomentum = Vector3.MoveTowards(m_jumpMomentum, Vector3.up * targetForce, acceleration);
	}
}
