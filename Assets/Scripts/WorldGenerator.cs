using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    [TitleGroup("Tiles")] 


    private readonly Dictionary<int, Vector2Int> _directions = new()
    {
        { -1, Vector2Int.zero },
        { 0, Vector2Int.up },
        { 1, Vector2Int.right },
        { 2, Vector2Int.down },
        { 3, Vector2Int.left }
    };

    private Vector2Int _portalPosition;
    private Tilemap Tilemap => GameManager.Instance.tilemap;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GeneratePaths());
    }
    
    private IEnumerator GeneratePaths()
    {
        yield return StartCoroutine(MakeNodes(GameManager.Instance.maxNodeCount, -1, Vector2Int.zero));
        yield return StartCoroutine(FillInGrass());
    }


    private IEnumerator FillInGrass()
    {
        foreach (Vector3Int position in Tilemap.cellBounds.allPositionsWithin)
        {
            if (Tilemap.GetTile(position) == null)
            {
                Tilemap.SetTile(position, GameManager.Instance.land);
            }

            yield return null;
        }
        
        GameManager.Instance.StartGame();
    }

    private IEnumerator MakeNodes(int remainingNodes, int direction, Vector2Int position)
    {
        while (remainingNodes > 0)
        {
            if (remainingNodes == GameManager.Instance.maxNodeCount)
            {
                Tilemap.SetTile(new Vector3Int(0, 0, 0), GameManager.Instance.portal);
                direction = Random.Range(0, 4);
                _portalPosition = position = new Vector2Int(0, 0);
                remainingNodes -= 1;
            }
            else
            {
                int x;
                int y;
                int safety = 0;
                int dir = direction;
                do
                {
                    direction = GetNextDirection(dir);
                    Vector2Int currentDirection = _directions[direction];
                    int distance = Random.Range(0, GameManager.Instance.maxDistance) * 2 + 1;
                    x = position.x + currentDirection.x * distance;
                    y = position.y + currentDirection.y * distance;
                    safety++;
                } while
                    (((position.x < _portalPosition.x && x > _portalPosition.x && position.y == _portalPosition.y) ||
                      (position.y < _portalPosition.y && y > _portalPosition.y && position.x == _portalPosition.x) ||
                      Tilemap.GetTile(new Vector3Int(x, y, 0)) != GameManager.Instance.land) && safety < 5);

                GameManager.pathNodes.Add(new Vector3Int(x, y, 0));
                Tilemap.SetTile(new Vector3Int(x, y, 0), remainingNodes == 1 ? GameManager.Instance.castle : GameManager.Instance.path);
                yield return StartCoroutine(BuildPath(position.x, position.y, x, y));
                position = new Vector2Int(x, y);
                remainingNodes -= 1;
            }

            yield return null;
        }
    }

    private IEnumerator BuildPath(int x1, int y1, int x2, int y2)
    {
        if (x1 == x2)
        {
            int startY = y1 < y2 ? y1 : y2;
            int endY = y1 > y2 ? y1 : y2;
            for (int y = startY; y < endY; y++)
            {
                if (Tilemap.GetTile(new Vector3Int(x1, y, 0)) == null) Tilemap.SetTile(new Vector3Int(x1, y, 0), GameManager.Instance.path);
                yield return null;
            }
        }
        else if (y1 == y2)
        {
            int startX = x1 < x2 ? x1 : x2;
            int endX = x1 > x2 ? x1 : x2;
            for (int x = startX; x < endX; x++)
            {
                if (Tilemap.GetTile(new Vector3Int(x, y1, 0)) == null) Tilemap.SetTile(new Vector3Int(x, y1, 0), GameManager.Instance.path);
                yield return null;
            }
        }
    }

    private int GetNextDirection(int currentDirection)
    {
        return currentDirection switch
        {
            0 => new[] { 0, 1, 3 }[Random.Range(0, 3)],
            1 => new[] { 0, 1, 2 }[Random.Range(0, 3)],
            2 => new[] { 1, 2, 3 }[Random.Range(0, 3)],
            3 => new[] { 0, 2, 3 }[Random.Range(0, 3)],
            _ => -1
        };
    }
}