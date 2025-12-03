using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    public Material lineMaterial;
    private GameObject countrylineObj;
    private GameObject regionlineObj;

    private List<GameObject> countrylineObjs = new List<GameObject>();
    private List<GameObject> regionlineObjs = new List<GameObject>();


    private Region[] allRegions;

    private void Start()
    {
        allRegions = GameObject.FindObjectsOfType<Region>();
        StartCoroutine(ToggleRegionLines());
        StartCoroutine(ToggleCountryLines());

    }

    public Material waterMat;
    public float speed = 1f;

    void Update()
    {
        waterMat.SetFloat("_T", Mathf.PingPong(Time.time * speed, 1f));
    }

    private IEnumerator ToggleRegionLines()
    {
        bool state = true;

        while (true)
        {
            for (int i = 0; i < regionlineObjs.Count; i++)
            {
                if (regionlineObjs[i] != null)
                    regionlineObjs[i].SetActive(state ? i % 2 != 0 : i % 2 == 0);
            }

            state = !state;
            yield return new WaitForSeconds(0.75f); 
        }
    }

    private IEnumerator ToggleCountryLines()
    {
        bool state = true;

        while (true)
        {
            for (int i = 0; i < countrylineObjs.Count; i++)
            {
                if (countrylineObjs[i] != null)
                    countrylineObjs[i].SetActive(state ? i % 2 == 0 : i % 2 != 0);
            }

            state = !state;
            yield return new WaitForSeconds(0.75f); 
        }
    }


    void DrawRegionOutline(Region region)
    {
        if (regionlineObj != null)
        {
            Destroy(regionlineObj);
            regionlineObjs.Clear();
        }

        StartCoroutine(CreateRegionOutlineAfterFrame(region));
    }

    IEnumerator CreateRegionOutlineAfterFrame(Region region)
    {
        yield return null; // 等待一帧，确保旧对象销毁完成

        regionlineObj = new GameObject("ClickedRegionOutline");
        regionlineObj.transform.SetParent(this.transform, false);
        regionlineObj.layer = 3;

        PolygonCollider2D clickedCollider = region.GetComponent<PolygonCollider2D>();
        if (clickedCollider != null)
        {
            // 遍历 PolygonCollider2D 中的每个路径
            for (int p = 0; p < clickedCollider.pathCount; p++)
            {
                Vector2[] points = clickedCollider.GetPath(p);

                // 将局部坐标转换为世界坐标
                Vector3[] worldPoints = points
                    .Select(pt => (Vector3)clickedCollider.transform.TransformPoint(pt))
                    .ToArray();

                // 使用 BuildPathsFromEdges 细分路径
                List<Edge> edges = new List<Edge>();
                for (int i = 0; i < worldPoints.Length; i++)
                {
                    Vector3 a = worldPoints[i];
                    Vector3 b = worldPoints[(i + 1) % worldPoints.Length];
                    edges.Add(new Edge(a, b));
                }

                List<List<Vector3>> edgePaths = BuildPathsFromEdges(edges, 0.75f); // 细分边缘

                // 为每一条细分路径创建一个新的 GameObject 和 LineRenderer
                   for (int j = 0; j < edgePaths.Count; j ++)
             {
                    List<Vector3> path = edgePaths[j];
                 GameObject pathObj = new GameObject("Path_" + p);
                 pathObj.transform.SetParent(regionlineObj.transform, false);
                 pathObj.layer = 3;

                 LineRenderer lrClicked = pathObj.AddComponent<LineRenderer>();
                 lrClicked.material = new Material(Shader.Find("Sprites/Default"));

                 lrClicked.widthMultiplier = 0.3f;

                 // 设置 LineRenderer 的点数和位置
                 lrClicked.positionCount = path.Count;
                 lrClicked.SetPositions(path.ToArray());

                 // 获取区域的颜色，并设置线条颜色和透明度
                 Color32 newColor = InvertBrightness(region.GetRegionColor());

                 newColor = new Color32(newColor.r, newColor.g, newColor.b, 255);

                 // 设置渐变颜色
                 Gradient gradient = new Gradient();
                 GradientColorKey[] colorKeys = new GradientColorKey[] {
                 new GradientColorKey(newColor, 1.0f),
                 new GradientColorKey(Color.white, 1.0f),
                 new GradientColorKey(newColor, 1.0f)
             };

                 GradientAlphaKey[] alphaKeys = new GradientAlphaKey[] {
                 new GradientAlphaKey(1.0f, 0.0f),
                 new GradientAlphaKey(1.0f, 0.199f),
                 new GradientAlphaKey(0.0f, 0.2f),
                 new GradientAlphaKey(0.0f, 0.799f),
                 new GradientAlphaKey(1.0f, 0.8f),
                 new GradientAlphaKey(1.0f, 1.0f)
             };

                     lrClicked.gameObject.SetActive(j % 2 == 0);
                    regionlineObjs.Add(lrClicked.gameObject);



                    gradient.SetKeys(colorKeys, alphaKeys);
                 gradient.mode = GradientMode.Fixed;

                 lrClicked.colorGradient = gradient;
                 lrClicked.sortingLayerName = "Default";
                 lrClicked.sortingOrder = 3;
                 lrClicked.useWorldSpace = true;

                 lrClicked.loop = false;
             }
                /*   foreach (var path in edgePaths)
                   {
                       GameObject pathObj = new GameObject("Path_" + p);
                       pathObj.transform.SetParent(regionlineObj.transform, false);
                       pathObj.layer = 3;

                       LineRenderer lrClicked = pathObj.AddComponent<LineRenderer>();
                       lrClicked.material = new Material(Shader.Find("Sprites/Default"));

                       lrClicked.widthMultiplier = 0.3f;

                       // 设置 LineRenderer 的点数和位置
                       lrClicked.positionCount = path.Count;
                       lrClicked.SetPositions(path.ToArray());

                       // 获取区域的颜色，并设置线条颜色和透明度
                       Color32 newColor = InvertBrightness(region.GetRegionColor());

                       newColor = new Color32(newColor.r, newColor.g, newColor.b, 255);

                       // 设置渐变颜色
                       Gradient gradient = new Gradient();
                       GradientColorKey[] colorKeys = new GradientColorKey[] {
                       new GradientColorKey(newColor, 1.0f),
                       new GradientColorKey(Color.white, 1.0f),
                       new GradientColorKey(newColor, 1.0f)
                   };

                       GradientAlphaKey[] alphaKeys = new GradientAlphaKey[] {
                       new GradientAlphaKey(1.0f, 0.0f),
                       new GradientAlphaKey(1.0f, 0.199f),
                       new GradientAlphaKey(0.0f, 0.2f),
                       new GradientAlphaKey(0.0f, 0.799f),
                       new GradientAlphaKey(1.0f, 0.8f),
                       new GradientAlphaKey(1.0f, 1.0f)
                   };

                           lrClicked.gameObject.SetActive(i % 2 == 0);


                       gradient.SetKeys(colorKeys, alphaKeys);
                       gradient.mode = GradientMode.Fixed;

                       lrClicked.colorGradient = gradient;
                       lrClicked.sortingLayerName = "Default";
                       lrClicked.sortingOrder = 3;
                       lrClicked.useWorldSpace = true;

                       lrClicked.loop = false;
                   }*/
            }
        }
    }


    public static bool IsColorDark(Color color)
    {
        // 使用加权平均公式来模拟人眼对颜色亮度的感知
        float brightness = (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f);
        return brightness < 0.5f; // 小于 0.5 认为是深色
    }

    public static Color InvertBrightnessExact(Color originalColor)
    {
        return new Color(
            1.0f - originalColor.r,
            1.0f - originalColor.g,
            1.0f - originalColor.b,
            originalColor.a
        );
    }


    // 转换深浅色
    public static Color InvertBrightness(Color originalColor)
    {
        bool isDark = IsColorDark(originalColor);

        if (isDark)
        {
            // 如果是深色，调亮：线性插值到白色
            return Color.Lerp(originalColor, Color.white, 0.6f);
        }
        else
        {
            // 如果是浅色，调暗：线性插值到黑色
            return Color.Lerp(originalColor, Color.black, 0.6f);
        }
    }


    private void DrawCountyOutline(Region clickedRegion)
    {
        DrawRegionOutline(clickedRegion);

        List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();

        foreach (var region in allRegions)
        {
            if (region.GetRegionValue().GetCountryENName() == clickedRegion.GetRegionValue().GetCountryENName())
            {
                PolygonCollider2D poly = region.GetComponent<PolygonCollider2D>();
                if (poly != null)
                {
                    colliders.Add(poly);
                }
            }
        }

        colliders.Remove(clickedRegion.gameObject.GetComponent<PolygonCollider2D>());


        if (countrylineObj != null)
        {
            Destroy(countrylineObj);
            countrylineObjs.Clear();

        }

        countrylineObj = new GameObject("CountyOutline");
        countrylineObj.transform.SetParent(this.transform, false);

        Dictionary<Edge, int> edgeCount = new Dictionary<Edge, int>(new EdgeComparer());

        foreach (var col in colliders)
        {
            for (int p = 0; p < col.pathCount; p++)
            {
                Vector2[] points = col.GetPath(p);
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 localA = points[i];
                    Vector2 localB = points[(i + 1) % points.Length];

                    Vector2 worldA = col.transform.TransformPoint(localA);
                    Vector2 worldB = col.transform.TransformPoint(localB);

                    Edge edge = new Edge(worldA, worldB);
                    if (edgeCount.ContainsKey(edge))
                        edgeCount[edge]++;
                    else
                        edgeCount[edge] = 1;
                }
            }
        }

        List<Edge> outerEdges = edgeCount.Where(kvp => kvp.Value == 1).Select(kvp => kvp.Key).ToList();

        List<List<Vector3>> edgePaths = BuildPathsFromEdges(outerEdges,0.5f);


        int k = 0;
        foreach (var path in edgePaths)
        {
            GameObject edgeObj = new GameObject("EdgePath");
            edgeObj.transform.SetParent(countrylineObj.transform);
            edgeObj.layer = 3;

            LineRenderer lr = edgeObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

            Color countryColor = clickedRegion.GetCountryColorByRegion();


            lr.startColor = countryColor;
            lr.endColor = countryColor;


            lr.positionCount = path.Count;
            lr.SetPositions(path.ToArray());
            lr.widthMultiplier = 0.1f;
            lr.sortingLayerName = "Default";
            lr.sortingOrder = 2;
            lr.useWorldSpace = true;
            lr.loop = true;

            lr.gameObject.SetActive(k % 2 == 1);
            k++;
            countrylineObjs.Add(lr.gameObject);

        }
    }

    private List<List<Vector3>> BuildPathsFromEdges(List<Edge> edges, float stepLength)
    {
        List<List<Vector3>> paths = new List<List<Vector3>>();
        HashSet<Edge> visited = new HashSet<Edge>(new EdgeComparer());

        while (edges.Any(e => !visited.Contains(e)))
        {
            Edge startEdge = edges.First(e => !visited.Contains(e));
            List<Vector2> rawPath = new List<Vector2> { startEdge.a, startEdge.b };
            visited.Add(startEdge);

            Vector2 current = startEdge.b;

            while (true)
            {
                Edge nextEdge = edges.FirstOrDefault(e =>
                    !visited.Contains(e) &&
                    (Approximately(e.a, current) || Approximately(e.b, current)));

                if (nextEdge.a == Vector2.zero && nextEdge.b == Vector2.zero)
                    break;

                Vector2 nextPoint = Approximately(nextEdge.a, current) ? nextEdge.b : nextEdge.a;
                rawPath.Add(nextPoint);
                visited.Add(nextEdge);
                current = nextPoint;

                if (Approximately(current, rawPath[0]))
                    break;
            }

            // 🧮 对 rawPath 重新采样，平均步长为 stepLength
            List<List<Vector3>> segments = ResamplePathToSegments(rawPath, stepLength);
            paths.AddRange(segments);
        }
            return paths;
    }

    // 按平均长度采样路径点
    private List<List<Vector3>> ResamplePathToSegments(List<Vector2> path, float stepLength)
    {
        List<List<Vector3>> result = new List<List<Vector3>>();
        if (path.Count < 2) return result;

        List<Vector3> currentSegment = new List<Vector3>();
        Vector2 prev = path[0];
        float remaining = 0f;
        currentSegment.Add(prev);

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 curr = path[i];
            float segmentLength = Vector2.Distance(prev, curr);

            while (segmentLength + remaining >= stepLength)
            {
                float t = (stepLength - remaining) / segmentLength;
                Vector2 newPoint = Vector2.Lerp(prev, curr, t);
                currentSegment.Add(newPoint);
                result.Add(new List<Vector3>(currentSegment));
                currentSegment.Clear();
                currentSegment.Add(newPoint);

                prev = newPoint;
                segmentLength = Vector2.Distance(prev, curr);
                remaining = 0f;
            }

            remaining += segmentLength;
            prev = curr;
        }

        return result;
    }


    private bool Approximately(Vector2 v1, Vector2 v2)
    {
        return Vector2.Distance(v1, v2) < 0.01f;
    }

    private struct Edge
    {
        public Vector2 a, b;
        public Edge(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }
    }

    private class EdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge e1, Edge e2)
        {
            return (Approximately(e1.a, e2.a) && Approximately(e1.b, e2.b)) ||
                   (Approximately(e1.a, e2.b) && Approximately(e1.b, e2.a));
        }

        public int GetHashCode(Edge e)
        {
            unchecked
            {
                int hashA = e.a.GetHashCode();
                int hashB = e.b.GetHashCode();
                return hashA ^ hashB;
            }
        }

        private bool Approximately(Vector2 v1, Vector2 v2)
        {
            return Vector2.Distance(v1, v2) < 0.01f;
        }
    }
}

