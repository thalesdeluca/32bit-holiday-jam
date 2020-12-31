using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ThrowableScript : MonoBehaviour {

  private Animator animator;
  private GameObject controller;

  private Rigidbody2D rigidbody;

  private float speed = 2;

  private bool broken = false;

  private float time = 0;

  private float brokenTime = 0.4f;

  private bool canBreak = false;

  private bool throwed = false;

  private const float VOLUME_HIT = 240;

  private GameObject obj;

  [SerializeField]
  private Sprite highlighted;

  private Sprite normal;

  [SerializeField]
  private GameObject pivot;

  // Start is called before the first frame update
  void Start() {
    animator = GetComponent<Animator>();
    controller = GameObject.Find("Player");
    PlayerMovement.updateVisible += CheckVisible;
    rigidbody = GetComponent<Rigidbody2D>();
    obj = this.transform.Find("Object").gameObject;
    normal = obj.GetComponent<SpriteRenderer>().sprite;

  }
  void OnDestroy() {
    PlayerMovement.updateVisible -= CheckVisible;
  }



  private void CheckVisible() {
    if (broken) {
      return;
    }
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
    if (throwed & !canBreak) {
      if (time <= brokenTime) {
        time += Time.deltaTime;

      } else {
        canBreak = true;
      }
    }
  }

  public void Select() {
    obj.GetComponent<SpriteRenderer>().sprite = highlighted;
    animator.Play("Selected");
  }

  public void Diselect() {
    if (obj != null || !throwed) {
      obj.GetComponent<SpriteRenderer>().sprite = normal;

      animator.Play("Idle");
    }

  }

  public void Throw(Vector2 direction) {
    if (!throwed) {
      pivot.gameObject.SetActive(false);
      Diselect();
      controller.GetComponent<ThrowableController>().UpdateObjects(this.gameObject, true);
      throwed = true;
      rigidbody.AddForce(direction * speed, ForceMode2D.Impulse);
    }

  }


  private void OnTriggerEnter2D(Collider2D other) {
    //Animation Break;
    if (throwed) {
      if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
        Debug.Log("Stun");
        other.gameObject.GetComponent<EnemyAI>().Stun();
      }
      broken = true;
      Break();
      GetComponent<NoiseGeneratorScript>().PlaySound(VOLUME_HIT);
    }
  }


  private void OnTriggerStay2D(Collider2D other) {

    if (canBreak) {

      //Animation Break;
      if (throwed) {
        broken = true;
        Break();
        GetComponent<NoiseGeneratorScript>().PlaySound(VOLUME_HIT);
      }
    }
  }

  public void Direction(Vector2 direction) {
    pivot.gameObject.SetActive(true);
    pivot.transform.rotation = Quaternion.Euler(45, 0, Vector2.Angle(Vector2.right, direction.normalized) * (direction.y <= 0 ? -1 : 1));
  }

  private void Break() {
    PlayerMovement.updateVisible -= CheckVisible;
    Destroy(this.gameObject);

  }
}
