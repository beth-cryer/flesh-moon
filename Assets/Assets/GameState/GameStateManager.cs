using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
	public static GameStateManager m_instance;
	private GameState m_state;
	public PlayerInput m_playerInput { get; private set; }

	[SerializeField] private GameState m_defaultState;

	private void Start()
	{
		EnterState(m_defaultState);

		Cursor.lockState = CursorLockMode.Locked;
	}

	public void EnterState(GameState newState)
	{
		m_state = newState;

		Cursor.visible = m_state.isMouseVisible;
		Time.timeScale = m_state.isGamePaused ? 0.0f : 1.0f;

		string actionMap = m_state.inputActionMap switch
		{
			InputActionMaps.Player => "Player",
			InputActionMaps.UI => "UI",
			_ => "Player"
		};

		m_playerInput.SwitchCurrentActionMap(actionMap);
	}

	private void Awake()
	{
		if (m_instance == null)
			m_instance = this;
		else
			Destroy(this);

		m_playerInput = GetComponent<PlayerInput>();
	}
}
