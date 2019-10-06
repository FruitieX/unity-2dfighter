using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

static class PhysicsConstants
{
  public const float jumpVel = 5.0f;
  public const float moveVel = 3.0f;
}

public class Player : MonoBehaviour
{
  public int playerId;
  private Rigidbody2D body;
  private Gamepad gamepad;

  void Start()
  {
    body = this.GetComponent<Rigidbody2D>();
  }

  float GetXVelocity(Vector2 xyInput)
  {    
    if (xyInput.x > 0)
    {
      return PhysicsConstants.moveVel;
    }
    else if (xyInput.x < 0)
    {
      return -PhysicsConstants.moveVel;
    }

    return 0;
  }

  float GetYVelocity(Vector2 xyInput)
  {
    if (xyInput.y > 0)
    {
      return PhysicsConstants.moveVel;
    }
    else if (xyInput.y < 0)
    {
      return -PhysicsConstants.moveVel;
    }
    return body.velocity.y;
  }

  // called once per physics frame
  void FixedUpdate()
  {
    gamepad = Gamepad.current;


    if (gamepad == null)
    {
      return; // No gamepad connected.
    }
    else
    {
      Vector2 xyInput = gamepad.leftStick.ReadValue() + gamepad.dpad.ReadValue();
      body.velocity = new Vector2(this.GetXVelocity(xyInput), this.GetYVelocity(xyInput));

      if (gamepad.bButton.isPressed)
      {
        print("b");
        SceneManager.LoadScene("MainMenu");
      }
    }
  }
}

