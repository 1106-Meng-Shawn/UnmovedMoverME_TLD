using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using static GetSprite;

public class HexCellUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int hexCoord;

    private Button button;
    private HexValue hexValue;
    public Image hexImage;
    public Image building;

    public HexGridUIManager hexGridUIManager;

    public Image hightImage;
    private Coroutine loopCoroutine;

    public GameObject costOj;

    private GameValue gameValue;
    public BuildingValue buildingValue = new BuildingValue();

    public BuildPanelTopRowControl buildPanelTopRowControl;


    public HexValue GetHexValue()
    {
        return hexValue;
    }


    public string GetTerrain()
    {
        return hexValue.terrain;
    }

    private void Awake()
    {
       hexValue = new HexValue();

    }
    private void Start()
    {
        gameValue = GameValue.Instance;
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnHexClicked);
        }
    }

    private void OnHexClicked()
    {


        if (!hexGridUIManager.GetIsPlayerBuild() && !(hexValue.building == "Empty")) {
            hexGridUIManager.SetIsPlayerBuild(true); 
        }

        if (hexValue.building == "Edge")
        {
            ExploitEdge();
            hexGridUIManager.UpdateAllHexBuilding();

            return;
        }

        if (hexGridUIManager.GetHexCellUI() != this && hexValue.building != "Edge") { 

            hexGridUIManager.ShowBuildPanel(this);
            StartAlphaLoop();
            // hexGridUIManager.SetHexCellUI(this);
        }


        // updata sprite
        hexGridUIManager.UpdateAllHexBuilding();
        UpBuildSprite();
        UpTerrainSprite();
    }

    void ExploitEdge()
    {
        if (gameValue.GetResourceValue().Build < buildingValue.GetBuildCost()) return;


        SetBuilding("Empty");
        CreatAroundHex();
        gameValue.GetResourceValue().Build -= buildingValue.GetBuildCost();
        if (buildPanelTopRowControl == null) Debug.Log("whyyyy????");
        buildPanelTopRowControl.UpUI();
        hexValue.building = null;
        costOj.SetActive(false);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // GetComponent<Image>().color = Color.red;
        if (hexValue.building == "Edge")
        {
            BuildingValue buildingValue = new BuildingValue();
            ShowCost(buildingValue);
        }

        hightImage.gameObject.SetActive(true);
        hightImage.color = new Color(hightImage.color.r, hightImage.color.g, hightImage.color.b,0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
        if (hexGridUIManager.GetHexCellUI() != this) hightImage.gameObject.SetActive(false);

        if (hexValue.building == "Edge")
        {
            costOj.gameObject.SetActive(false);
        }

    }

    void StartAlphaLoop()
    {
        SetHightImage();
        if (loopCoroutine == null && hightImage != null)
        {
            loopCoroutine = StartCoroutine(LoopAlpha());
        }
    }

    void SetHightImage()
    {
        if (hexGridUIManager.GetCityIndex() == 0)
        {
            hightImage.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Build/building");
        } else
        {
            hightImage.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Build/buildingCity");
        }

    }

    void SetCenterImage()
    {
        if (hexGridUIManager.GetCityIndex() == 0)
        {
            building.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Build/Center");
        }
        else
        {
            building.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Build/CenterCity");
        }

    }


    public void StopAlphaLoop()
    {
        hightImage.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Build/Hex");
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;

            Color color = hightImage.color;
            color.a = 1f;
            hightImage.color = color;
        }
        hightImage.gameObject.SetActive(false);
    }

    private IEnumerator LoopAlpha()
    {
        float duration = 1f;
        float minAlpha = 0f;
        float maxAlpha = 127f / 255f;
        float elapsedTime = 0f;

        while (true)
        {
            elapsedTime += Time.deltaTime;
            float normalized = Mathf.Sin((elapsedTime / duration) * Mathf.PI * 2f) * 0.5f + 0.5f;

            Color color = hightImage.color;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, normalized);
            hightImage.color = color;

            yield return null;
        }
    }

    private IEnumerator FadeAlpha(float from, float to, float time)
    {
        float elapsed = 0f;
        Color color = hightImage.color;

        while (elapsed < time)
        {
            float t = elapsed / time;
            color.a = Mathf.Lerp(from, to, t);
            hightImage.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = to;
        hightImage.color = color;
    }


    void CreatAroundHex()
    {
        Vector2Int[] directions = new Vector2Int[]
{
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1)
};

        foreach (var dir in directions)
        {
            Vector2Int neighbor = hexCoord + dir;
            if (!hexGridUIManager.HasHexAt(neighbor))
            {
                hexGridUIManager.CreateHex(neighbor);
                hexGridUIManager.AdjustContentSizeAfterAdd();
            }
        }


    }


    public void ShowCost(BuildingValue buildingValue)
    {
        if (buildingValue == null)
        {
            costOj.SetActive(false);
            return;
        }

        costOj.SetActive(true);
        costOj.GetComponentInChildren<Image>().sprite = GetValueSprite(buildingValue.GetBuildCostType());
        TextMeshProUGUI text = costOj.GetComponentInChildren<TextMeshProUGUI>();
        text.text = buildingValue.GetBuildCost().ToString("N0");

        float playerHad = 0;
        if (buildingValue.GetBuildCostType() == "Build")
        {
            if (gameValue == null) gameValue = FindObjectOfType<GameValue>();
            playerHad = (float)gameValue.GetResourceValue().Build;
        }

        if (playerHad >= buildingValue.GetBuildCost()) { text.color = Color.black; }
        else { text.color = Color.red; }
    }

    public void SetBuilding(string buildingType)
    {

        hexValue.building = buildingType;
        UpBuildSprite();
    }

    void UpBuildSprite()
    {
        if (!string.IsNullOrEmpty(hexValue.building) && hexValue.building != "Empty") { 
            building.gameObject.SetActive(true);
        }else { building.gameObject.SetActive(false); 
        }

        if (hexValue.building == "Center")
        {
            SetCenterImage(); 
        } else
        {
            building.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Build/{hexValue.building}");

        }


    }



    public void SetTerrain(string terrainType)
    {
        hexValue.terrain = terrainType;
        UpTerrainSprite();

    }

    void UpTerrainSprite()
    {
        hexImage.sprite = Resources.Load<Sprite>($"MyDraw/UI/Region/Terrain/{hexValue.terrain}");

    }

    public void SetHexValue(HexValue hexValue,HexGridUIManager hexGridUIManager,GameValue gameValue)
    {
        this.hexValue = hexValue;
        SetHexGridUIManager(hexGridUIManager);
        this.gameValue = gameValue;
        UpBuildSprite();
        UpTerrainSprite();

    }

    public void SetHexGridUIManager(HexGridUIManager hexGridUIManager)
    {
        this.hexGridUIManager = hexGridUIManager;
        this.buildPanelTopRowControl = hexGridUIManager.buildPanelTopRowControl;
    }

}

public class HexValue
{
    public bool isEdge;
    public string terrain;
    public string building;

    public HexValue()
    {
        isEdge = false;
        terrain = null;
        building = null;
    }
}
