using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;
    private List<Vector2Int> dungeonRooms;
    private Graph graph;
    void Start()
    {
       
    
        graph = new Graph();
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        SpawnRooms(dungeonRooms);
        
    }

    void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {

        // Création du graphe
        Graph graph = new Graph();

        // Ajouter les nœuds
        graph.AddNode("Start");
        graph.AddNode("Empty");
        graph.AddNode("End");
        graph.AddNode("scene");
        graph.AddNode("scene2");


        // Ajouter des arêtes entre les nœuds de manière aléatoire
        AddRandomEdges(graph);
       
       graph.AddEdge("Empty", "End", 2); 
       graph.AddEdge("End", "scene2", 2);   
       graph.AddEdge("scene", "scene2", 2); 
       graph.AddEdge("scene", "Empty", 2);  

        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        // BFS à partir d'un nœud aléatoire parmi "Start", "End", "Empty", "scene", ou "scene2"
        string[] startnodeKeys = { "Start", "Empty", "scene", "scene2" };
        var bfsResult = graph.BFS(startnodeKeys).Take(dungeonRooms.Count()).ToList();

        foreach (var nodeKey in bfsResult)
        {
            Vector2Int roomLocation = nodeKey.Item2;

            // Vérifier si la position est déjà utilisée
            while (usedPositions.Contains(roomLocation))
            {
                // Choisir une nouvelle position
                roomLocation += graph.GetRandomDirection();
            }

            // Marquer la position comme utilisée
            usedPositions.Add(roomLocation);

            // Charge la salle en utilisant le type de salle indiqué par BFS
            RoomController.instance.LoadRoom(nodeKey.Item1, roomLocation.x, roomLocation.y);
        
        }
        foreach (var node in bfsResult)
        {
            graph.StoreBfsResult(node.Item1, node.Item2);
        }



    
       // Calculer les distances avec Dijkstra depuis "Empty"
      
      var (path, distance) = graph.DijkstraFindPathFromStartToEnd("Empty", "End");
  

    }
        void AddRandomEdges(Graph graph)
    {
    // Récupérer les clés des nœuds du graphe
    List<string> nodeKeys = graph.GetNodeKeys().ToList();

    // Vérifier qu'il y a au moins cinq nœuds
    if (nodeKeys.Count < 5)
    {
        Debug.LogError("Le graphe doit avoir au moins cinq nœuds.");
        return;
    }

    // Assurer les connexions de base et ajouter des connexions supplémentaires de manière aléatoire
    foreach (var node in nodeKeys)
    {
        foreach (var otherNode in nodeKeys)
        {
            if (node != otherNode && !graph.AreConnected(node, otherNode))
            {
                int randomWeight = UnityEngine.Random.Range(1, 10); // Poids aléatoire entre 1 et 10
                graph.AddEdge(node, otherNode, randomWeight);
            }
        }
    }
   }

    
    
}

public class Graph
{
    private Dictionary<string, List<(string, int)>> adjList = new Dictionary<string, List<(string, int)>>();
     private Dictionary<string, Vector2Int> bfsResults = new Dictionary<string, Vector2Int>();

    public void AddNode(string node)
    {
        if (!adjList.ContainsKey(node))
        {
            adjList[node] = new List<(string, int)>();
        }
    }


     public bool AreConnected(string u, string v)
    {
        if (!adjList.ContainsKey(u) || !adjList.ContainsKey(v))
        {
            return false;
        }
        return adjList[u].Any(edge => edge.Item1 == v);
    }

    public IEnumerable<string> GetNodeKeys()
    {
        return adjList.Keys;
    }


    public void AddEdge(string u, string v, int weight)
    {
        if (!adjList.ContainsKey(u))
        {
            adjList[u] = new List<(string, int)>();
        }
        adjList[u].Add((v, weight));

        // Since it's an undirected graph, add edge from v to u as well
        if (!adjList.ContainsKey(v))
        {
            adjList[v] = new List<(string, int)>();
        }
        adjList[v].Add((u, weight));
    }
     public void StoreBfsResult(string nodeKey, Vector2Int location)
    {
        // Store BFS result in the graph
        bfsResults[nodeKey] = location;
    }

    public IEnumerable<(string, Vector2Int)> BFS(IEnumerable<string> startNodes)
    {
        HashSet<string> visited = new HashSet<string>();
        Queue<(string, Vector2Int)> queue = new Queue<(string, Vector2Int)>();

        // Convertir la liste de départ en tableau pour utiliser Length
        string[] startNodesArray = startNodes.ToArray();

        // Vérifier si la liste des nœuds de départ est vide
        if (startNodesArray.Length == 0)
        {
            Debug.LogError("Aucun nœud de départ disponible pour le BFS.");
            yield break; // Sortir de la méthode
        }

        // Choix aléatoire d'un nœud de départ parmi la liste fournie
        string randomStartNode = startNodesArray[UnityEngine.Random.Range(0, startNodesArray.Length)];

        // Enfilement du nœud de départ avec ses coordonnées (0, 0)
        queue.Enqueue((randomStartNode, new Vector2Int(0, 0)));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentNode = current.Item1;
            var currentLocation = current.Item2;

            if (visited.Contains(currentNode))
            {
                continue;
            }

            visited.Add(currentNode);
            Debug.Log("Visited node: " + currentNode + " at location: " + currentLocation); // Imprimer le nœud visité
            yield return current;

            if (adjList.ContainsKey(currentNode))
            {
                foreach (var (neighbor, _) in adjList[currentNode])
                {
                    Vector2Int newLocation = currentLocation + GetRandomDirection();
                    queue.Enqueue((neighbor, newLocation));
                }
            }
        }
    }
    public static List<string> path = new List<string>();
   public (List<string> path, int distance) DijkstraFindPathFromStartToEnd(string startNode, string endNode)
    {
        // Dijkstra's algorithm logic
        Dictionary<string, int> distances = new Dictionary<string, int>();
        Dictionary<string, string> previous = new Dictionary<string, string>();
        HashSet<string> unvisitedNodes = new HashSet<string>();

        // Initialize distances
        foreach (var node in adjList.Keys)
        {
            distances[node] = int.MaxValue;
            previous[node] = null;
            unvisitedNodes.Add(node);
        }

        distances[startNode] = 0;

        while (unvisitedNodes.Count > 0)
        {
            string currentNode = null;
            foreach (var node in unvisitedNodes)
            {
                if (currentNode == null || distances[node] < distances[currentNode])
                {
                    currentNode = node;
                }
            }

            if (currentNode == null || currentNode == endNode)
            {
                break;  // Exit if we reached the end node
            }

            unvisitedNodes.Remove(currentNode);

            if (adjList.ContainsKey(currentNode))
            {
                foreach (var (neighbor, weight) in adjList[currentNode])
                {
                    int alt = distances[currentNode] + weight;
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = currentNode;
                    }
                }
            }
        }

        // Construire le chemin parcouru
        
        string currentNodeInPath = endNode;
        while (currentNodeInPath != null)
        {
            path.Add(currentNodeInPath);
            currentNodeInPath = previous[currentNodeInPath];
        }
        path.Reverse();

        // Afficher le chemin dans la console
        Debug.Log("Chemin trouvé : " + string.Join(" -> ", path));
        Debug.Log("Distance = " + distances[endNode]);

        return (path, distances[endNode]);
    }

    public Vector2Int GetRandomDirection()
    {
        // Génère une direction aléatoire parmi les quatre directions cardinales
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
        return directions[UnityEngine.Random.Range(0, directions.Length)];
    }
}


