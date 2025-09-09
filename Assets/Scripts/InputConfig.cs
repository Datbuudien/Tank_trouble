using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputConfig", menuName = "Tank/InputTankConfig")]
public class InputConfig : ScriptableObject
{
    [System.Serializable]
    public class PlayerInput
    {
        public KeyCode moveForward;
        public KeyCode moveBackward;
        public KeyCode rotateLeft;
        public KeyCode rotateRight;
        public KeyCode fire;
    }
    
    [Header("Player 1 Controls")]
    public PlayerInput player1 = new PlayerInput
    {
        moveForward = KeyCode.W,
        moveBackward = KeyCode.S,
        rotateLeft = KeyCode.A,
        rotateRight = KeyCode.D,
        fire = KeyCode.Q
    };
    
    [Header("Player 2 Controls")]
    public PlayerInput player2 = new PlayerInput
    {
        moveForward = KeyCode.UpArrow,
        moveBackward = KeyCode.DownArrow,
        rotateLeft = KeyCode.LeftArrow,
        rotateRight = KeyCode.RightArrow,
        fire = KeyCode.Keypad0
    };
    
    [Header("Player 3 Controls")]
    public PlayerInput player3 = new PlayerInput
    {
        moveForward = KeyCode.I,
        moveBackward = KeyCode.K,
        rotateLeft = KeyCode.J,
        rotateRight = KeyCode.L,
        fire = KeyCode.Keypad1
    };
    
    [Header("Player 4 Controls")]
    public PlayerInput player4 = new PlayerInput
    {
        moveForward = KeyCode.Keypad8,
        moveBackward = KeyCode.Keypad5,
        rotateLeft = KeyCode.Keypad4,
        rotateRight = KeyCode.Keypad6,
        fire = KeyCode.Keypad2
    };
}