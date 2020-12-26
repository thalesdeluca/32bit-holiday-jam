using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThrowableController : MonoBehaviour {
  private List<GameObject> objectsVisible;
  private ThrowableScript selected;

  private int currentObj = 0;
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
      var point = selected ? selected.gameObject.transform.position : this.transform.position;
      Dictionary<float, GameObject> ranking = new Dictionary<float, GameObject>();

      var lastPointLeft = objectsVisible.Aggregate((actual, next) => actual.transform.position.x < next.transform.position.x ? actual : next);

      var lastPointRight = objectsVisible.Aggregate((actual, next) => actual.transform.position.x > next.transform.position.x ? actual : next);

      foreach (var obj in objectsVisible) {
        if (selected) {
          if (obj == selected.gameObject) {
            continue;
          }
          if (left) {
            if (selected.gameObject == lastPointLeft) {
              return lastPointRight;
            }

          } else {
            if (selected.gameObject == lastPointRight) {
              return lastPointLeft;
            }
          }
        }



        float points = 0;
        var distanceFromPoint = Vector2.Distance(obj.transform.position, point);

        points -= distanceFromPoint;

        if (left) {
          if (obj.transform.position.x <= point.x) {
            points += 2;
          }
        } else {
          if (obj.transform.position.x >= point.x) {
            points += 2;
          }
        }

        ranking.Add(points, obj);
      }
      return ranking.OrderByDescending((item) => item.Key).First().Value;

    }
    return null;
  }

  public void UpdateObjects(GameObject obj, bool remove) {
    if (remove) {
      if (objectsVisible.Contains(obj)) {
        if (selected) {
          if (obj == selected) {
            selected = null;
          }
        }

        objectsVisible.Remove(obj);
      }
    } else {
      if (!objectsVisible.Contains(obj)) {
        objectsVisible.Add(obj);

      }
    }

  }




}
