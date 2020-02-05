using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config parameters
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    // States variables

    // Cached component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    Collider2D myCollider2D;
    float gravityScaleAtStart;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider2D = GetComponent<Collider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
        FlipSprite();
        ClimbLadder();
    }

    private void Run() {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        myAnimator.SetBool("isRunning", isPlayerRunning());
    }

    private void ClimbLadder()
    {
        if (!myCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("isClimbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myAnimator.SetBool("isClimbing", isPlayerClimbing());
        myRigidBody.gravityScale = 0f;
    }

    private void Jump()
    {
        if (!myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity = myRigidBody.velocity + jumpVelocity;
        }
    }

    private void FlipSprite()
    {
        if (isPlayerRunning()) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x) * 2f, 2f);
        }
    }

    private bool isPlayerRunning()
    {
        return Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
    }

    private bool isPlayerClimbing()
    {
        return Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
    }
}
