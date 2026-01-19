using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Data/GameState")]
public class GameState : ScriptableObject
{
	public bool isMouseVisible;
	public bool isGamePaused;
	public InputActionMaps inputActionMap;
}

public enum InputActionMaps
{
	Player,
	UI
}