using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    public Color outlineColor = Color.cyan;
    public float outlineWidth = 0.05f;
    public int sortingOrder = 2;
    public bool liveUpdate = false;

    private SpriteRenderer spriteRenderer;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Texture2D readableTexture;
    private Sprite processedSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateOutlines();
    }

    void Update()
    {
        if (liveUpdate && spriteRenderer.sprite != processedSprite)
        {
            ClearOutlines();
            CreateOutlines();
        }
    }

    private Vector3[] ConvertToLocalPoints(List<Vector2> points, Sprite sprite)
    {
        Vector3[] localPoints = new Vector3[points.Count];
        float pixelsPerUnit = sprite.pixelsPerUnit;
        Rect rect = sprite.rect;
        Vector2 pivot = sprite.pivot;

        // 计算纹理在原始图片中的偏移量
        float textureOffsetX = rect.x - pivot.x;
        float textureOffsetY = rect.y - pivot.y;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            // 修正后的坐标转换公式
            localPoints[i] = new Vector3(
                (point.x + textureOffsetX) / pixelsPerUnit,
                (point.y + textureOffsetY) / pixelsPerUnit,
                0);
        }

        return localPoints;
    }

    private void CreateLineRenderer(Vector3[] points)
    {
        GameObject lineObj = new GameObject("SpriteOutlinePart");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = outlineColor;
        lr.endColor = outlineColor;
        lr.startWidth = outlineWidth;
        lr.endWidth = outlineWidth;
        lr.useWorldSpace = false;
        lr.sortingOrder = sortingOrder;
        lr.loop = true;
        lr.positionCount = points.Length;
        lr.SetPositions(points);

        lineRenderers.Add(lr);
    }

    public void CreateOutlines()
    {
        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null) return;

        // 创建可读的纹理副本
        CreateReadableTexture(sprite);

        // 找到所有边缘点
        List<Vector2> edgePoints = FindTrueAlphaEdges(readableTexture, sprite.rect);

        if (edgePoints.Count == 0)
        {
            Debug.LogWarning("No outline edge found");
            return;
        }

        // 找到所有独立的轮廓
        List<List<Vector2>> allContours = FindAllContours(edgePoints);

        // 为每个独立轮廓创建LineRenderer
        foreach (var contour in allContours)
        {
            if (contour.Count < 3) continue;

            // 转换为局部坐标（修正后的方法）
            Vector3[] localPoints = ConvertToLocalPoints(contour, sprite);

            // 创建LineRenderer
            CreateLineRenderer(localPoints);
        }

        processedSprite = sprite;
    }

    private void CreateReadableTexture(Sprite sprite)
    {
        Texture2D originalTexture = sprite.texture;

        if (originalTexture.isReadable)
        {
            readableTexture = originalTexture;
            return;
        }

        readableTexture = new Texture2D(
            (int)sprite.rect.width,
            (int)sprite.rect.height,
            originalTexture.format,
            originalTexture.mipmapCount > 1);

        Color[] pixels = originalTexture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height);

        readableTexture.SetPixels(pixels);
        readableTexture.Apply();
    }

    private List<Vector2> FindTrueAlphaEdges(Texture2D texture, Rect spriteRect)
    {
        List<Vector2> edges = new List<Vector2>();
        int width = (int)spriteRect.width;
        int height = (int)spriteRect.height;

        Color[] pixels = texture.GetPixels(0, 0, width, height);

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Color current = pixels[y * width + x];

                if (current.a > 0.5f)
                {
                    if (pixels[(y - 1) * width + x].a < 0.5f || // 上
                        pixels[(y + 1) * width + x].a < 0.5f || // 下
                        pixels[y * width + (x - 1)].a < 0.5f || // 左
                        pixels[y * width + (x + 1)].a < 0.5f)   // 右
                    {
                        edges.Add(new Vector2(x, y));
                    }
                }
            }
        }

        return edges;
    }

    private List<List<Vector2>> FindAllContours(List<Vector2> edgePoints)
    {
        List<List<Vector2>> allContours = new List<List<Vector2>>();
        HashSet<Vector2> remainingPoints = new HashSet<Vector2>(edgePoints);

        while (remainingPoints.Count > 0)
        {
            var enumerator = remainingPoints.GetEnumerator();
            enumerator.MoveNext();
            Vector2 start = enumerator.Current;

            List<Vector2> contour = TraceContour(start, remainingPoints);

            if (contour.Count >= 3)
            {
                allContours.Add(contour);
            }
        }

        return allContours;
    }

    private List<Vector2> TraceContour(Vector2 start, HashSet<Vector2> remainingPoints)
    {
        List<Vector2> contour = new List<Vector2>();
        Vector2 current = start;
        Vector2 previous = current - Vector2.right;
        int safetyCounter = 0;
        int maxIterations = remainingPoints.Count * 2;

        do
        {
            contour.Add(current);
            remainingPoints.Remove(current);

            Vector2 next = Vector2.zero;
            int startDir = DirectionToIndex(previous - current);

            for (int i = 0; i < 8; i++)
            {
                int dir = (startDir + 7 - i) % 8;
                Vector2 test = current + DirectionFromIndex(dir);

                if (remainingPoints.Contains(test))
                {
                    next = test;
                    break;
                }
            }

            if (next == Vector2.zero) break;

            previous = current;
            current = next;
            safetyCounter++;
        }
        while (current != start && safetyCounter < maxIterations);

        return contour;
    }

 

    private int DirectionToIndex(Vector2 dir)
    {
        if (dir == Vector2.right) return 0;
        if (dir == (Vector2.right + Vector2.up)) return 1;
        if (dir == Vector2.up) return 2;
        if (dir == (Vector2.left + Vector2.up)) return 3;
        if (dir == Vector2.left) return 4;
        if (dir == (Vector2.left + Vector2.down)) return 5;
        if (dir == Vector2.down) return 6;
        return 7;
    }

    private Vector2 DirectionFromIndex(int index)
    {
        switch (index)
        {
            case 0: return Vector2.right;
            case 1: return Vector2.right + Vector2.up;
            case 2: return Vector2.up;
            case 3: return Vector2.left + Vector2.up;
            case 4: return Vector2.left;
            case 5: return Vector2.left + Vector2.down;
            case 6: return Vector2.down;
            default: return Vector2.right + Vector2.down;
        }
    }

    public void ClearOutlines()
    {
        foreach (var lr in lineRenderers)
        {
            if (lr != null && lr.gameObject != null)
            {
                Destroy(lr.gameObject);
            }
        }
        lineRenderers.Clear();
    }

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        foreach (var lr in lineRenderers)
        {
            if (lr != null)
            {
                lr.startColor = color;
                lr.endColor = color;
            }
        }
    }

    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        foreach (var lr in lineRenderers)
        {
            if (lr != null)
            {
                lr.startWidth = width;
                lr.endWidth = width;
            }
        }
    }
}



/*using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutline : MonoBehaviour
{
    /* public SpriteRenderer spriteRenderer; // 设置需要绘制边缘的 Sprite
     public Transform parentTransform;

     private void Start()
     {
         Sprite sprite = spriteRenderer.sprite;
         MakeTextureReadable(sprite);
         GenerateOutlineFromSprite(sprite, parentTransform);
     }

     private void MakeTextureReadable(Sprite sprite)
     {
         // 获取 Sprite 对应的纹理
         Texture2D texture = sprite.texture;

         // 如果纹理还没有设置为可读，则修改它的设置
         if (!texture.isReadable)
         {
             // 创建一个新的 Texture2D 实例，并将原始纹理的像素复制到新的纹理中
             Texture2D readableTexture = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);

             // 将原始纹理的像素数据复制到新的纹理
             readableTexture.SetPixels(texture.GetPixels());
             readableTexture.Apply();  // 应用更改

             // 将新的纹理应用到 Sprite
             sprite = Sprite.Create(readableTexture, sprite.rect, sprite.pivot);
         }
     }


     private void GenerateOutlineFromSprite(Sprite sprite, Transform parentTransform)
     {
         // 获取纹理和矩形区域
         Texture2D texture = sprite.texture;
         Rect spriteRect = sprite.textureRect;
         int width = (int)spriteRect.width;
         int height = (int)spriteRect.height;

         // 获取纹理的像素数据
         Color[] pixels = texture.GetPixels(
             (int)spriteRect.x,
             (int)spriteRect.y,
             width,
             height
         );

         List<Vector2> edgePixels = new List<Vector2>();

         // 遍历像素，查找非透明且边缘的像素
         for (int y = 1; y < height - 1; y++)
         {
             for (int x = 1; x < width - 1; x++)
             {
                 Color pixel = pixels[y * width + x];

                 // 是非透明且接近黑色的像素
                 if (pixel.a > 0.1f && pixel.r < 0.2f && pixel.g < 0.2f && pixel.b < 0.2f)
                 {
                     bool isEdge = false;

                     // 检查周围的像素是否透明或非黑色
                     for (int dy = -1; dy <= 1 && !isEdge; dy++)
                     {
                         for (int dx = -1; dx <= 1 && !isEdge; dx++)
                         {
                             if (dx == 0 && dy == 0) continue;

                             Color neighbor = pixels[(y + dy) * width + (x + dx)];

                             if (neighbor.a < 0.1f || neighbor.r > 0.2f || neighbor.g > 0.2f || neighbor.b > 0.2f)
                             {
                                 isEdge = true;
                             }
                         }
                     }

                     if (isEdge)
                     {
                         // 将像素坐标转为局部坐标（考虑pivot和textureRect偏移）
                         float px = x + spriteRect.x - sprite.pivot.x;
                         float py = y + spriteRect.y - sprite.pivot.y;
                         Vector3 localPos = new Vector3(px, py, 0) / sprite.pixelsPerUnit;
                         edgePixels.Add(localPos);
                     }
                 }
             }
         }

         if (edgePixels.Count == 0)
         {
             Debug.LogWarning("No outline edge found");
             return;
         }

         // 使用边缘像素进行顺序排列
         List<Vector2> orderedEdgePixels = OrderEdgePixels(edgePixels, width);

         // 创建一个新的 GameObject 用于 LineRenderer
         GameObject lineObj = new GameObject("AlphaOutline");
         lineObj.transform.SetParent(parentTransform);

         LineRenderer lr = lineObj.AddComponent<LineRenderer>();
         lr.material = new Material(Shader.Find("Sprites/Default"));
         lr.startColor = lr.endColor = Color.cyan;
         lr.startWidth = lr.endWidth = 0.05f;
         lr.useWorldSpace = true;
         lr.sortingOrder = 2;

         // 创建闭合路径，确保最后一个点连接到第一个点
         Vector3[] worldPoints = new Vector3[orderedEdgePixels.Count + 1];
         for (int i = 0; i < orderedEdgePixels.Count; i++)
         {
             Vector2 local = orderedEdgePixels[i];
             worldPoints[i] = parentTransform.TransformPoint(local);
         }

         // 闭合路径
         worldPoints[worldPoints.Length - 1] = worldPoints[0];

         lr.positionCount = worldPoints.Length;
         lr.SetPositions(worldPoints);
     }

     private List<Vector2> OrderEdgePixels(List<Vector2> edgePixels, int width)
     {
         List<Vector2> orderedPixels = new List<Vector2>();
         Vector2 currentPixel = edgePixels[0];
         orderedPixels.Add(currentPixel);

         edgePixels.RemoveAt(0); // 删除已使用的像素

         while (edgePixels.Count > 0)
         {
             Vector2 nextPixel = FindNearestNeighbor(currentPixel, edgePixels);
             orderedPixels.Add(nextPixel);
             edgePixels.Remove(nextPixel);
             currentPixel = nextPixel;
         }

         return orderedPixels;
     }

     private Vector2 FindNearestNeighbor(Vector2 currentPixel, List<Vector2> edgePixels)
     {
         Vector2 nearestPixel = edgePixels[0];
         float minDistance = Vector2.Distance(currentPixel, nearestPixel);

         foreach (Vector2 pixel in edgePixels)
         {
             float distance = Vector2.Distance(currentPixel, pixel);
             if (distance < minDistance)
             {
                 nearestPixel = pixel;
                 minDistance = distance;
             }
         }

         return nearestPixel;
     }

[Header("Outline Settings")]
    public Color outlineColor = Color.cyan;
    public float outlineWidth = 0.05f;
    public int sortingOrder = 2;
    public bool liveUpdate = false;

    private SpriteRenderer spriteRenderer;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Texture2D readableTexture;
    private Sprite processedSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateOutlines();
    }

    void Update()
    {
        if (liveUpdate && spriteRenderer.sprite != processedSprite)
        {
            ClearOutlines();
            CreateOutlines();
        }
    }

    public void CreateOutlines()
    {
        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null) return;

        // 创建可读的纹理副本
        CreateReadableTexture(sprite);

        // 找到所有边缘点
        List<Vector2> edgePoints = FindTrueAlphaEdges(readableTexture, sprite.rect);

        if (edgePoints.Count == 0)
        {
            Debug.LogWarning("No outline edge found");
            return;
        }

        // 找到所有独立的轮廓
        List<List<Vector2>> allContours = FindAllContours(edgePoints);

        // 为每个独立轮廓创建LineRenderer
        foreach (var contour in allContours)
        {
            if (contour.Count < 3) continue; // 需要至少3个点形成闭合轮廓

            // 转换为局部坐标
            Vector3[] localPoints = ConvertToLocalPoints(contour, sprite);

            // 创建LineRenderer
            CreateLineRenderer(localPoints);
        }

        processedSprite = sprite;
    }

    private void CreateLineRenderer(Vector3[] points)
    {
        GameObject lineObj = new GameObject("SpriteOutlinePart");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = outlineColor;
        lr.endColor = outlineColor;
        lr.startWidth = outlineWidth;
        lr.endWidth = outlineWidth;
        lr.useWorldSpace = false;
        lr.sortingOrder = sortingOrder;
        lr.loop = true;
        lr.positionCount = points.Length;
        lr.SetPositions(points);

        lineRenderers.Add(lr);
    }

    private void CreateReadableTexture(Sprite sprite)
    {
        Texture2D originalTexture = sprite.texture;

        if (originalTexture.isReadable)
        {
            readableTexture = originalTexture;
            return;
        }

        readableTexture = new Texture2D(
            (int)sprite.rect.width,
            (int)sprite.rect.height,
            originalTexture.format,
            originalTexture.mipmapCount > 1);

        Color[] pixels = originalTexture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height);

        readableTexture.SetPixels(pixels);
        readableTexture.Apply();
    }

    private List<Vector2> FindTrueAlphaEdges(Texture2D texture, Rect spriteRect)
    {
        List<Vector2> edges = new List<Vector2>();
        int width = (int)spriteRect.width;
        int height = (int)spriteRect.height;

        Color[] pixels = texture.GetPixels(0, 0, width, height);

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Color current = pixels[y * width + x];

                if (current.a > 0.5f)
                {
                    if (pixels[(y - 1) * width + x].a < 0.5f || // 上
                        pixels[(y + 1) * width + x].a < 0.5f || // 下
                        pixels[y * width + (x - 1)].a < 0.5f || // 左
                        pixels[y * width + (x + 1)].a < 0.5f)   // 右
                    {
                        edges.Add(new Vector2(x, y));
                    }
                }
            }
        }

        return edges;
    }

    private List<List<Vector2>> FindAllContours(List<Vector2> edgePoints)
    {
        List<List<Vector2>> allContours = new List<List<Vector2>>();
        HashSet<Vector2> remainingPoints = new HashSet<Vector2>(edgePoints);

        while (remainingPoints.Count > 0)
        {
            // 获取一个起始点
            var enumerator = remainingPoints.GetEnumerator();
            enumerator.MoveNext();
            Vector2 start = enumerator.Current;

            // 追踪这个轮廓
            List<Vector2> contour = TraceContour(start, remainingPoints);

            if (contour.Count >= 3) // 需要至少3个点形成闭合轮廓
            {
                allContours.Add(contour);
            }
        }

        return allContours;
    }

    private List<Vector2> TraceContour(Vector2 start, HashSet<Vector2> remainingPoints)
    {
        List<Vector2> contour = new List<Vector2>();
        Vector2 current = start;
        Vector2 previous = current - Vector2.right; // 初始方向向右
        int safetyCounter = 0;
        int maxIterations = remainingPoints.Count * 2;

        do
        {
            contour.Add(current);
            remainingPoints.Remove(current);

            // 8方向搜索顺序（从上一个方向开始逆时针）
            Vector2 next = Vector2.zero;
            int startDir = DirectionToIndex(previous - current);

            for (int i = 0; i < 8; i++)
            {
                int dir = (startDir + 7 - i) % 8; // 逆时针搜索
                Vector2 test = current + DirectionFromIndex(dir);

                if (remainingPoints.Contains(test))
                {
                    next = test;
                    break;
                }
            }

            if (next == Vector2.zero) break; // 没有找到下一个点

            previous = current;
            current = next;
            safetyCounter++;
        }
        while (current != start && safetyCounter < maxIterations);

        return contour;
    }

    private Vector3[] ConvertToLocalPoints(List<Vector2> points, Sprite sprite)
    {
        Vector3[] localPoints = new Vector3[points.Count];
        float pixelsPerUnit = sprite.pixelsPerUnit;
        Vector2 pivot = sprite.pivot;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            localPoints[i] = new Vector3(
                (point.x - pivot.x) / pixelsPerUnit,
                (point.y - pivot.y) / pixelsPerUnit,
                0);
        }

        return localPoints;
    }

    private int DirectionToIndex(Vector2 dir)
    {
        if (dir == Vector2.right) return 0;
        if (dir == (Vector2.right + Vector2.up)) return 1;
        if (dir == Vector2.up) return 2;
        if (dir == (Vector2.left + Vector2.up)) return 3;
        if (dir == Vector2.left) return 4;
        if (dir == (Vector2.left + Vector2.down)) return 5;
        if (dir == Vector2.down) return 6;
        return 7;
    }

    private Vector2 DirectionFromIndex(int index)
    {
        switch (index)
        {
            case 0: return Vector2.right;
            case 1: return Vector2.right + Vector2.up;
            case 2: return Vector2.up;
            case 3: return Vector2.left + Vector2.up;
            case 4: return Vector2.left;
            case 5: return Vector2.left + Vector2.down;
            case 6: return Vector2.down;
            default: return Vector2.right + Vector2.down;
        }
    }

    public void ClearOutlines()
    {
        foreach (var lr in lineRenderers)
        {
            if (lr != null && lr.gameObject != null)
            {
                Destroy(lr.gameObject);
            }
        }
        lineRenderers.Clear();
    }

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        foreach (var lr in lineRenderers)
        {
            if (lr != null)
            {
                lr.startColor = color;
                lr.endColor = color;
            }
        }
    }

    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        foreach (var lr in lineRenderers)
        {
            if (lr != null)
            {
                lr.startWidth = width;
                lr.endWidth = width;
            }
        }
    }
}*/