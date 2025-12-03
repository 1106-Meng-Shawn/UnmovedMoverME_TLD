using UnityEngine;
using System.Collections.Generic;

public class CityConnection : MonoBehaviour
{
    public int ContentID;
    public Transform pointA;
    public Transform pointB;

    public CityValue cityA;
    public CityValue cityB;

    public List<Vector3> controlPoints = new List<Vector3>();

    [Header("Bezier Sampling")]
    public float segmentDensityMultiplier = 3f;
    public int minSegmentCount = 10;
    public int maxSegmentCount = 1000;
    public int segmentCount = 30;

    [Header("Solid Line")]
    public bool showSolidLine = true;
    public float solidLineWidth = 0.07f;
    public Color solidLineColor = Color.white;
    public int solidLineSortingOrder = 0;

    [Header("Dashed Line")]
    public bool showDashedLine = true;
    public float dashedLineWidth = 0.05f;
    public Color dashedLineColor = Color.white;
    public int dashedLineSortingOrder = 1;

    public float dashLength = 0.3f;
    public float gapLength = 0.2f;
    public float dashMoveSpeed = 5f;
    public bool enableDashAnimation = false;

    public Material lineMaterial;

    public LineRenderer solidLineRenderer;
    private List<GameObject> dashSegments = new List<GameObject>();
    private float dashOffset = 0f;

    [Header("Other")]
    public GameObject cityEvent;


    private void Start()
    {
        GenerateLine();
    }
    private Vector3 lastA, lastB;

    private void Update()
    {
      /*  if (pointA != null && pointB != null)
        {
            // ???????????
            if (pointA.position != lastA || pointB.position != lastB)
            {
                GenerateLine();
                lastA = pointA.position;
                lastB = pointB.position;
            }
        }*/

        if (!enableDashAnimation) return;

        dashOffset += Time.deltaTime * dashMoveSpeed; // FIXED SPEED PER SECOND

        float patternLength = dashLength + gapLength;

        while (dashOffset >= patternLength)
            dashOffset -= patternLength;
        while (dashOffset < 0)
            dashOffset += patternLength;

        GenerateLine();  // redraw with updated offset
    }

    public void CreateLine(Region region1, int city1Index, Region region2, int city2Index, int ID)
    {
        cityA = region1.GetRegionValue().GetCityValue(city1Index);
        cityB = region2.GetRegionValue().GetCityValue(city2Index);

        pointA = region1.citys[city1Index].transform;
        pointB = region2.citys[city2Index].transform;
        ContentID = ID;
    }

    public bool IsPlayerConnected()
    {
        return cityA.IsPlayerCity() || cityB.IsPlayerCity();
    }

    public void GenerateLine()
    {
        if (pointA == null || pointB == null) return;

        Vector3 start = pointA.position;
        Vector3 end = pointB.position;
        float distance = Vector3.Distance(start, end);
        segmentCount = Mathf.Clamp(Mathf.RoundToInt(distance * segmentDensityMultiplier), minSegmentCount, maxSegmentCount);

        // ?????
        List<Vector3> allPoints = new List<Vector3> { start };
        allPoints.AddRange(controlPoints.Count == 0 ? new[] { (start + end) * 0.5f } : controlPoints);
        allPoints.Add(end);

        // ?? z = 0
        for (int i = 0; i < allPoints.Count; i++)
            allPoints[i] = new Vector3(allPoints[i].x, allPoints[i].y, 0);

        // ????????
        Vector3[] points = new Vector3[segmentCount + 1];
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            points[i] = CalculateBezierPoint(allPoints, t);
        }

        // ????
        if (showSolidLine)
        {
            if (solidLineRenderer == null)
                InitializeSolidLineRenderer();

            solidLineRenderer.positionCount = points.Length;
            solidLineRenderer.startWidth = solidLineWidth;
            solidLineRenderer.endWidth = solidLineWidth;
            solidLineRenderer.startColor = solidLineColor;
            solidLineRenderer.endColor = solidLineColor;
            solidLineRenderer.sortingOrder = solidLineSortingOrder;

            for (int i = 0; i < points.Length; i++)
                solidLineRenderer.SetPosition(i, points[i]);
        }
        else if (solidLineRenderer != null)
        {
            solidLineRenderer.positionCount = 0;
        }

        // ????
        if (showDashedLine)
            DrawDashedLine(points);
        else
            ClearOldDashSegments();

        // -----------------------------
        // ? cityEvent ??????????????
        // -----------------------------
        if (cityEvent != null && points.Length > 1)
        {
            float totalLength = 0f;
            float[] segmentLengths = new float[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                float len = Vector3.Distance(points[i], points[i + 1]);
                segmentLengths[i] = len;
                totalLength += len;
            }

            float halfLength = totalLength / 2f;
            float accumulated = 0f;

            for (int i = 0; i < segmentLengths.Length; i++)
            {
                if (accumulated + segmentLengths[i] >= halfLength)
                {
                    float t = (halfLength - accumulated) / segmentLengths[i];
                    if (float.IsNaN(t) || float.IsInfinity(t))
                    {
                        Debug.Log($"[GenerateLine] t invalid: {t}, segLen={segmentLengths[i]} between {points[i]} and {points[i + 1]}");
                        t = 0.5f; // fallback ???
                    }

                    Vector3 pos = Vector3.Lerp(points[i], points[i + 1], t);

                    // ?? pos ? NaN
                    if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z))
                    {
                        Debug.LogError($"[GenerateLine] pos invalid: {pos}");
                    }
                    else
                    {
                        cityEvent.transform.position = pos;
                    }
                    break;
                }
                accumulated += segmentLengths[i];
            }
        }
    }


    private void DrawDashedLine(Vector3[] points)
    {
        ClearOldDashSegments();
        if (points.Length < 2) return;

        float[] cumulativeLengths = new float[points.Length];
        for (int i = 1; i < points.Length; i++)
            cumulativeLengths[i] = cumulativeLengths[i - 1] + Vector3.Distance(points[i], points[i - 1]);

        float totalLength = cumulativeLengths[^1];
        float patternLength = dashLength + gapLength;
        float currentLength = dashOffset % patternLength;

        while (currentLength < totalLength)
        {
            float startLength = currentLength;
            float endLength = Mathf.Min(currentLength + dashLength, totalLength);

            Vector3 startPos = InterpolatePointAtLength(points, cumulativeLengths, startLength);
            Vector3 endPos = InterpolatePointAtLength(points, cumulativeLengths, endLength);
            CreateDashSegment(startPos, endPos);

            currentLength += patternLength;
        }
    }

    private void CreateDashSegment(Vector3 start, Vector3 end)
    {
        GameObject dash = new GameObject("DashSegment");
        dash.transform.SetParent(solidLineRenderer.gameObject.transform, false);

        LineRenderer lr = dash.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = dashedLineWidth;
        lr.endWidth = dashedLineWidth;
        lr.material = lineMaterial ?? new Material(Shader.Find("Sprites/Default"));
        lr.startColor = dashedLineColor;
        lr.endColor = dashedLineColor;
        lr.useWorldSpace = true;
        lr.alignment = LineAlignment.View;
        lr.numCapVertices = 2;
        lr.sortingOrder = dashedLineSortingOrder;

        dashSegments.Add(dash);
    }

    private void ClearOldDashSegments()
    {
        foreach (var dash in dashSegments)
            if (dash != null) DestroyImmediate(dash);
        dashSegments.Clear();
    }

    private void InitializeSolidLineRenderer()
    {
      //  solidLineRenderer = GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
        solidLineRenderer.useWorldSpace = true;
        solidLineRenderer.alignment = LineAlignment.View;
        solidLineRenderer.numCapVertices = 2;
        // solidLineRenderer.material = lineMaterial ?? new Material(Shader.Find("Sprites/Default"));
        solidLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private Vector3 InterpolatePointAtLength(Vector3[] points, float[] cumulativeLengths, float targetLength)
    {
        for (int i = 1; i < cumulativeLengths.Length; i++)
        {
            if (cumulativeLengths[i] >= targetLength)
            {
                float segLength = cumulativeLengths[i] - cumulativeLengths[i - 1];
                float t = (targetLength - cumulativeLengths[i - 1]) / segLength;
                return Vector3.Lerp(points[i - 1], points[i], t);
            }
        }
        return points[^1];
    }

    private Vector3 CalculateBezierPoint(List<Vector3> pts, float t)
    {
        if (pts.Count == 1) return pts[0];
        List<Vector3> nextPts = new();
        for (int i = 0; i < pts.Count - 1; i++)
            nextPts.Add(Vector3.Lerp(pts[i], pts[i + 1], t));
        return CalculateBezierPoint(nextPts, t);
    }

    public void SetDirectionByTarget(CityValue from, CityValue to)
    {
        
        if (cityA == from && cityB == to)
            dashMoveSpeed = Mathf.Abs(dashMoveSpeed);
        else if (cityA == to && cityB == from)
            dashMoveSpeed = -Mathf.Abs(dashMoveSpeed);
        else
            Debug.LogWarning("SetDirectionByTarget: mismatched city pair");


        StartDashAnimation();

    }


    public void SetLineActive(bool isActive)
    {
        solidLineRenderer.gameObject.SetActive(isActive);
        SetCityEventActive();
    }

    void SetCityEventActive()
    {
        if (cityA.cityCountry == cityB.cityCountry || !IsPlayerConnected())
        {
            cityEvent.gameObject.SetActive(false);
            return;
        }

        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        cityEvent.gameObject.SetActive(countryManager.IsAtWar(cityA.cityCountry,cityB.cityCountry));

    }

    public void BattleHappend()
    {
        if (cityA.cityCountry != GameValue.Instance.GetPlayerCountryENName()) {
            BattlePanelManage.Instance.ShowBattlePanel(cityA.regionValue,cityA.cityIndex,false);
        } else if (cityB.cityCountry != GameValue.Instance.GetPlayerCountryENName())
        {
            BattlePanelManage.Instance.ShowBattlePanel(cityB.regionValue, cityB.cityIndex, false);
        }
    }


    public void StartDashAnimation() => enableDashAnimation = true;
    public void StopDashAnimation() => enableDashAnimation = false;
}



