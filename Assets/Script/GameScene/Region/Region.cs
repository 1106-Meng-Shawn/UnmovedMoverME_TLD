using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GetColor;

public class Region : MonoBehaviour
{
    public int regionID;

    private RegionValue regionValue;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer landforms;
    public bool hasLandforms = false;

    private Coroutine blinkCoroutine;
    private bool isBlinking = false;
    public SpriteRenderer highLightSprite;

    private Color originalColor;

    public GameValue gameValue;
    public List<Color> countryColor;

    public GameObject hintPrefab;

    private PolygonCollider2D regionCollider;


  //  public List<GameObject> cityObjects;
      public List<CityControl> citys = new List<CityControl>();
    public List<GameObject> Hints;
    public float cameraMoveSpeed = CameraConstants.CameraMoveDuration;
    public OutlineSprite outline;



    private void Start()
    {
        landforms.gameObject.SetActive(hasLandforms);
    }
    public void ApplyZoomVisibility(float zoom)
    {
        for (int i = 0; i < regionValue.GetCityCountryNum(); i++)
        {
            if (citys[i] != null)
                citys[i].ApplyZoomVisibility(zoom);
        }
    }
    private void OnDestroy()
    {
        StopBlinking();
        if (regionValue != null)
        {
            regionValue.OnDestroy();
        }
        
    }

    void HideCity()
    {
        for (int i = 0; i < regionValue.GetCityCountryNum(); i++)
        {
            if (citys[i] != null)
                citys[i].HideCity();
        }

    }

    void ShowMainCity()
    {
        for (int i = 0; i < regionValue.GetCityCountryNum(); i++)
        {
            if (citys[i] != null)
                citys[i].ShowMainCity();
        }
    }

    void ShowCity()
    {
        for (int i = 0; i < regionValue.GetCityCountryNum(); i++)
        {
            if (citys[i] != null)
                citys[i].ShowCity();
        }
    }




    public RegionValue GetRegionValue()
    {
        return regionValue;
    }


    public void SetRegionValue()
    {
        gameValue = GameValue.Instance;
        if (gameValue.GetRegionValue(regionID) == null)
        {
            Debug.LogWarning($"Invalid regionID: {regionID} for GameObject: {gameObject.name}, gameValue region count {gameValue.GetAllRegionValues().Count}");
            return;
        }
        regionValue = gameValue.GetRegionValue(regionID);
        regionValue.region = this;
        SetupCollider();
        SetupSprite();
        SetRegionColor();
        SetCityName();
    }

    private void SetupCollider()
    {
        regionCollider = GetComponent<PolygonCollider2D>();
        if (regionCollider == null)
        {
            regionCollider = gameObject.AddComponent<PolygonCollider2D>();
        }

        regionCollider.isTrigger = true;
        regionCollider.layerOverridePriority = 6;
    }


    private void SetupSprite()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && highLightSprite != null)
        {
            highLightSprite.sprite = spriteRenderer.sprite;
        }
        else
        {
            Debug.LogWarning($"Missing SpriteRenderer or highLightSprite on {gameObject.name}");
        }
    }


    void SetCityName()
    {
        List<string> citysNames = regionValue.GetCityNames();
        for (int i = citysNames.Count; i < citys.Count; i++)
        {
            Destroy(citys[i].gameObject);
        }

        int extra = citys.Count - citysNames.Count;
        if (extra > 0)
        {
            citys.RemoveRange(citysNames.Count, extra);
        }

        for (int i = 0; i < citysNames.Count; i++)
        {
            int index = i;
            citys[index].SetCityValue(regionValue.GetCityValue(index));
        }
    }



    public  void SetRegionColor(){
        if (spriteRenderer == null) return;
        originalColor = GetRegionColor();
        spriteRenderer.color = GetRegionColor();

    }

    public Color32 GetRegionColor()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        return countryManager.GetCountryColor(regionValue.GetCityCountry(0));
    }


    public Color GetCountryColorByRegion()
    {
        CountryManager countryManager = GameValue.Instance.GetCountryManager();
        if (countryManager.HasOverlord(regionValue.GetCityCountry(0)))
        {
            string overLordName = countryManager.GetOverlord(regionValue.GetCityCountry(0));
            return countryManager.GetCountry(overLordName).GetColor();
        }
        else
        {
            return countryManager.GetCountry(regionValue.GetCityCountry(0)).GetColor();
        }
    }


    public Color32 TransfCountryNameToColor(string countryName)
    {
        switch (countryName)
        {
            case "Holy Romulus Empire": return new Color32(255, 216, 52, 255);
            case "Drakensberg Mountain Range": return new Color32(139, 69, 19, 255);
            case "Sahara": return new Color32(255, 223, 127, 255);
            case "Siberia": return new Color32(228, 228, 228, 255);
            case "Cyprus": return new Color32(239, 127, 26, 255);

            default: return new Color32(0,0,0,255);
        }
    }

//    RegionManager regionManager;

    public void ShoudCreatHit(){
       /* for (int i = 0; i < regionValue.shouldCreatTips.Count ; i ++){
            if (regionValue.shouldCreatTips[i]) CreatTips(i);
        }*/

        if (regionValue.IsReduced) CreatTips(1);

    }




    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(regionValue.GetRegionENName() + "happend with " + collision.gameObject.GetComponent<Region>().regionValue.GetRegionENName());
    }



    private Vector3 mouseDownPos;

    void OnMouseDown()
    {
        mouseDownPos = Input.mousePosition;

    }

    void OnMouseUp()
    {
        spriteRenderer.color = originalColor;

        if (IsPointerOverUI()) return;

        float distance = Vector3.Distance(mouseDownPos, Input.mousePosition);
        if (distance < 5f) 
        {
            ShowRegionPanel();
            ApplySelectionEffect();
        }
    }


    void OnMouseEnter()
    {
        if (IsPointerOverUI()) return;

        Darken();
    }

    void OnMouseExit()
    {
        RestoreColor();
    }


    // ----------------------------
    // 功能模块一：判断是否点击在UI上
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // ----------------------------
    // 功能模块二：显示描边 + 变暗
    public void ApplySelectionEffect()
    {
        SetCountryOutline();

       /* if (spriteRenderer != null && spriteRenderer.color == originalColor)
        {
            Color c = spriteRenderer.color;
            float darkenFactor = 0.7f;
            spriteRenderer.color = new Color(c.r * darkenFactor, c.g * darkenFactor, c.b * darkenFactor, c.a);
        }*/
    }

    private void Darken()
    {
        if (spriteRenderer != null && spriteRenderer.color == originalColor)
        {
            Color c = spriteRenderer.color;
            float darkenFactor = 0.7f;
            spriteRenderer.color = new Color(c.r * darkenFactor, c.g * darkenFactor, c.b * darkenFactor, c.a);
        }
    }

    private void RestoreColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }


    public void SetCountryOutline()
    {
       List<RegionValue> allRegionValues = GameValue.Instance.GetAllRegionValues();

        foreach (var regionValue in allRegionValues)
        {
            regionValue.region.CloseOutline();
            if (regionValue.GetCountryENName() == this.regionValue.GetCountryENName()) {
                regionValue.region.SetRegionOutline();
            }
        }
        Highlight(true);
    }

    public void SetRegionOutline()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sortingOrder = 11;
        outline.spriteRenderer.sortingOrder = 10;
        //outline.SetOutLine(GetColor.GetCountryColor(regionValue.GetCountry()));
        outline.SetOutLine(GetCountryColorByRegion());
    }


    public void CloseOutline()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sortingOrder = 2;
        outline.spriteRenderer.sortingOrder = 2;
        outline.CloseOutLine();
        Highlight(false);

    }

    public void CloseAllCountryOutline()
    {
        foreach (var regionValue in GameValue.Instance.GetAllRegionValues())
        {
            regionValue.region.CloseOutline();
        }
    }


    // ----------------------------
    // 功能模块三：显示己方/敌方面板
    public void ShowRegionPanel()
    {
        //   if (regionValue.GetCountry() == gameValue.PlayerCountry || regionValue.GetCityCountry(0) == gameValue.PlayerCountry)
        if (regionValue.GetCityValue(0).IsShowRegionPanel())
        {
            RegionInfo.Instance.ShowPanel(regionValue);
            DestroyTips();
        }
        else
        {
            //playerRegionInfos[1].ShowPanel(regionValue);
            UnplayerRegionInfo.Instance.OpenPanel(regionValue);

        }
    }

    public void ShowRegionPanel(int cityIndex)
    {
        if (regionValue.GetCityValues().Count == 0)
        {
            ShowRegionPanel();
            return;
        }


        if (regionValue.GetCountryENName() == gameValue.GetPlayerCountryENName())
        {
            RegionInfo.Instance.ShowPanel(regionValue, cityIndex);
            DestroyTips();
        }
        else
        {
            UnplayerRegionInfo.Instance.OpenPanel(regionValue, cityIndex);

        }
    }


    public void CloseRegionPanel()
    {
        if (regionValue.GetCityValue(0).IsShowRegionPanel())
        {
            RegionInfo.Instance.ClosePanel();
        }
        else
        {
            UnplayerRegionInfo.Instance.ClosePanel();

        }
    }



    // ----------------------------
    // 功能模块四：判断双击，进行相机跳转
    private void CheckDoubleClick()
    {
        ShowRegionPanel(0);
        ZoomToCity(0);
    }


    public void ZoomToCity(int index)
    {
        if (index < 0) return;
        if (CameraControl2D.Instance == null || citys == null || citys.Count == 0 || citys[index] == null)
            return;

        Vector3 cityPos = citys[index].transform.position;
        CameraControl2D.Instance.SmoothMoveAndZoom(cityPos, 4f, cameraMoveSpeed);
    }


    public void DestroyTips()
    {
     /*   for (int i = 0; i < regionValue.shouldCreatTips.Count ; i ++){
        
            regionValue.shouldCreatTips[i] = false;
        }*/
        regionValue.IsReduced = false;

        for (int i = 0; i < Hints.Count ; i ++){
            Destroy(Hints[i]);
        }
        Hints.Clear();
    }


    public void CreatTips(int num)
{
    // 获取Collider2D的边界
    Bounds colliderBounds = regionCollider.bounds;

        Vector2 center = colliderBounds.center;

    // 计算圆的半径为中间50%区域的10%
    float radius = (colliderBounds.size.x * 0.5f) * 0.1f;  // 10% of the width of the middle area

    // 定义一个变量来保存随机生成的位置
    Vector2 randomPosition;

    do
    {
        // 生成一个随机位置在以中心为原点的圆内
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);  // 随机角度
        float distance = UnityEngine.Random.Range(0f, radius);  // 随机距离
        randomPosition = new Vector2(
            center.x + distance * Mathf.Cos(angle),  // 使用极坐标转换为笛卡尔坐标
            center.y + distance * Mathf.Sin(angle)
        );
    }
    while (!regionCollider.OverlapPoint(randomPosition));  // 如果位置不在Collider2D内，继续随机


    // 在有效的位置生成提示对象
    GameObject hint = Instantiate(hintPrefab, randomPosition, Quaternion.identity);
    hint.transform.SetParent(transform, false);  

    // 调用生成提示的控制方法
    hint.GetComponent<HintControl>().GenerateReductionTip(num);

    // 调整生成物体的大小
    AdjustHintSize(hint, colliderBounds);

    Hints.Add(hint);
}
  

    private void AdjustHintSize(GameObject hint, Bounds colliderBounds)
    {
        float regionWidth = colliderBounds.size.x;
        float regionHeight = colliderBounds.size.y;

        float mid = (colliderBounds.size.x + colliderBounds.size.y) /2;

        float scaleFactor = 0.05f;
        hint.transform.localScale = new Vector3(mid * scaleFactor, mid * scaleFactor, 1);
    }

    public void Highlight(bool on)
    {
        if (on)
        {
            StartBlinking();
        }
        else
        {
            StopBlinking();
        }
    }


    public void StartBlinking()
    {
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(LoopAlpha());
            highLightSprite.gameObject.SetActive(true);

        }
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            highLightSprite.gameObject.SetActive(false);
        }
    }

    private IEnumerator LoopAlpha()
    {
        float duration = 0.5f; // 
        float minAlpha = 0f;
        float maxAlpha = 127f / 255f;

        while (true)
        {
            float time = Time.time % duration;
            float normalized = Mathf.Sin((time / duration) * Mathf.PI * 2f) * 0.5f + 0.5f;

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, normalized);

            Color color = highLightSprite.color;
            color.a = alpha;
            highLightSprite.color = color;

            yield return null;
        }
    }

    public void SetCityPosition(int cityIndex,Vector3 newPosition)
    {
        if (cityIndex < 0 || cityIndex >= citys.Count)
        {
            Debug.Log($"out of {cityIndex} cityIndex in region");
            return;
        }

        citys[cityIndex].SetPosition(newPosition,transform.localScale);
    }

}
