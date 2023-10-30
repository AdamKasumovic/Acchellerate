using System.Collections.Generic;
using UnityEngine;

public class PolygonUtility
{
    public static bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
    {
        bool isInside = false;
        int j = polygon.Count - 1;

        for (int i = 0; i < polygon.Count; i++)
        {
            if (polygon[i].y < point.y && polygon[j].y >= point.y || polygon[j].y < point.y && polygon[i].y >= point.y)
            {
                if (polygon[i].x + (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < point.x)
                {
                    isInside = !isInside;
                }
            }
            j = i;
        }

        return isInside;
    }

    public static List<Vector2> SamplePointsInsidePolygon(List<Vector2> polygon, int numPoints)
    {
        List<Vector2> sampledPoints = new List<Vector2>();

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        // Compute bounding box
        foreach (Vector2 vertex in polygon)
        {
            minX = Mathf.Min(minX, vertex.x);
            maxX = Mathf.Max(maxX, vertex.x);
            minY = Mathf.Min(minY, vertex.y);
            maxY = Mathf.Max(maxY, vertex.y);
        }

        // Sample points
        int numSampled = 0;
        while (numSampled < numPoints)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector2 sampledPoint = new Vector2(x, y);

            if (IsPointInsidePolygon(sampledPoint, polygon))
            {
                sampledPoints.Add(sampledPoint);
                numSampled++;
            }
        }

        return sampledPoints;
    }
}
