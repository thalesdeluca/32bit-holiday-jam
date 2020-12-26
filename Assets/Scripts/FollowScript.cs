using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour {

  public Transform target;
  [SerializeField]
  private float speed = 4f;

  void Start() {

  }

  void Update() {
    if (target) {
      Vector3 newPosition = new Vector3(target.position.x, target.position.y, this.transform.position.z);
      this.transform.position = Vector3.Lerp(newPosition, this.transform.position, Time.deltaTime * speed);
    }
  }
}