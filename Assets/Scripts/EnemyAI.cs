using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;


public enum EnemyBehaviour {
  Follow,
  Searching,
  Patrol,
  Catching,
  Hear
}

public class EnemyAI : MonoBehaviour, NoiseDetectionScript {

  [SerializeField]
  private float speed = 20f;

  private float speedModified = 0;
  [SerializeField]
  private float nextPointDist = 0.2f;

  private Path path;
  private int currentWaypoint = 0;
  private bool endReached = false;

  private Seeker seeker;
  private Rigidbody2D rigidbody;

  [SerializeField]
  private Transform vision;

  [SerializeField]
  private Vector2 target;

  public EnemyBehaviour behaviour { get; private set; }

  private bool inSight = false;

  [SerializeField]
  private Transform[] patrolPoints;

  private float time = 0;
  [SerializeField]

  private float waitTime = 1.5f;

  [SerializeField]

  private float searchTime = 8f;


  private GameObject player;

  [SerializeField]
  private Vector2 direction;

  private bool newPath = false;


  [SerializeField]
  private float catchMaxTime = 0.4f;

  private const float STUN_TIME = 2f;

  private bool stunned = false;

  private Animator animator;

  private SpriteRenderer sprite;


  // Start is called before the first frame update
  void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    seeker = GetComponent<Seeker>();
    Patrol();
    behaviour = EnemyBehaviour.Patrol;
    player = GameObject.Find("Player");
    speedModified = speed / 10f;
    direction = rigidbody.velocity;
    animator = GetComponent<Animator>();
    sprite = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update() {
    if (stunned) {
      if (time <= STUN_TIME) {
        time += Time.deltaTime;
        return;
      } else {
        stunned = false;
        GetComponent<Collider2D>().enabled = true;

        behaviour = EnemyBehaviour.Searching;
        time = 0;
      }
    }


    switch (behaviour) {
      case EnemyBehaviour.Hear:
        direction = target - (Vector2)this.transform.position;
        WalkToTarget();
        break;

      case EnemyBehaviour.Follow:
        vision.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        direction = player.transform.position - this.transform.position;
        WalkToTarget();
        break;

      case EnemyBehaviour.Patrol:
        if (time <= waitTime) {
          time += Time.deltaTime;
        } else {
          direction = rigidbody.velocity;
          WalkToTarget(Patrol);
        }
        break;

      case EnemyBehaviour.Searching:
        if (time <= searchTime) {
          time += Time.deltaTime;

          vision.GetComponentInChildren<SpriteRenderer>().color = Color.white;
          if (time <= searchTime / 3f || time >= (searchTime / 3f * 2)) {
            direction.x = (direction.normalized.x + (time >= searchTime / 3f ? -0.025f : 0.02f));
            direction.y = (direction.normalized.y + (time >= searchTime / 3f ? -0.025f : 0.01f));
          }

        } else {
          time = 0;
          behaviour = EnemyBehaviour.Patrol;
        }
        break;

      case EnemyBehaviour.Catching:
        rigidbody.velocity = Vector2.zero;

        if (time <= catchMaxTime) {
          vision.Find("Attack").gameObject.SetActive(true);
          time += Time.deltaTime;
        } else {
          time = 0;

          if (vision.GetComponent<KillScript>().onRange) {
            player.GetComponent<PlayerMovement>().Blocked();

            GameController.Instance.Reset();
          } else {
            vision.Find("Attack").gameObject.SetActive(false);

            time = 0;
            behaviour = EnemyBehaviour.Searching;
          }
        }

        break;
    }

    if (behaviour != EnemyBehaviour.Catching) {
      CastVision();
    }



  }

  void WalkToTarget(Action callback) {
    rigidbody.velocity = Vector2.zero;

    if (endReached) {
      time = 0;
      endReached = false;
      callback();
      return;
    }

    if (currentWaypoint >= path.vectorPath.Count) {
      endReached = true;
      return;
    }

    Vector2 direction = (Vector2)path.vectorPath[currentWaypoint] - rigidbody.position;
    rigidbody.velocity = direction.normalized * speedModified;


    if (direction.x > 0) {
      sprite.flipX = true;
      animator.Play("MoveHorizontal");
    } else if (direction.x < 0) {
      sprite.flipX = false;
      animator.Play("MoveHorizontal");
    } else {
      sprite.flipX = false;
      if (direction.y > 0) {
        animator.Play("MoveVertical");
      } else {
        animator.Play("idle");
      }
    }



    vision.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, direction.normalized) * (direction.y <= 0 ? -1 : 1));

    float distance = Vector2.Distance(rigidbody.position, path.vectorPath[currentWaypoint]);
    if (distance < nextPointDist) {
      currentWaypoint++;
    }
    endReached = false;
  }

  void WalkToTarget() {
    rigidbody.velocity = Vector2.zero;
    if (endReached) {
      time = 0;
      endReached = false;

      if (!inSight) {
        behaviour = EnemyBehaviour.Searching;
      }
      return;
    }

    if (currentWaypoint >= path.vectorPath.Count) {
      endReached = true;
      return;
    }

    Vector2 direction = (Vector2)path.vectorPath[currentWaypoint] - rigidbody.position;
    rigidbody.velocity = direction.normalized * speedModified;



    vision.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, this.direction.normalized) * (this.direction.y <= 0 ? -1 : 1));

    float distance = Vector2.Distance(rigidbody.position, path.vectorPath[currentWaypoint]);
    if (distance < nextPointDist) {
      currentWaypoint++;
    }
    endReached = false;
  }

  void Patrol() {
    int nextPoint = target == (Vector2)patrolPoints[0].position ? 1 : 0;
    target = patrolPoints[nextPoint].position;

    time = 0;
    seeker.StartPath(rigidbody.position, target, OnPathComplete);
    speedModified = speed / 10f;
  }

  void CastVision() {

    RaycastHit2D[] hits = Physics2D.CircleCastAll(vision.position, 0.005f, direction.normalized, 1.5f);
    bool wallHit = false;

    inSight = false;
    foreach (var hit in hits) {
      if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Walls")) {
        wallHit = true;
      }

      if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !wallHit) {

        behaviour = EnemyBehaviour.Follow;
        direction = target - (Vector2)this.transform.position;
        inSight = true;

        FollowPlayer(hit.point);
        break;
      }
    }

    Debug.DrawLine(vision.position, ((Vector2)vision.position + (new Vector2(1f, 1f) * 1.5f * direction.normalized)));

  }


  void FollowPlayer(Vector2 point) {
    if (point != target && currentWaypoint > 1) {
      target = point;
      seeker.StartPath(rigidbody.position, target, OnPathComplete);
      speedModified = speed / 2f;
    }

  }

  void CatchPlayer() {
    //Overlap Circle
    Debug.Log("Try catching");
  }

  void OnPathComplete(Path p) {
    if (!p.error) {
      path = p;
      currentWaypoint = 0;
    }
  }

  public void Stun() {
    rigidbody.velocity = Vector2.zero;
    stunned = true;
    GetComponent<Collider2D>().enabled = false;
    time = 0;
  }

  public void TriggerHit() {
    if (behaviour != EnemyBehaviour.Catching) {
      time = 0;
      behaviour = EnemyBehaviour.Catching;
    }

  }

  private void OnCollisionEnter2D(Collision2D other) {
    if (!stunned) {
      if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
        other.gameObject.GetComponent<PlayerMovement>().Blocked(0.1f);
      }
    }

  }

  public void Hear(Vector2 point) {
    if (behaviour != EnemyBehaviour.Follow) {
      inSight = true;
      behaviour = EnemyBehaviour.Hear;
      FollowPlayer(point);
      Vector2 direction = (Vector2)this.transform.position - point;
      vision.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, direction.normalized) * (direction.x < 0 ? -1 : 1));
    }

  }
}
