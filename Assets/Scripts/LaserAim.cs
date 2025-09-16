using UnityEngine;
using System.Collections.Generic;
public class LaserAim : MonoBehaviour
{
    [Header("Laser Aim")]
    public LineRenderer aimLine;
    public int maxReflections = 5;
    public LayerMask wallLayer;
    private LineRenderer instanceLine;
    private bool isAiming = false;
    private Transform firePoint;
    private Color c;
    const float maxDist = 100f;
    const float epsilon = 0.01f;
    private List<Vector3> pts;
    private void Awake(){
        instanceLine = Instantiate(aimLine,transform);
        instanceLine.enabled = false;
        instanceLine.useWorldSpace = true;
    }
    public void setIsAiming(bool isAiming){
        this.isAiming = isAiming;
        instanceLine.enabled = isAiming;
    }
    public void setFirePoint(Transform firePoint){
        c = GetComponent<Tank>().GetColor();
        instanceLine.startColor = c;
        instanceLine.endColor = c;
        this.firePoint = firePoint;
    }
    private void Update(){
        if(isAiming)
        {
            DrawLaserAim();
        }
    }
    private void DrawLaserAim()
    {
        if (instanceLine == null || firePoint == null) return;

        // const float minSeg = 0.001f;

        Vector2 origin = firePoint.position;
        Vector2 dir = ((Vector2)transform.right).normalized;

        pts = new List<Vector3> { origin };

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDist, wallLayer);
            if (!hit)
            {
                pts.Add(origin + dir * maxDist);
                break;
            }

            // if (hit.distance < minSeg)
            // {
            //     origin += dir * epsilon; // đẩy nhẹ rồi thử lại
            //     continue;
            // }

            pts.Add(hit.point);

            Vector2 reflect = Vector2.Reflect(dir, hit.normal).normalized;

            // đẩy origin ra khỏi bề mặt theo pháp tuyến để không “dính mép”
            origin = hit.point + hit.normal * epsilon;
            dir = reflect;
        }
        instanceLine.positionCount = pts.Count;
        instanceLine.SetPositions(pts.ToArray());
    }
    public List<Vector3> getPts(){
        return pts;
    }
}