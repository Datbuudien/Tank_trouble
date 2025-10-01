using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private int width;
    private int height;
    private bool[,] walkable;

    private static readonly Vector2Int[] directions = {
        new Vector2Int(1,0), new Vector2Int(-1,0),
        new Vector2Int(0,1), new Vector2Int(0,-1),
        new Vector2Int(1,1), new Vector2Int(1,-1),
        new Vector2Int(-1,1), new Vector2Int(-1,-1)
    };

    public Pathfinding(bool[,] walkable)
    {
        this.walkable = walkable;
        width = walkable.GetLength(0);
        height = walkable.GetLength(1);
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        var openSet = new PriorityQueue<Node>();
        var allNodes = new Dictionary<Vector2Int, Node>();

        Node startNode = GetNode(start, allNodes);
        startNode.g = 0;
        startNode.h = Heuristic(start, goal);
        openSet.Enqueue(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current.position == goal)
                return ReconstructPath(current);

            current.closed = true;

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = current.position + dir;
                if (!IsInside(neighborPos) || !walkable[neighborPos.x, neighborPos.y]) continue;

                Node neighbor = GetNode(neighborPos, allNodes);
                if (neighbor.closed) continue;

                float tentativeG = current.g + 1;

                if (tentativeG < neighbor.g)
                {
                    neighbor.parent = current;
                    neighbor.g = tentativeG;
                    neighbor.h = Heuristic(neighborPos, goal);

                    if (!neighbor.opened)
                    {
                        neighbor.opened = true;
                        openSet.Enqueue(neighbor);
                    }
                }
            }
        }

        return null; // không tìm thấy đường
    }

    private bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Vector2Int.Distance(a, b); // Manhattan
    }

    private Node GetNode(Vector2Int pos, Dictionary<Vector2Int, Node> allNodes)
    {
        if (!allNodes.TryGetValue(pos, out Node node))
        {
            node = new Node(pos);
            allNodes[pos] = node;
        }
        return node;
    }

    private List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.position);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    // ---------- Lớp Node và PriorityQueue ----------
    private class Node : IHeapItem<Node>
    {
        public Vector2Int position;
        public float g = float.MaxValue;
        public float h;
        public Node parent;
        public bool opened;
        public bool closed;

        public float f => g + h;

        public int HeapIndex { get; set; }

        public Node(Vector2Int pos) { position = pos; }

        public int CompareTo(Node other)
        {
            return f.CompareTo(other.f);
        }
    }

    private class PriorityQueue<T> where T : IHeapItem<T>
    {
        private List<T> items = new List<T>();

        public int Count => items.Count;

        public void Enqueue(T item)
        {
            items.Add(item);
            SortUp(item);
        }

        public T Dequeue()
        {
            T firstItem = items[0];
            int lastIndex = items.Count - 1;
            items[0] = items[lastIndex];
            items.RemoveAt(lastIndex);
            if (items.Count > 0) SortDown(items[0]);
            return firstItem;
        }

        private void SortUp(T item)
        {
            int index = items.Count - 1;
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (items[index].CompareTo(items[parentIndex]) < 0)
                {
                    Swap(index, parentIndex);
                    index = parentIndex;
                }
                else break;
            }
        }

        private void SortDown(T item)
        {
            int index = 0;
            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int smallest = index;

                if (left < items.Count && items[left].CompareTo(items[smallest]) < 0) smallest = left;
                if (right < items.Count && items[right].CompareTo(items[smallest]) < 0) smallest = right;

                if (smallest != index)
                {
                    Swap(index, smallest);
                    index = smallest;
                }
                else break;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }

    private interface IHeapItem<T> : System.IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
