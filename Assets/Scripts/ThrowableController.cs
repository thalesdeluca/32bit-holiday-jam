using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThrowableController : MonoBehaviour {
  private List<GameObject> objectsVisible;
  private GameObject selected;

  private int currentObj = 0;

  private bool throwedHold;
  // Start is called before the first frame update
  void Start() {
    objectsVisible = new List<GameObject>();
  }

  // Update is called once per frame
  void Update() {
    var changeLeft = Input.GetButtonDown("Select Left");
    var changeRight = Input.GetButtonDown("Select Right");
    var throwed = Input.GetButtonDown("Throw");
    var hold = Input.GetButton("Throw");

    if (hold && selected) {
      Debug.Log("Hold");
      throwedHold = true;
      GetComponent<PlayerMovement>().Blocked(throwedHold);
    } else if (throwedHold) {
      Debug.Log("Release");
      throwedHold = false;
      GetComponent<PlayerMovement>().Blocked(throwedHold);
    }



    if (changeLeft || changeRight) {

      var obj = GetObjectClose(changeLeft);
      if (obj != null) {
        if (selected) {
          selected.GetComponent<ThrowableScript>().Diselect();
        }
        selected = obj;
        selected.GetComponent<ThrowableScript>().Select();
      }
    }
    if (throwed && selected) {
      var horizontal = Input.GetAxisRaw("Horizontal");
      var vertical = Input.GetAxisRaw("Vertical");

      var direction = new Vector2(horizontal, vertical);
      selected.GetComponent<ThrowableScript>().Throw(direction.normalized);
      selected = null;
    }


  }

  GameObject GetObjectClose(bool left) {
    if (objectsVisible.Count > 0) {

      if (objectsVisible.Count == 1) {
        return objectsVisible[0];
      }

      Debug.Log("Close " + objectsVisible.Count);
      var point = selected ? selected.transform.position : this.transform.position;
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
        var removed = objectsVisible.RemoveAll(x => x == obj);
        Debug.Log("Removed " + obj.name + " " + removed);

      }
    } else {
      if (!objectsVisible.Contains(obj)) {
        Debug.Log("Added " + obj.name);

        objectsVisible.Add(obj);
      }
    }
  }
}
