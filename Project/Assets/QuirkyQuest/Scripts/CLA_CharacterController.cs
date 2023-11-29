using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CLA_CharacterController : MonoBehaviour, ITF_CharacterActions
{
    [Header("External Dependencies")]
    public Camera camera_;
    public GameObject mesh_;

    [Header("Components")]
    private CharacterController characterController_;
    private Animator animator_;

    [Header("Action Keys")]
    // public KeyCode swimKey_; // unassignable action
    public KeyCode runKey_;
    // public KeyCode walkKey_; // unassignable action
    public KeyCode rollKey_;
    public KeyCode flyKey_;
    public KeyCode eatKey_;
    // public KeyCode dieKey_ // unassingable action
    public KeyCode attackKey_;
    // public KeyCode clickKey_; // unassingable action
    public KeyCode jumpKey_;

    [Header("Axis Handles")]
    public string horizontalMovement_;
    public string verticalMovement_;
    public string mouseHorizontal_;
    public string mouseVertical_;

    [Header("Character Data")]
    public SOB_CharacterData characterData_;

    // Private Properties
    private float currentJump_;
    private bool isGrounded_;
    private bool isSwimming_;
    private bool isMoving_;
    private bool isFlying_;
    private Vector3 direction_;
    private Vector3 velocity_;
    private float speedMultiplier_;

    private void Awake()
    {
        Input.ResetInputAxes();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        characterController_ = TryGetComponent(out CharacterController cc) ? cc : null;
        animator_ = transform.childCount == 0 ? null : transform.GetChild(0).TryGetComponent(out Animator a) ? a : null;

        if (characterController_ == null || animator_ == null)
            Debug.Break();
    }

    private void Update()
    {
        speedMultiplier_ = Input.GetKey(runKey_) ? characterData_.runningSpeed_ : characterData_.walkingSpeed_;

        isSwimming_ = Physics.Raycast(transform.position + Vector3.up * 0.05f, transform.TransformDirection(Vector3.down),
            out RaycastHit floatHit, 0.25f, characterData_.waterLayers_, QueryTriggerInteraction.Ignore);

        isGrounded_ = Physics.Raycast(transform.position + Vector3.up * 0.05f, transform.TransformDirection(Vector3.down),
            out RaycastHit groundHit, 0.25f, characterData_.groundLayers_, QueryTriggerInteraction.Ignore);

        

        animator_.SetBool("isGrounded", isGrounded_ && !isSwimming_);

        ///////////////////////////////////////////////////////////////////////

        direction_ = new Vector3(Input.GetAxisRaw(horizontalMovement_), 0.0f, Input.GetAxisRaw(verticalMovement_));
        direction_ = camera_.transform.TransformDirection(direction_);
        direction_.y = 0.0f;
        direction_.Normalize();

        if (!isGrounded_) currentJump_ += characterData_.gravityPull_ * Time.deltaTime;

        ///////////////////////////////////////////////////////////////////////
        
        // Self-filtered actions
        Walk();
        Fly();
        Swim();
        Run();
        Roll();

        // User-filtered actions
        if (Input.GetKeyDown(eatKey_)) Eat();
        if (Input.GetKeyDown(attackKey_)) Attack();
        if (Input.GetKeyDown(jumpKey_)) Jump();

        ///////////////////////////////////////////////////////////////////////

        velocity_ = new Vector3(direction_.x * speedMultiplier_, currentJump_, direction_.z * speedMultiplier_) * Time.deltaTime;
        characterController_.Move(velocity_);

        isMoving_ = new Vector2(direction_.x, direction_.y).magnitude != 0.0f;

        ///////////////////////////////////////////////////////////////////////

        transform.Rotate(720.0f * Time.deltaTime * new Vector3(0.0f, Input.GetAxisRaw(mouseHorizontal_), 0.0f), Space.Self);
        mesh_.transform.forward = Vector3.Dot(mesh_.transform.forward, direction_) < 0.995f ?
            Vector3.RotateTowards(mesh_.transform.forward, direction_, Mathf.PI * 2.0f * Time.deltaTime, 0.0f) : direction_;
    }

    public bool Swim()
    {
        mesh_.transform.position = isSwimming_ ?
            transform.position + Vector3.down * 0.25f : transform.position;
        speedMultiplier_ = isSwimming_ ? characterData_.swimmingSpeed_ : speedMultiplier_;
        animator_.SetBool("isSwimming", isSwimming_);
        return isSwimming_;
    }

    public bool Run()
    {
        bool keyPressed = Input.GetKey(runKey_);
        speedMultiplier_ = (isGrounded_ && keyPressed) ? characterData_.runningSpeed_ : speedMultiplier_;
        animator_.SetBool("isRunning", keyPressed && isMoving_ && isGrounded_);
        return keyPressed;
    }

    public bool Walk()
    {
        speedMultiplier_ = (isMoving_ && isGrounded_) ? characterData_.walkingSpeed_ : speedMultiplier_;
        animator_.SetBool("isWalking", isMoving_ && isGrounded_);
        return isMoving_;
    }

    public bool Roll()
    {
        bool keyPressed = Input.GetKey(rollKey_);
        speedMultiplier_ = (keyPressed && isGrounded_) ? characterData_.rollingSpeed_ : speedMultiplier_;
        animator_.SetBool("isRolling", keyPressed && isMoving_ && isGrounded_);
        return keyPressed;
    }

    public bool Fly()
    {
        isFlying_ = (isGrounded_ && velocity_.y < 0.0f) ? false : true;
        speedMultiplier_ = (isFlying_) ? characterData_.flyingSpeed_ : speedMultiplier_;
        animator_.SetBool("isFlying", isFlying_);
        return isFlying_;    
    }

    public void Eat()
    {
        animator_.SetTrigger("toEat");
    }

    public void Die()
    {
        animator_.SetTrigger("toDie");
    }

    public void Attack()
    {
        animator_.SetTrigger("toAttack");
    }

    public void Click()
    {
        animator_.SetTrigger("toClick");
    }

    public void Jump()
    {
        if (!isGrounded_) return;

        currentJump_ = characterData_.jumpStrength_;
        animator_.SetTrigger("toJump");
        animator_.SetBool("isFlying", true);
    }
}
