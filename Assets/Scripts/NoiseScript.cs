using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseScript : MonoBehaviour {
  [SerializeField]
  private AudioClip clip;

  [SerializeField]
  private float volume;

  private bool triggered = false;

  private float radius;

  // Start is called before the first frame update
  void Start() {
    volume /= 100f;

    triggered = true;
  }

  // Update is called once per frame
  void Update() {
    if (triggered) {

      radius += Time.deltaTime * 4;
      if (radius >= volume * 1.10) {
        triggered = false;
        Destroy(this.gameObject);
        radius = 0;
        return;
      } else if (radius >= volume) {

      } else {
        Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, radius / 2);
        this.transform.localScale = new Vector3(radius, radius, 1);


        foreach (Collider2D hit in hits) {

          MonoBehaviour[] scripts = hit.gameObject.GetComponents<MonoBehaviour>();
          foreach (var script in scripts) {
            if (script is NoiseDetectionScript) {
              NoiseDetectionScript detection = (NoiseDetectionScript)script;
              detection.Hear(this.transform.position);
            }
          }
        }
      }

    }
  }

  public void SetVolume(float volume) {
    this.volume = volume;
  }
}