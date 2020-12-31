using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour {
  [SerializeField]
  private string nextScene;


  private void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.layer == LayerMask.NameToLayer("Player") && nextScene != null) {
      SceneManager.LoadScene(nextScene);
    }
  }
}
