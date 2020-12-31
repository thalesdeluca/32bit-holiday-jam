using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
  [SerializeField]
  private float speed = 2f;

  private Rigidbody2D rigidbody;

  private float time = 0;


  public delegate void UpdateVisible();
  public static event UpdateVisible updateVisible;


  private bool blocked = false;
  private bool blockedTime = false;


  private float stepTime = 0;

  private const float VOLUME_STEPS = 85;

  private const float RUNNING_THRESHOLD = 0.5f;

  private float stepMaxTime = 0.25f;
  private Animator animator;

  private SpriteRenderer sprite;
  // Start is called before the first frame update
  void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    stepTime = 0;
    animator = GetComponent<Animator>();
    sprite = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();

  }

  // Update is called once per frame
  void Update() {

    if (blocked) {
      rigidbody.velocity = Vector2.zero;

      if (blockedTime) {
        if (time >= 0) {
          time -= Time.deltaTime;
        } else {
          blocked = false;
          blockedTime = false;
          time = 0;
        }
      }
      return;
    }

    var horizontal = Input.GetAxisRaw("Horizontal");
    var vertical = Input.GetAxisRaw("Vertical");

    Vector2 movement = Vector2.zero;
    rigidbody.velocity = Vector2.zero;

    movement.x = horizontal;
    movement.y = vertical;

    if (movement != Vector2.zero) {
      if (updateVisible != null) {
        if (updateVisible.GetInvocationList().Length > 0) {
          updateVisible();
        }
      }


      if (Mathf.Abs(horizontal) > RUNNING_THRESHOLD || Mathf.Abs(vertical) > RUNNING_THRESHOLD) {
        if (stepTime >= stepMaxTime) {
          GetComponent<NoiseGeneratorScript>().PlaySound(VOLUME_STEPS);
        } else {
          stepTime += Time.deltaTime;
        }
      }
    } else {
      stepTime = 0;
    }


    // Debug.Log("Teste" + movement);
    rigidbody.velocity = movement * speed;

    if (movement.x > 0) {
      sprite.flipX = true;
      animator.Play("MoveHorizontal");
    } else if (movement.x < 0) {
      sprite.flipX = false;
      animator.Play("MoveHorizontal");
    } else {
      sprite.flipX = false;
      if (movement.y > 0) {
        animator.Play("MoveVertical");
      } else {
        animator.Play("Idle");
      }
    }



  }


  public void Blocked() {
    rigidbody.velocity = Vector2.zero;
    blocked = true;
  }

  public void Blocked(float time) {
    rigidbody.AddForce(rigidbody.velocity.normalized * -1, ForceMode2D.Impulse);
    rigidbody.velocity = Vector2.zero;
    blocked = true;
    blockedTime = true;
    this.time = time;
  }

  public void Blocked(bool block) {
    rigidbody.velocity = Vector2.zero;
    blocked = block;
  }
}
