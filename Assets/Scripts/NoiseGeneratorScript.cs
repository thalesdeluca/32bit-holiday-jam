using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGeneratorScript : MonoBehaviour {
  [SerializeField]
  private GameObject soundPrefab;

  private float time = 0;

  private float timeMax = 0.4f;

  void Start() {
    time = timeMax;
  }

  void Update() {
    if (time >= 0 && time <= timeMax) {
      time -= Time.deltaTime;
    }
  }

  public void PlaySound(float volume) {
    if (time <= 0) {

      time = timeMax;

      var sound = Instantiate(soundPrefab, this.transform.position, Quaternion.identity);
      sound.GetComponent<NoiseScript>().SetVolume(volume);
    }
  }
}



