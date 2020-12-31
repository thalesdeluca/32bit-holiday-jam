using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour {
  // Start is called before the first frame update
  private EnemyAI ai;

  public bool onRange { get; private set; }

  void Start() {
    ai = this.transform.parent.GetComponent<EnemyAI>();
    onRange = false;
  }
  private void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !onRange) {
      onRange = true;
      ai.TriggerHit();

    }
  }

  private void OnTriggerStay2D(Collider2D other) {
    if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
      onRange = true;
      ai.TriggerHit();

    }
  }

  private void OnTriggerExit2D(Collider2D other) {
    if (other.gameObject.layer == LayerMask.NameToLayer("Player") && onRange) {
      onRange = false;
    }
  }
}
