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


  private float stepTime = 0;

  private const float VOLUME_STEPS = 120;

  private const float RUNNING_THRESHOLD = 0.5f;

  private float stepMaxTime = 0.35f;
  // Start is called before the first frame update
  void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    stepTime = 0;

  }

  // Update is called once per frame
  void Update() {

    if (blocked) {

      return;
    }

    var horizontal = Input.GetAxisRaw("Horizontal");
    var vertical = Input.GetAxisRaw("Vertical");

    Vector2 movement = Vector2.zero;
    rigidbody.velocity = Vector2.zero;

    movement.x = horizontal;
    movement.y = vertical;

    if (movement != Vector2.zero) {
      if (updateVisible.GetInvocationList().Length > 0) {
        updateVisible();
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

  }


  public void Blocked() {
    blocked = true;
  }

  public void Blocked(float time) {
    this.time = time;
  }

  public void Blocked(bool block) {
    blocked = block;
  }
}
