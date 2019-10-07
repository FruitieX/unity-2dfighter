using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
  void Update()
  {
    if (Gamepad.current.startButton.isPressed)
    {
      print("b");
      SceneManager.LoadScene("MainMenu");
    }
  }
}
