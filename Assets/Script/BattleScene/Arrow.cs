using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GetColor;

public class Arrow : MonoBehaviour
{
    public GameObject reticleBlock;
    public GameObject reticleArrow;

    public int MaxCount = 100;
    public Vector3 StartPoint;
    private Vector3 controlPoint1;
    private Vector3 controlPoint2;
    private Vector3 endPoint;
    private List<GameObject> ArrowItemList;
    private List<Image> ImageItemList;
    private Animator Arrow_anim;

    private bool isEnemy = true;
    private bool isSelect;
    private Skill skill;
    private Vector2 currentMouse;
    private bool isAtBattleField;

    public bool IsSelect
    {
        get => isSelect;
        set
        {
            if (isSelect != value)
            {
                isSelect = value;
                if (value == true)
                {
                    PlayAnim();
                }
            }
        }
    }

    private void Awake()
    {
        InitData();
    }

    private void InitData()
    {
        ArrowItemList = new List<GameObject>();
        ImageItemList = new List<Image>();

        for (int i = 0; i < MaxCount; i++)
        {
            GameObject Arrow = (i == MaxCount - 1) ? reticleArrow : reticleBlock;
            GameObject temp = Instantiate(Arrow, this.transform);

            Image imageComponent = temp.GetComponentInChildren<Image>();
            if (imageComponent != null)
            {
                ImageItemList.Add(imageComponent);
            }
            if (i == MaxCount - 1)
            {
                Arrow_anim = temp.GetComponent<Animator>();
            }

            ArrowItemList.Add(temp);
        }

        currentMouse = Input.mousePosition;
    }

    private void Update()
    {
        Move();

        DrawBezierCurve();

        if (Input.GetMouseButtonDown(1)) Cance();
    }

    public void Cance()
    {
        Cursor.visible = true;
        Mouse.current.WarpCursorPosition(currentMouse);
        Destroy(gameObject);
    }

    private void DrawBezierCurve()
    {
        // ????????????t??
        int sampleCount = 100;
        PrepareArcLengthTable(sampleCount, out List<float> arcLengths, out List<float> ts);
        float totalLength = arcLengths[arcLengths.Count - 1];

        for (int i = 0; i < ArrowItemList.Count; i++)
        {
            // ????
            float targetDistance = totalLength * i / (ArrowItemList.Count - 1);
            float t = GetTForDistance(targetDistance, arcLengths, ts);

            Vector3 position = CalculateBezierPoint(t, StartPoint, controlPoint1, controlPoint2, endPoint);
            ArrowItemList[i].transform.position = position;

            ArrowItemList[i].transform.localScale = Vector3.one * (t / 2f) + Vector3.one * 0.3f;

            if (i > 0)
            {
                float signedAngle = Vector2.SignedAngle(Vector2.up,
                    ArrowItemList[i].transform.position - ArrowItemList[i - 1].transform.position);
                ArrowItemList[i].transform.rotation = Quaternion.Euler(0, 0, signedAngle);
            }
        }
    }

    private void PrepareArcLengthTable(int sampleCount, out List<float> arcLengths, out List<float> ts)
    {
        arcLengths = new List<float>(sampleCount);
        ts = new List<float>(sampleCount);

        Vector3 prevPoint = CalculateBezierPoint(0, StartPoint, controlPoint1, controlPoint2, endPoint);
        arcLengths.Add(0);
        ts.Add(0);

        float totalLength = 0;

        for (int i = 1; i < sampleCount; i++)
        {
            float t = i / (float)(sampleCount - 1);
            Vector3 currentPoint = CalculateBezierPoint(t, StartPoint, controlPoint1, controlPoint2, endPoint);
            totalLength += Vector3.Distance(prevPoint, currentPoint);
            arcLengths.Add(totalLength);
            ts.Add(t);
            prevPoint = currentPoint;
        }
    }

    private float GetTForDistance(float targetDistance, List<float> arcLengths, List<float> ts)
    {
        int index = arcLengths.FindIndex(length => length >= targetDistance);
        if (index == -1) return 1f; // ???????1

        if (index == 0) return ts[0];

        float lengthBefore = arcLengths[index - 1];
        float lengthAfter = arcLengths[index];
        float tBefore = ts[index - 1];
        float tAfter = ts[index];

        float factor = (targetDistance - lengthBefore) / (lengthAfter - lengthBefore);
        return Mathf.Lerp(tBefore, tAfter, factor);
    }

    public void Move()
    {
        Vector3 worldPosition = Vector3.zero;

        if (!isEnemy && !isAtBattleField)
        {
            Vector3 mousePosition = Input.mousePosition;
            worldPosition = mousePosition;
            worldPosition.z = 2f;
            endPoint = worldPosition;
        }
        else
        {
            worldPosition = endPoint;
        }

        if (StartPoint == endPoint)
        {
            controlPoint1 = (Vector2)StartPoint + new Vector2(150f, 200f) * controlPoint1Scale;
            controlPoint2 = (Vector2)StartPoint + new Vector2(150f, 200f) * controlPoint2Scale;
        }
        else
        {
            controlPoint1 = (Vector2)StartPoint + (worldPosition - StartPoint) * controlPoint1Scale;
            controlPoint2 = (Vector2)StartPoint + (worldPosition - StartPoint) * controlPoint2Scale;
        }

        controlPoint1.z = StartPoint.z;
        controlPoint2.z = StartPoint.z;
    }

    public void SetStartPos(Vector3 pos, Skill skill, bool isEnemy, bool isAtBattleField, Vector3 enemySkillEnd)
    {
        StartPoint = pos;
        this.skill = skill;
        this.isEnemy = isEnemy;
        this.isAtBattleField = isAtBattleField;

        if (isEnemy || isAtBattleField)
        {
            endPoint = enemySkillEnd;
        }
        else if (!isEnemy && !isAtBattleField)
        {
            endPoint = Vector3.zero;
        }

        SetControlPointScale(skill, isEnemy, isAtBattleField);
    }



    private void SetControlPointScale(Skill skillBase, bool isEnemy, bool isAtBattleField)
    {
        switch (skillBase.rangeType)
        {
            case SkillRangeType.Melee:
                controlPoint1Scale = new Vector2(0f, 0f);
                controlPoint2Scale = new Vector2(0f, 0f);
                break;
            case SkillRangeType.Ranged:
                if (!isEnemy)
                {
                    controlPoint1Scale = isAtBattleField ? new Vector2(0.5f, 1f) : new Vector2(0f, 1f);
                    controlPoint2Scale = isAtBattleField ? new Vector2(-0.5f, 1f) : new Vector2(0.5f, 2f);
                }
                else
                {
                    controlPoint1Scale = isAtBattleField ? new Vector2(-0.5f, -1f) : new Vector2(0f, 0f);
                    controlPoint2Scale = isAtBattleField ? new Vector2(0.5f, -1f) : new Vector2(0f, 2f);
                }
                break;
            default:
                Debug.LogWarning($"Unknown skill: {skill.GetSkillENName()}");
                break;
                
        }
    }





      public void SetColor(Color color, bool isSelect)
      {
          if (ImageItemList == null || ImageItemList.Count == 0) return;

          foreach (var img in ImageItemList)
          {
              if (img != null)
              {
                  Color targetColor = new Color(color.r, color.g, color.b, isSelect ? 1f : 0f);
                  img.color = targetColor;

                  // ???????? Image ??
                  if (img.transform.childCount > 0)
                  {
                      Transform child = img.transform.GetChild(0);
                      Image childImg = child.GetComponent<Image>();
                      if (childImg != null)
                      {
                          childImg.color = color;
                      }
                  } else
                  {
                      img.color = new Color(color.r, color.g, color.b, 1f);
                  }
              }
          }
      }
      

    public void SetColor(bool isSelect)
    {
        IsSelect = isSelect;
        SetColor(GetSkillFunctionTypeColor(skill.functionType),isSelect);
    }

    private void PlayAnim()
    {
        if (Arrow_anim == null) return;
        Arrow_anim.SetTrigger("select");
    }

    public static Vector3 CalculateBezierPoint(float t, Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * startPoint;
        point += 3 * uu * t * controlPoint1;
        point += 3 * u * tt * controlPoint2;
        point += ttt * endPoint;

        return point;
    }

    public Vector2 controlPoint1Scale = new Vector2(0f, 0.1f);
    public Vector2 controlPoint2Scale = new Vector2(1f, 1.5f);
}
