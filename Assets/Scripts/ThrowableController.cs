using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour {
  private List<GameObject> objectsVisible;
  private ThrowableScript selected;
  // Start is called before the first frame update
  void Start() {
    objectsVisible = new List<GameObject>();
  }

  // Update is called once per frame
  void Update() {
    var changeLeft = Input.GetButtonDown("Select Left");
    var changeRight = Input.GetButtonDown("Select Right");

    if (changeLeft || changeRight) {

      var obj = GetObjectClose(changeLeft);
      if (obj != null) {
        if (selected) {
          selected.Diselect();
        }
        selected = obj.GetComponent<ThrowableScript>();
        selected.Select();
      }
    }
  }

  GameObject GetObjectClose(bool left) {
    if (objectsVisible.Count > 0) {
      GameObject mostClose = objectsVisible[0];
      float distanceClose = Vector2.Distance(mostClose.transform.position, this.transform.position);
      int lastPoints = 0;
      foreach (var obj in objectsVisible) {
        Debug.Log(obj.name);
        var direction = Camera.main.WorldToViewportPoint(obj.transform.position);
        float distanceObj = Vector2.Distance(obj.transform.position, this.transform.position);

        int points = 0;
        if (left) {
          if (direction.x < 0) {
            points += 2;
          }
        } else {
          if (direction.x >= 0) {
            points += 2;
          }
        }

        if (distanceObj <= distanceClose) {
          points++;
        }

        if (points > lastPoints) {
          mostClose = obj;
          lastPoints = points;
        }
      }

      return mostClose;
    }
    return null;
  }

  public void UpdateObjects(GameObject obj, bool remove) {
    if (remove) {
      if (objectsVisible.Contains(obj)) {
        objectsVisible.Remove(obj);
      }
      return;
    }

    objectsVisible.Add(obj);
  }
}
