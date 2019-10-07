using UnityEngine;
using UnityEngine.InputSystem;

static class PlayerPhysics
{
  public const float jumpForce = 30.0f;
  public const float jumpForceDecayRate = 3.0f;
  public const float jumpSpeed = 6.0f;
  public const int jumpCount = 1; // >1 = allow double-jumping

  public const float moveForce = 40.0f;
  public const float maxSpeed = 6.0f;

  public const int dashCount = 2; // >1 = allow double-dashing
  public const float dashCooldown = 1;
  public const float dashSpeed = 10.0f;
}

public class Player : MonoBehaviour
{
  public int playerId;
  private Rigidbody2D body;
  private Gamepad gamepad;

  private int jumpCount = PlayerPhysics.jumpCount;
  private bool jumpLastFrame = false;
  private bool isJumping = false;
  private float jumpTime = 0;

  private int dashCount = 0;
  private bool dashLastFrame = false;
  private float dashTime = -PlayerPhysics.dashCooldown;

  void Start()
  {
    body = this.GetComponent<Rigidbody2D>();
    gamepad = Gamepad.current;
  }

  Vector2 GetXYInput()
  {
    // TODO: normalize / snap to 8 directional input?
    Vector2 xyInput = gamepad.leftStick.ReadValue() + gamepad.dpad.ReadValue();

    return new Vector2(
      // don't go twice as fast if holding both dpad and analog stick right :-)
      Mathf.Max(-1, Mathf.Min(1, xyInput.x)),
      Mathf.Max(-1, Mathf.Min(1, xyInput.y))
    );
  }

  void HandleHorizontalMovement()
  {
    Vector2 xyInput = this.GetXYInput();

    float wishDir = 0;

    if (xyInput.x > 0) wishDir = PlayerPhysics.moveForce;
    else if (xyInput.x < 0) wishDir = -PlayerPhysics.moveForce;

    if ((wishDir > 0 && this.body.velocity.x < PlayerPhysics.maxSpeed) ||
        (wishDir < 0 && this.body.velocity.x > -PlayerPhysics.maxSpeed))
    {
      this.body.AddForce(new Vector2(wishDir, 0));
    }
  }

  void HandleJump()
  {
    if (this.jumpLastFrame && jumpCount < PlayerPhysics.jumpCount)
    {
      jumpCount++;
      this.isJumping = true;
      this.jumpTime = Time.time;

      this.body.velocity = new Vector2(this.body.velocity.x, PlayerPhysics.jumpSpeed);
    }

    float dt = Time.time - this.jumpTime;
    float decayedJumpForce = PlayerPhysics.jumpForce * (1 - PlayerPhysics.jumpForceDecayRate * dt);

    if (this.isJumping && gamepad.buttonSouth.isPressed && decayedJumpForce > 0)
    {
      this.body.AddForce(new Vector2(0, decayedJumpForce));
    }
    else
    {
      this.isJumping = false;
    }

    this.jumpLastFrame = false;
  }

  void HandleDash()
  {
    float dt = Time.time - this.dashTime;

    if (this.dashLastFrame && dashCount < PlayerPhysics.dashCount && dt > PlayerPhysics.dashCooldown)
    {
      dashCount++;
      this.dashTime = Time.time;

      this.body.velocity = this.GetXYInput() * PlayerPhysics.dashSpeed;
    }

    this.dashLastFrame = false;
  }

  void HandlePlayerPhysics()
  {
    this.HandleDash();
    this.HandleHorizontalMovement();
    this.HandleJump();
  }

  // called once per physics frame so our physics run the same independent of framerate
  void FixedUpdate()
  {
    this.HandlePlayerPhysics();
  }

  void Update()
  {
    // query jump & dash button state every frame or we will miss wasPressed events
    this.jumpLastFrame = this.jumpLastFrame || gamepad.buttonSouth.wasPressedThisFrame;
    this.dashLastFrame = this.dashLastFrame || gamepad.buttonWest.wasPressedThisFrame;
  }

  void OnCollisionStay2D(Collision2D collision)
  {
    // player is on the ground
    if (collision.gameObject.tag == "Floor")
    {
      this.dashCount = 0;
    }
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    // player touched the ground
    if (collision.gameObject.tag == "Floor")
    {
      this.jumpCount = 0;

      // always allow another dash immediately after landing
      this.dashTime = -PlayerPhysics.dashCooldown;
    }
  }
}

