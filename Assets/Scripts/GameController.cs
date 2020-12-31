using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

  private static GameController _instance;

  public static GameController Instance { get { return _instance; } }
  public float time { get; private set; }

  private GameObject resetPoint;

  public GameObject player;

  [SerializeField]
  private string currentLevel = "SampleScene";

  private float resetTime = 0;

  private const float resetMaxTime = 1;

  private bool reset = false;



  void Start() {
    resetPoint = GameObject.Find("ResetPoint");
    player.transform.position = resetPoint.transform.position;


  }


  void Update() {
    time += Time.deltaTime;

    if (reset) {
      if (resetTime <= resetMaxTime) {
        resetTime += Time.unscaledDeltaTime;
      } else {
        reset = false;
        resetTime = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevel);
      }
    }
  }

  public void Reset() {
    Time.timeScale = 0;
    reset = true;
  }

  void Awake() {
    if (_instance != null && _instance != this) {
      Destroy(this.gameObject);
    } else {
      _instance = this;
    }
  }
}
