using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableScript : MonoBehaviour {

  private Animator animator;
  private GameObject controller;

  private Rigidbody2D rigidbody;

  private float speed = 2;

  private bool broken = false;

  private bool throwed = false;

  private const float VOLUME_HIT = 200;

  // Start is called before the first frame update
  void Start() {
    animator = GetComponent<Animator>();
    controller = GameObject.Find("Player");
    PlayerMovement.updateVisible += CheckVisible;
    rigidbody = GetComponent<Rigidbody2D>();
  }


  void OnDestroy() {
    PlayerMovement.updateVisible -= CheckVisible;
  }

  private void CheckVisible() {
    if (broken) {
      return;
    }
    Debug.Log("CheckVisible");
    Vector2 direction = Camera.main.WorldToViewportPoint(this.transform.position);
    if (direction.x >= 0.02 && direction.x <= 0.98 && direction.y >= 0.02 && direction.y <= 0.98) {
      controller.GetComponent<ThrowableController>().UpdateObjects(this.gameObject, throwed);
    } else {
      controller.GetComponent<ThrowableController>().UpdateObjects(this.gameObject, true);
      Diselect();
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public void Select() {
    animator.Play("Selected");
  }

  public void Diselect() {
    animator.Play("Idle");
  }

  public void Throw(Vector2 direction) {
    if (!throwed) {
      Diselect();
      controller.GetComponent<ThrowableController>().UpdateObjects(this.gameObject, true);
      throwed = true;
      rigidbody.AddForce(direction * speed, ForceMode2D.Impulse);
    }

  }


  private void OnTriggerEnter2D(Collider2D other) {
    //Animation Break;
    if (throwed) {
      broken = true;
      Break();
      GetComponent<NoiseGeneratorScript>().PlaySound(VOLUME_HIT);

    }

  }

  private void Break() {
    Destroy(this.gameObject);

  }
}
