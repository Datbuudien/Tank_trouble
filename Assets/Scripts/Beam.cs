using UnityEngine; 
using System.Collections.Generic;
public class Beam : MonoBehaviour
{
    [Header("Beam Properties")]
    public float damage = 0.00000001f;

    public float timeDuration = 5f;
    public static float lifetime = 12f;
    public LayerMask tankLayer;
    public static float fireRate = 10f;
    public static float beamLength = 50f;
    public static float beamWidth = 0.7f;
    // private float offset = 25f;
    private Transform firePoint;
    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>(3);

    public void BeamUpdate(){
        // Debug.Log("BeamUpdate");
        hits.Clear();
        RaycastHit2D hit_1 = Physics2D.Raycast(firePoint.position + firePoint.up.normalized * (beamWidth * 0.5f),
                                       firePoint.right, beamLength+0.1f, tankLayer);
        RaycastHit2D hit_2 = Physics2D.Raycast(firePoint.position - firePoint.up.normalized * (beamWidth * 0.5f),
                                       firePoint.right, beamLength+0.1f, tankLayer);
        RaycastHit2D hit_3 = Physics2D.Raycast(firePoint.position,
                                       firePoint.right, beamLength+0.1f,tankLayer);
        hits.Add(hit_1);
        hits.Add(hit_2);
        hits.Add(hit_3);
        foreach (RaycastHit2D temp in hits){
            if(temp.collider !=null){
                Tank tank = temp.collider.GetComponent<Tank>();
                tank.TakeDamage(damage);
                return;
            }   
        }
        
        // RaycastHit2D hit = Physics2D.BoxCast(firePoint.position +firePoint.right.normalized*offset ,new Vector2(50f,0.7f),firePoint.eulerAngles.z,firePoint.right,0.5f, tankLayer);
        // if(hit.collider !=null){
        //     Tank tank = hit.collider.GetComponent<Tank>();
        //     tank.TakeDamage(damage);

        // }
    }
        public void setFirePoint(Transform firePoint){
        this.firePoint = firePoint; 
    }


}