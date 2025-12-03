using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static ExcelReader;
using System;

public class CityConnectEditor : MonoBehaviour
{
    public Button ReadButton;
    public Button SaveButton;
    public Button SaveCityPositionButton;

    void Start()
    {
        ReadButton.onClick.AddListener(OnReadButtonClick);
        SaveButton.onClick.AddListener(OnSaveButtonClick);
        SaveCityPositionButton.onClick.AddListener(OnSaveCityPositionButtonClick);


    }

    public LineRenderer linePrefab;

    private CityControl firstCity = null;
    private LineRenderer tempLine = null;

    private List<(CityControl, CityControl, LineRenderer)> connections = new();

    void Update()
    {
        // ???????????
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
              //  Debug.Log($"Hit to {hit.collider.name}");

                CityControl city = hit.collider.GetComponent<CityControl>();
                if (city != null)
                {
                    Debug.Log($"Click city is {city.CityConnectEditorHelper()}");

                    if (firstCity == null)
                    {
                        // ??????????
                        firstCity = city;

                        // ???????
                        tempLine = Instantiate(linePrefab);
                        tempLine.positionCount = 2;
                        tempLine.SetPosition(0, firstCity.transform.position);
                        tempLine.SetPosition(1, firstCity.transform.position);
                    }
                    else if (city != firstCity)
                    {
                        // ??????????
                        tempLine.SetPosition(1, city.transform.position);
                        connections.Add((firstCity, city, tempLine));

                        Debug.Log($"Content {firstCity.CityConnectEditorHelper()} And {city.CityConnectEditorHelper()}");

                        // ??
                        firstCity = null;
                        tempLine = null;
                    }
                }
            }
        }

        // ????????????
        if (Input.GetMouseButtonDown(1))
        {
            if (connections.Count > 0)
            {
                var last = connections[^1]; // C# ^1 ??????
                Destroy(last.Item3.gameObject); // ???
                connections.RemoveAt(connections.Count - 1);
                Debug.Log("Return last work?");
            }
            else
            {
                Debug.Log("don't have last work?");
            }

            // ????????
            firstCity = null;
            if (tempLine != null)
            {
                Destroy(tempLine.gameObject);
                tempLine = null;
            }
        }

        if (firstCity != null && tempLine != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mousePos = ray.GetPoint(distance);
                tempLine.SetPosition(1, mousePos);
            }
        }
    }

    public void OnReadButtonClick()
    {
        string filePath = Path.Combine(Application.dataPath, "StreamingAssets/Value/CityConnectEditor.txt");

        // ??????? LoadCityConnections
        List<CityConnetData> cityConnetDatas = LoadCityConnections(filePath);

        // ???? RegionValue?????? GameValue ???
        List<RegionValue> allRegionValues = GameValue.Instance.GetAllRegionValues();

        foreach (var conn in cityConnetDatas)
        {
            // ? RegionID ?????? region
            if (conn.Region1ID < 0 || conn.Region1ID >= allRegionValues.Count ||
                conn.Region2ID < 0 || conn.Region2ID >= allRegionValues.Count)
            {
                Debug.LogWarning($"Region ID ???{conn.Region1ID} ? {conn.Region2ID}");
                continue;
            }

            Region region1 = allRegionValues[conn.Region1ID].region;
            Region region2 = allRegionValues[conn.Region2ID].region;

            // ? CityID ?? index ?????????? index?
            if (conn.City1ID < 0 || conn.City1ID >= region1.citys.Count ||
                conn.City2ID < 0 || conn.City2ID >= region2.citys.Count)
            {
                Debug.LogWarning($"City ID ???{conn.City1ID} ? {conn.City2ID}");
                continue;
            }

            var city1 = region1.citys[conn.City1ID];
            var city2 = region2.citys[conn.City2ID];

            if (city1 == null || city2 == null)
            {
                Debug.LogWarning($"?????{conn.City1ID} ? {conn.City2ID}");
                continue;
            }

            // ????
            var line = Instantiate(linePrefab);
            line.positionCount = 2;
            line.SetPosition(0, city1.gameObject.transform.position);
            line.SetPosition(1, city2.gameObject.transform.position);

            Debug.Log($"?????{conn.Region1Name} - {conn.City1Name} ? {conn.Region2Name} - {conn.City2Name}");

            // ???????????
            CityControl ctrl1 = city1.gameObject.GetComponent<CityControl>();
            CityControl ctrl2 = city2.gameObject.GetComponent<CityControl>();
            connections.Add((ctrl1, ctrl2, line));
        }

        Debug.Log($"???????? {cityConnetDatas.Count} ????");
    }


    public void SaveConnections()
    {
        string path = Path.Combine(Application.dataPath, "StreamingAssets/Value/CityConnectEditor.txt");

        using (StreamWriter writer = new StreamWriter(path, false)) // false ???????
        {
            foreach (var conn in connections)
            {
                string a = conn.Item1.CityConnectEditorHelper();
                string b = conn.Item2.CityConnectEditorHelper();

                string line = $"{a},{b}";
                writer.WriteLine(line);

                Debug.Log($"Saved Link is ?{line}");
            }
        }

        Debug.Log($"Saced done to?{path}");
    }



    void OnSaveButtonClick()
    {
        SaveConnections();
    }

    // Update is called once per frame

    public void OnSaveCityPositionButtonClick()
    {

        string filePath = Path.Combine(Application.dataPath, "StreamingAssets/Value/CityPosition.txt");


        List<RegionValue> allRegionValues = GameValue.Instance.GetAllRegionValues();

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            foreach (var regionValue in allRegionValues)
            {
                for (int i = 0; i < regionValue.GetCityCountryNum(); i++)
                {
                    int index = i;
                    string line = $"{regionValue.GetRegionENName()},{regionValue.region.citys[index].GetCityENName()},{index},{regionValue.region.citys[index].transform.position.x},{regionValue.region.citys[index].transform.position.y},{regionValue.region.citys[index].transform.position.z}";
                    writer.WriteLine(line);
                }
            }
        }

        Debug.Log($"Saved done to: {filePath}");
    }


}
