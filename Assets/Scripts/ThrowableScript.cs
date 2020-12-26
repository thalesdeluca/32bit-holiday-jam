﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableScript : MonoBehaviour {

  private Animator animator;
  private GameObject controller;
  // Start is called before the first frame update
  void Start() {
    animator = GetComponent<Animator>();
    controller = GameObject.Find("Player");
    PlayerMovement.updateVisible += CheckVisible;
  }

  void OnDestroy() {
    PlayerMovement.updateVisible -= CheckVisible;

  }

  private void CheckVisible() {
    Vector2 direction = Camera.main.WorldToViewportPoint(this.transform.position);
    if (direction.x >= 0 && direction.x <= 1 && direction.y >= 0 && direction.y <= 1) {
      controller.GetComponent<ThrowableController>().UpdateObjects(this.gameObject, false);
    } else {
      controller.GetComponent<ThrowableController>().UpdateObjects(this.gameObject, true);
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public void Select() {
    animator.Play("Selected");
  }

  public void Diselect() {
    animator.StopPlayback();
  }
}