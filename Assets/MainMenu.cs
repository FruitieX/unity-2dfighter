using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    print("Welcome to main menu");
  }

  // Update is called once per frame
  void Update()
  {
    if (Gamepad.current.aButton.isPressed)
    {
      SceneManager.LoadScene("InGame");
    }
  }
}
