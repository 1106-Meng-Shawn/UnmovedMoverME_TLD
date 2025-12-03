using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexGridUIManager : MonoBehaviour
{
    public static HexGridUIManager Instance;


    private RegionValue regionValue;

    public GameObject hexPrefab;
    public RectTransform viewport;
    public RectTransform hexContainer;
    public Button restButton;

    public Button refreshButton;
    public Button hideBuildButton;

    private int cityIndex;


    public float hexSize = 50f;

    private float currentScale = 1f;
    private const float maxScale = 5f;
    private const float zoomSpeed = 0.5f;

    private HashSet<Vector2Int> placedHexes = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, HexCellUI> coordToCell = new Dictionary<Vector2Int, HexCellUI>();
    //  private Dictionary<Vector2Int, string> terrainMap = new Dictionary<Vector2Int, string>();


    private bool isShowBuild = false; public bool GetIsShowBuild() {  return isShowBuild; }
    public new Animation animation;

    private HexCellUI hexCellUI;
    private GameValue gameValue;

    public BuildPanelTopRowControl buildPanelTopRowControl;


    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1)
    };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        restButton.onClick.AddListener(OnRestButtonClick);
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
        hideBuildButton.onClick.AddListener(HideBuildPanel);
        hideBuildButton.gameObject.SetActive(false);

    }

    void OnRefreshButtonClick()
    {
        if (isShowBuild) HideBuildPanel();
        RandomSeed();
        ClearMap();
        InitMap();
    }

    void ClearMap()
    {
        foreach (Transform child in hexContainer)
        {
            Destroy(child.gameObject); // 销毁所有地图上的 Hex 对象
        }
        placedHexes.Clear();           // 清空记录
        coordToCell.Clear();
    }

    void InitMap()
    {

        RandomSeed();

        GenerateHexGrid(3);
        AdjustContentSize(3);
        GenerateRivers();
        UpdateAllHexBuilding();

        SetScale(GetMinScale());


    }


    private void Update()
    {
        HandleViewportControl();
    }

    private void HandleViewportControl()
    {
        if (!IsPointerOverRect(hexContainer)) return;

        HandleZoomInput();
        HandleMoveInput();
    }


  /*  void ShowOrHideBuildPanel()
    {
        if (isShowBuild)
        {
            HideBuildPanel();
        }
        else
        {
            ShowBuildPanel();
        }
    }*/

    public void ShowBuildPanel(HexCellUI hexCell)
    {
        SetHexCellUI(hexCell);
        animation.Play("BuildingPanelShow");
        isShowBuild = true;
      //  hideBuildButton.gameObject.SetActive(true);
    }

    public void HideBuildPanel()
    {
        SetHexCellUI(null);
        animation.Play("BuildingPanelHide");
        isShowBuild = false;
      //  hideBuildButton.gameObject.SetActive(false);

    }



    private void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float targetScale = currentScale + scroll * zoomSpeed;
            targetScale = Mathf.Clamp(targetScale, GetMinScale(), maxScale);
            SetScale(targetScale);
        }
    }

    private void HandleMoveInput()
    {
        Vector2 move = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move.y -= 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move.y += 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move.x += 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move.x -= 1f;

        if (move != Vector2.zero)
        {
            float moveSpeed = 300f;
            hexContainer.anchoredPosition += move * moveSpeed * Time.deltaTime;
        }
    }

    void OnRestButtonClick()
    {
        hexContainer.localPosition = Vector2.zero;
    }

    private void SetScale(float scale)
    {
        currentScale = scale;
        hexContainer.localScale = Vector3.one * currentScale;
    }

    private bool IsPointerOverRect(RectTransform rect)
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out localMousePos);
        return rect.rect.Contains(localMousePos);
    }

    private float GetMinScale()
    {
        float scaleX = viewport.rect.width / hexContainer.sizeDelta.x;
        float scaleY = viewport.rect.height / hexContainer.sizeDelta.y;
        return Mathf.Min(1f, Mathf.Max(scaleX, scaleY));
    }





    public void ShowHexGridUI(RegionValue regionValue,int index)
    {
        cityIndex = index;
        GameValue gameValue = FindObjectOfType<GameValue>();
        this.regionValue = regionValue;
        this.seed = regionValue.GetSeed(index);
        if (this.regionValue.GetHexValue(index) != null && this.regionValue.GetHexValue(index).Count > 0)
        {
            foreach (var kvp in regionValue.GetHexValue(index))
            {
                Vector2Int coord = kvp.Key;
                HexValue hexValue = kvp.Value;
                CreateHex(coord, hexValue);
            }

        }
        else
        {
            InitMap();
        }

        refreshButton.gameObject.transform.parent.gameObject.SetActive(!regionValue.GetIsPlayerBuild(cityIndex));

    }

    public void HideHexGridUI()
    {
        HideBuildPanel();
        Dictionary<Vector2Int, HexValue> saveHexValue = new Dictionary<Vector2Int, HexValue>();
            foreach (var kvp in coordToCell)
            {
                Vector2Int coord = kvp.Key;
                HexValue hexValue = kvp.Value.GetHexValue();
                saveHexValue.Add(coord, hexValue);
            }
            regionValue.SetHexValue(cityIndex,saveHexValue, seed);
        ClearMap();

    }

    public void SetHexCellUI(HexCellUI cellUI)
    {
        if (hexCellUI != null) hexCellUI.StopAlphaLoop();
        hexCellUI = cellUI;
    }

    public HexCellUI GetHexCellUI()
    {
        return hexCellUI;
    }

    public int GetCityIndex()
    {
        return cityIndex;
    }


    public void CreateHex(Vector2Int axial, HexValue hexValue)
    {
        if (coordToCell.TryGetValue(axial, out HexCellUI existingCell))
        {
           
            existingCell.SetHexValue(hexValue,this,gameValue);
            return;
        }

        GameObject hexGO = Instantiate(hexPrefab, hexContainer);
        RectTransform rect = hexGO.GetComponent<RectTransform>();
        rect.anchoredPosition = AxialToPixel(axial);

        HexCellUI cell = hexGO.GetComponent<HexCellUI>();
        if (cell != null)
        {
            cell.hexCoord = axial;
            cell.SetHexValue(hexValue, this, gameValue);
            coordToCell[axial] = cell;
        }
        placedHexes.Add(axial);
    }

    public void GenerateHexGrid(int rings)
    {
        placedHexes.Clear();
        coordToCell.Clear();

        for (int r = 0; r <= rings; r++)
        {
            List<Vector2Int> ring = GetRing(r);
            foreach (var hex in ring)
            {
                CreateHex(hex);
            }
        }
    }


    public void CreateHex(Vector2Int axial)
    {
        if (placedHexes.Contains(axial)) return;

        GameObject hexGO = Instantiate(hexPrefab, hexContainer);
        RectTransform rect = hexGO.GetComponent<RectTransform>();
        rect.anchoredPosition = AxialToPixel(axial);

        HexCellUI cell = hexGO.GetComponent<HexCellUI>();
        cell.SetHexGridUIManager(this);

        if (cell != null)
        {
            cell.hexCoord = axial;

            float elevation = GetElevation(axial);

            float temperature = GetTemperature(axial);

            float humidity = GetHumidity(axial);


            //   float noiseValue = Mathf.PerlinNoise((axial.x + seed) * noiseScale, (axial.y + seed) * noiseScale);
            string terrainType = "Plains";

            if (axial != Vector2Int.zero)
            {
                terrainType = GetTerrainType(elevation, temperature, humidity);
            }

            cell.SetTerrain(terrainType);
            if (IsEdgeHex(axial) && IsLikelyRiverPath(axial))
            {
                cell.SetTerrain("River"); 
            }

            coordToCell[axial] = cell;
        }

        placedHexes.Add(axial);
    }

    public bool HasHexAt(Vector2Int coord)
    {
        return placedHexes.Contains(coord);
    }

    public void AdjustContentSizeAfterAdd()
    {
        int maxRing = 0;
        foreach (var hex in placedHexes)
        {
            int dist = HexDistance(Vector2Int.zero, hex);
            if (dist > maxRing) maxRing = dist;
        }
        AdjustContentSize(maxRing);
    }

    public void AdjustContentSize(int rings)
    {
        float hexWidth = Mathf.Sqrt(3) * hexSize;
        float hexHeight = 2f * hexSize;

        float newWidth = hexWidth * (rings * 2 + 1);
        float newHeight = hexHeight * (rings * 1.5f + 1);

        Vector3 worldCenterBefore = hexContainer.TransformPoint(hexContainer.rect.center);
        hexContainer.sizeDelta = new Vector2(newWidth, newHeight);
        Vector3 worldCenterAfter = hexContainer.TransformPoint(hexContainer.rect.center);
        Vector3 worldOffset = worldCenterBefore - worldCenterAfter;
        hexContainer.anchoredPosition += new Vector2(worldOffset.x, worldOffset.y);
    }

    private int HexDistance(Vector2Int a, Vector2Int b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        int dz = -a.x - a.y + b.x + b.y;
        return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz)) / 2;
    }

    private Vector2 AxialToPixel(Vector2Int axial)
    {
        float width = Mathf.Sqrt(3) * hexSize;
        float height = 2f * hexSize;

        float x = width * (axial.x + axial.y / 2f);
        float y = -height * (3f / 4f) * axial.y;

        return new Vector2(x, y);
    }

    private List<Vector2Int> GetRing(int radius)
    {
        List<Vector2Int> ring = new List<Vector2Int>();
        if (radius == 0)
        {
            ring.Add(Vector2Int.zero);
            return ring;
        }

        Vector2Int hex = directions[4] * radius;

        for (int dir = 0; dir < 6; dir++)
        {
            for (int i = 0; i < radius; i++)
            {
                ring.Add(hex);
                hex += directions[dir];
            }
        }
        return ring;
    }

    public bool IsEdgeHex(Vector2Int coord)
    {
        foreach (var dir in directions)
        {
            Vector2Int neighbor = coord + dir;
            if (!placedHexes.Contains(neighbor))
                return true;
        }
        return false;
    }

    public void UpdateHexAndNeighborsColor(Vector2Int center)
    {
        List<Vector2Int> toUpdate = new List<Vector2Int> { center };
        foreach (var dir in directions)
        {
            toUpdate.Add(center + dir);
        }

        foreach (var coord in toUpdate)
        {
            if (coordToCell.TryGetValue(coord, out var cell))
            {
                if (coord == Vector2Int.zero)
                {
                    cell.SetBuilding("Center");
                }
                else if (IsEdgeHex(coord))
                {
                    cell.SetBuilding("Edge");
                }
                else
                {
                    // cell.SetColor(Color.white);
                }
            }
        }
    }

    public bool GetIsPlayerBuild()
    {
        return regionValue.GetIsPlayerBuild(cityIndex);
    }

    public void SetIsPlayerBuild(bool build)
    {
        regionValue.SetIsPlayerBuild(build,cityIndex);
        refreshButton.gameObject.transform.parent.gameObject.SetActive(!build);

    }


    public void UpdateAllHexBuilding()
    {
        foreach (var pair in coordToCell)
        {
            Vector2Int coord = pair.Key;
            HexCellUI cell = pair.Value;

            if (coord == Vector2Int.zero)
            {
                cell.SetBuilding("Center");
            }
            else if (IsEdgeHex(coord))
            {
                cell.SetBuilding("Edge");
            }
            else
            {
                cell.SetBuilding("Empty");
            }
        }
    }

    // ---- Terrain & River Generation ----

    private System.Random rand = new System.Random();
    private int seed;
    private float noiseScale = 0.05f; // 示例值，根据实际效果调整

    public void RandomSeed()
    {
        seed = System.Environment.TickCount; ; 
        rand = new System.Random(seed);
    }




    private string GetTerrainType(float elevation, float temperature, float humidity)
    {
        if (elevation < 0.1f && regionValue.GetHasSea())
            return "Sea";

        if (elevation < 0.2f)
            return GetLowlandTerrain(temperature, humidity);

        if (elevation < 0.5f)
            return GetPlainsTerrain(temperature, humidity);

        if (elevation < 0.8f)
            return GetHillsTerrain(temperature, humidity);

        return GetMountainTerrain(temperature, humidity);
    }

    private string GetLowlandTerrain(float temperature, float humidity)
    {
        if (temperature > 0.6f && humidity < 0.3f)
            return "Desert";
        if (temperature > 0.4f && humidity > 0.6f)
            return "Wetland";
        if (temperature > 0.3f && humidity > 0.7f)
            return "Swamp";
        if (temperature < 0.3f && humidity < 0.3f)
            return "SnowLand";
        if (temperature < 0.3f && humidity > 0.3f)
            return "Frozen Soil";
        if (humidity > 0.3f && humidity <= 0.6f)
            return "Plains";

        return "Plains";
    }
    private string GetPlainsTerrain(float temperature, float humidity)
    {
        if (temperature > 0.6f && humidity < 0.3f)
            return "Desert";
        if (temperature > 0.4f && humidity > 0.6f)
            return "Forest";
        if (temperature > 0.4f && humidity > 0.5f)
            return "Woods";
        if (temperature > 0.5f && humidity > 0.3f)
            return "Grassland";
        if (temperature > 0.5f && humidity > 0.2f && humidity <= 0.4f)
            return "Shrubland";
        if (temperature < 0.3f && humidity < 0.3f)
            return "Snowland";
        if (temperature < 0.3f && humidity > 0.3f)
            return "Frozen Soil";

        return "Plains";
    }
    private string GetHillsTerrain(float temperature, float humidity)
    {
        if (temperature > 0.6f && humidity < 0.3f)
            return "Desert Hills";
        if (temperature > 0.5f && humidity > 0.2f && humidity <= 0.5f)
            return "Grassland Hills";
        if (temperature > 0.5f && humidity > 0.3f)
            return "Shrubland Hills";
        if (temperature > 0.4f && humidity > 0.6f)
            return "Forest Hills";
        if (temperature > 0.4f && humidity > 0.5f)
            return "Woods Hills";
        if (temperature < 0.5f && humidity > 0.4f)
            return "Coniferous Forest";
        if (temperature < 0.3f && humidity < 0.3f)
            return "SnowLand Hills";
        if (temperature < 0.3f && humidity > 0.3f)
            return "Frozen Soil Hills";

        return "Hills";
    }
    string GetMountainTerrain(float temperature, float humidity) {
        return "Mountain";
    }



    private string GetTerrainType(float noise)
    {
        if (noise < 0.2f) return "Sea";          // 比较大的水域
        else if (noise < 0.3f) return "Swamp";   // 沼泽
        else if (noise < 0.35f) return "Desert"; // 沙漠（比平原稍干燥）
        else if (noise < 0.45f) return "Plains"; // 平原
        else if (noise < 0.55f) return "Grassland"; // 草原（比Plains更绿）
        else if (noise < 0.7f) return "Forest";  // 森林
        else if (noise < 0.8f) return "Hills";   // 丘陵
        else if (noise < 0.9f) return "Mountain"; // 山地
        else return "SnowLand";                   // 雪地
    }
    public void GenerateRivers()
    {
        int riverCount = UnityEngine.Random.Range(regionValue.GetMinRiverCount(), regionValue.GetMinRiverCount());
        for (int i = 0; i < riverCount; i++)
        {
            Vector2Int start = GetRandomHex();
            GenerateSingleRiver(start);
        }
    }

    private Vector2Int GetRandomHex()
    {
        // 过滤非中心，且周围至少有邻居海拔更低的点的格子
        var candidates = placedHexes
            .Where(hex => hex != Vector2Int.zero && HasLowerNeighbor(hex))
            .ToList();

        if (candidates.Count == 0)
        {
            candidates = placedHexes.Where(hex => hex != Vector2Int.zero).ToList();
        }

        candidates.Sort((a, b) => GetElevation(b).CompareTo(GetElevation(a)));

        int selectRange = Mathf.Max(3, candidates.Count / 3);

        return candidates[rand.Next(selectRange)];
    }

    private bool HasLowerNeighbor(Vector2Int hex)
    {
        float selfElevation = GetElevation(hex);
        foreach (var dir in directions)
        {
            Vector2Int neighbor = hex + dir;
            if (placedHexes.Contains(neighbor))
            {
                float neighborElevation = GetElevation(neighbor);
                if (neighborElevation < selfElevation)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private float GetElevation(Vector2Int pos)
    {
        float noise = Mathf.PerlinNoise((pos.x + seed) * noiseScale, (pos.y + seed) * noiseScale);

        return Remap(noise, 0f, 1f, regionValue.GetElevationMIN(), regionValue.GetElevationMAX());
    }

    private float GetTemperature(Vector2Int pos)
    {
        float noise = Mathf.PerlinNoise((pos.x + seed + 1000) * noiseScale, (pos.y + seed + 1000) * noiseScale);

        return Remap(noise, 0f, 1f, regionValue.GetTemperatureMIN(), regionValue.GetTemperatureMAX());
    }

    private float GetHumidity(Vector2Int pos)
    {
        float noise = Mathf.PerlinNoise((pos.x + seed + 2000) * noiseScale, (pos.y + seed + 2000) * noiseScale);

        return Remap(noise, 0f, 1f, regionValue.GetHumidityMIN(), regionValue.GetHumidityMAX());
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }



    private void GenerateSingleRiver(Vector2Int start)
    {
        Vector2Int current = start;

        while (true)
        {
            if (coordToCell.TryGetValue(current, out var cell))
            {

                string terrain = cell.GetTerrain();
                if (terrain == "Swamp" || terrain == "Sea")
                {
                    break; 
                }

                if (current != Vector2Int.zero)
                {
                    cell.SetTerrain("River");
                }
            }

            Vector2Int? next = GetNextRiverStep(current);
            if (next == null)
            {
                break; 
            }

            current = next.Value;
        }
    }

    private bool IsLikelyRiverPath(Vector2Int coord)
    {
        float selfElevation = GetElevation(coord);
        float selfHumidity = GetHumidity(coord);
        float selfTemperature = GetTemperature(coord);

        if (selfHumidity < 0.3f) return false;

        foreach (var dir in directions)
        {
            Vector2Int neighbor = coord + dir;
            if (!placedHexes.Contains(neighbor)) continue;

            float neighborElevation = GetElevation(neighbor);

            if (neighborElevation < selfElevation - 0.05f)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2Int? GetNextRiverStep(Vector2Int current)
    {
        float currentNoise = Mathf.PerlinNoise((current.x + seed) * noiseScale, (current.y + seed) * noiseScale);
        Vector2Int? bestNeighbor = null;
        float bestNoise = currentNoise;

        foreach (var dir in directions)
        {
            Vector2Int neighbor = current + dir;
            if (!placedHexes.Contains(neighbor)) continue;

            float neighborNoise = Mathf.PerlinNoise((neighbor.x + seed) * noiseScale, (neighbor.y + seed) * noiseScale);
            if (neighborNoise < bestNoise)
            {
                bestNoise = neighborNoise;
                bestNeighbor = neighbor;
            }
        }

        return bestNoise < currentNoise ? bestNeighbor : null; // 仅当存在更低噪声时返回
    }
}
