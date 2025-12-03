using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static ExcelReader;

public class CityConnetManage : MonoBehaviour
{
    public static CityConnetManage Instance { get; private set; }
    public GameObject cityConentLinePrefab;
    public List<CityConnection> cityConentLines = new List<CityConnection>();
    [SerializeField] private List<Region> allRegions = new List<Region>(); // For debugging / visualization


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        allRegions.Clear();
        Region[] regionsInScene = GameObject.FindObjectsOfType<Region>();
        foreach (var region in regionsInScene)
        {
            allRegions.Add(region);
        }
    }

    public void OnDestroy()
    {
        CameraControl2D.OnZoomChanged -= ApplyZoomVisibility;
    }

    private void OnDisable()
    {
        CameraControl2D.OnZoomChanged -= ApplyZoomVisibility;
    }

    private void OnEnable()
    {
        CameraControl2D.OnZoomChanged += ApplyZoomVisibility;
    }

    private void Start()
    {
        CollectAndInitializeRegions(); // Step 1: Setup all Region values
        GenerateAllLines();            // Step 2: Build all city connection lines
        InitAllRegionCityPosition();
        BottomButton.Instance.SetTotalPanelSaveData();
    }

    private void CollectAndInitializeRegions()
    {
        foreach (var region in allRegions)
        {
            region.SetRegionValue();
        }
        float currentZoom = Camera.main.orthographicSize;
        ApplyZoomVisibility(currentZoom);
    }


    public void GenerateAllLines()
    {
        List<CityConnetData> cityConnetDatas = LoadCityConnections();
        List<RegionValue> allRegionValues = GameValue.Instance.GetAllRegionValues();

        foreach (var conn in cityConnetDatas)
        {
            if (conn.Region1ID < 0 || conn.Region1ID >= allRegionValues.Count ||
                conn.Region2ID < 0 || conn.Region2ID >= allRegionValues.Count)
            {
                Debug.LogWarning($"Region ID bug {conn.Region1ID} ? {conn.Region2ID}");
                continue;
            }

            Region region1 = GameValue.Instance.GetRegionValue(conn.Region1ID).region;
            Region region2 = GameValue.Instance.GetRegionValue(conn.Region2ID).region;

            if (conn.City1ID < 0 || conn.City1ID >= region1.citys.Count ||
                conn.City2ID < 0 || conn.City2ID >= region2.citys.Count)
            {
                Debug.LogWarning($"City ID bug {conn.City1ID} ? {conn.City2ID}");
                continue;
            }

            GameObject lineGO = Instantiate(cityConentLinePrefab, transform);
            CityConnection ctrl = lineGO.GetComponent<CityConnection>();
            if (ctrl == null)
            {
                Debug.LogError("BezierCurveDashedConnector2D component not found in prefab!");
                continue;
            }

            ctrl.CreateLine(region1, conn.City1ID, region2, conn.City2ID, conn.CityConnetID);
            cityConentLines.Add(ctrl);
        }

        // ????????
        ApplyZoomVisibility(Camera.main.orthographicSize);
    }

    void InitAllRegionCityPosition()
    {
        string path = Path.Combine(Application.dataPath, "StreamingAssets/Value/CityPosition.txt");
        if (!File.Exists(path))
        {
            Debug.LogError($"CityPosition.txt not found at {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length < 6)
            {
                Debug.LogWarning($"Invalid line (not enough fields): {line}");
                continue;
            }

            string regionName = parts[0].Trim();
            int cityIndex;
            float x, y, z;

            if (!int.TryParse(parts[2].Trim(), out cityIndex) ||
                !float.TryParse(parts[3].Trim(), out x) ||
                !float.TryParse(parts[4].Trim(), out y) ||
                !float.TryParse(parts[5].Trim(), out z))
            {
                Debug.LogWarning($"Invalid number in line: {line}");
                continue;
            }

            Region region = allRegions.Find(r => r.GetRegionValue().GetRegionENName() == regionName);
            if (region == null)
            {
                Debug.LogWarning($"Region not found: {regionName}");
                continue;
            }

            region.SetCityPosition(cityIndex, new Vector3(x, y, z));
        }
    }

    public void UpLineShow()
    {
        ApplyZoomVisibility(Camera.main.orthographicSize);
    }

    void ApplyZoomVisibility(float zoom)
    {
        foreach (var region in allRegions)
        {
            region.ApplyZoomVisibility(zoom);
        }

        foreach (var line in cityConentLines)
        {
            bool show = zoom < 8f && line.IsPlayerConnected();
            line.SetLineActive(show);
           // line.SetLineActive(true);
        }
    }

    /// <summary>
    /// BFS ???????????????????????
    /// </summary>
    // BFS????
    private List<CityValue> BFS(CityValue start, CityValue goal, Dictionary<CityValue, List<CityValue>> graph)
    {
        Queue<CityValue> queue = new();
        Dictionary<CityValue, CityValue> cameFrom = new();
        HashSet<CityValue> visited = new();

        queue.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == goal) break;

            if (!graph.ContainsKey(current)) continue;

            foreach (var neighbor in graph[current])
            {
                if (visited.Contains(neighbor)) continue;
                visited.Add(neighbor);
                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        // ????
        List<CityValue> path = new();
        CityValue step = goal;
        while (step != null)
        {
            path.Insert(0, step);
            step = cameFrom.ContainsKey(step) ? cameFrom[step] : null;
        }

        // ????????????????????
        if (path.Count == 1 && path[0] != start)
            return new List<CityValue>();

        return path;
    }

    // ??????????????????????????


    // ??????????????
    public CityConnection FindLine(CityValue a, CityValue b)
    {
        foreach (var line in cityConentLines)
        {
            if ((line.cityA == a && line.cityB == b) || (line.cityA == b && line.cityB == a))
                return line;
        }
        return null;
    }

    /// <summary>
    /// ??????????????????????
    /// </summary>
    public void AnimateNeighborEdgesToward(CityValue target)
    {
        StopAllAnimations(); // ??????????

        if(target == null) return;  

        foreach (var line in cityConentLines)
        {
            if (line.cityA == null || line.cityB == null) continue;

            if (line.cityA == target)
            {
                line.SetDirectionByTarget(line.cityB, line.cityA); // B ? A
                line.StartDashAnimation();
            }
            else if (line.cityB == target)
            {
                line.SetDirectionByTarget(line.cityA, line.cityB); // A ? B
                line.StartDashAnimation();
            }
        }
    }


    /// <summary>
    /// ???????
    /// </summary>
    public void StopAllAnimations()
    {
        foreach (var line in cityConentLines)
        {
            line.StopDashAnimation();
        }
    }

    public bool IsAdjacentToPlayerCity(CityValue city)
    {
        foreach (var line in cityConentLines)
        {
            if (line.cityA == null || line.cityB == null) continue;
            // ?????????????????
            if (line.cityA == city && line.cityB.IsPlayerCity())
            {
                return true;
            }
            else if (line.cityB == city && line.cityA.IsPlayerCity())
            {
                return true;
            }
        }

        return false;
    }

    public Region GetRegion(int regionID)
    {
        var region = allRegions.FirstOrDefault(r => r.regionID == regionID);
        if (region == null)
        {
            UnityEngine.Debug.LogWarning($"[GetRegion] Region not found for ID: {regionID}");
        }
        return region;
    }

}
