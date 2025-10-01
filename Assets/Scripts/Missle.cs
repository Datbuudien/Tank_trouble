using System.Collections.Generic; 
using UnityEngine;
public class Missle : MonoBehaviour
{
    [Header("Missle Properties")]
    public float speed = 3f;
    public Transform target; 
    public ScanMap scanMap;
    private List<Vector2Int> path;
    private int pathIndex; 
    private Vector2Int lastGoal;
    private float repathCooldown = 0.1f; 
    private float lastRepathTime;
    private Vector2Int start; 
    private Vector2Int goal;
    private Pathfinding pf;
    private Vector3 targetWorld;
    private Vector3 dir;
    private float angle;
    private float rotationSpeed = 270f;
    Quaternion targetRotation;
    void Start(){
        path = new List<Vector2Int>();
        pathIndex = 0;
        lastGoal = new Vector2Int(int.MinValue,int.MinValue);
        lastRepathTime = -repathCooldown;
        pf = new Pathfinding(scanMap.walkable);
    }
    void Update(){
        if(scanMap == null || target == null) return;
        goal = scanMap.WorldToGrid(target.position);
        if(goal != lastGoal && Time.time - lastRepathTime > repathCooldown){
            start = scanMap.WorldToGrid(transform.position);
            path = pf.FindPath(start,goal);
            pathIndex = 1;
            lastGoal = goal;
            lastRepathTime = Time.time;
        }
        FollowPath();
    }
    void FollowPath(){
        if(path == null || pathIndex >= path.Count) return;
        targetWorld = scanMap.GridToWorld(path[pathIndex]);
        dir = (targetWorld - transform.position).normalized;
        transform.position += dir*speed*Time.deltaTime;
        angle = Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0,0,angle);
        // transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,rotationSpeed*Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,rotationSpeed * Time.deltaTime);
        Debug.Log(Time.deltaTime);
        if(Vector3.Distance(transform.position,targetWorld) <0.1f){
            transform.position = targetWorld;
            pathIndex++;
        }
    }
}