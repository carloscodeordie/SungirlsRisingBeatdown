﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config parameters
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    // States variables
    bool isAlive = true;

    // Cached component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeetCollider2D;
    float gravityScaleAtStart;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Run();
        Jump();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    private void Run() {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        myAnimator.SetBool("isRunning", isPlayerRunning());
    }

    private void ClimbLadder()
    {
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
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
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
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

    private void Die()
    {
        if (!myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            return;
        }

        myAnimator.SetTrigger("isDeath");
        myRigidBody.velocity = deathKick;
        isAlive = false;
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
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
