using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Direction
{
    up = 0,
    left = 1,
    down = 2,
    right = 3
};

public class DungeonCrawlerController : MonoBehaviour
{
    public static List<Vector2Int> positionsVisited = new List<Vector2Int>();
    private static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new Dictionary<Direction, Vector2Int>
    {
        {Direction.up, new Vector2Int(0, 1)},
        {Direction.left, new Vector2Int(-1, 0)},
        {Direction.down, new Vector2Int(0, -1)},
        {Direction.right, new Vector2Int(1, 0)}
    };

    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData)
    {
        positionsVisited.Clear(); // Effacer les positions précédentes
        List<DungeonCrawler> dungeonCrawlers = new List<DungeonCrawler>();

        for (int i = 0; i < dungeonData.numberOfCrawlers; i++)
        {
            dungeonCrawlers.Add(new DungeonCrawler(GetRandomStartPosition()));
        }

        int iterations = Random.Range(dungeonData.iterationMin, dungeonData.iterationMax);

        for (int i = 0; i < iterations; i++)
        {
            foreach (DungeonCrawler dungeonCrawler in dungeonCrawlers)
            {
                Vector2Int newPos = dungeonCrawler.Move(directionMovementMap);
                positionsVisited.Add(newPos);
            }
        }

        return positionsVisited.Distinct().ToList(); // Assurer des positions uniques
    }

    // Fonction pour obtenir une position de départ aléatoire dans les limites du donjon
    private static Vector2Int GetRandomStartPosition()
    {
        return new Vector2Int(Random.Range(int.MinValue, int.MaxValue), Random.Range(int.MinValue, int.MaxValue));
    }
}

public class DungeonCrawler
{
    public Vector2Int Position { get; set; }

    public DungeonCrawler(Vector2Int startPos)
    {
        Position = startPos;
    }

    public Vector2Int Move(Dictionary<Direction, Vector2Int> directionMovementMap)
    {
        Direction toMove = (Direction)Random.Range(0, directionMovementMap.Count);
        Vector2Int newPos = Position + directionMovementMap[toMove];

        Position = newPos;
        return Position;
    }
}
