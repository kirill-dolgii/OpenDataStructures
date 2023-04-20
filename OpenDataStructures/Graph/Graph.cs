using System.Collections.Generic;
using System.Linq;
using OpenDataStructures.Graph.Interfaces;
using OpenDataStructures.Misc;

namespace OpenDataStructures.Graph;

public class Graph<TNode, TEdge> : IMutableGraph<TNode, TEdge> where TNode : notnull
{
    private readonly IDictionary<TNode, Collections.Hashing.HashSet<TNode>> _adjacent;
    private readonly IDictionary<(TNode x, TNode y), Collections.Hashing.HashSet<TEdge>> _edges;
    public IEqualityComparer<TNode> NodeEqualityComparer { get; }
    public IEqualityComparer<TEdge> EdgeEqualityComparer { get; }
    private int _edgesCount;

    /// <summary>
    /// Constructs an empty Graph object with default EqualityComparer for TEdge and TNode.
    /// </summary>
    public Graph(bool directed) : this(new Dictionary<TNode, ICollection<TNode>>(), 
                                       new Dictionary<(TNode, TNode), ICollection<TEdge>>(), 
                                       EqualityComparer<TNode>.Default, 
                                       EqualityComparer<TEdge>.Default, 
                                       directed) {}

    private Graph(IDictionary<TNode, ICollection<TNode>> adjacent, 
                  IDictionary<(TNode, TNode), ICollection<TEdge>> edges, 
                  IEqualityComparer<TNode> nodeEqualityComparer, 
                  IEqualityComparer<TEdge> edgeEqualityComparer, 
                  bool directed)
    {
        NodeEqualityComparer = nodeEqualityComparer;
        _adjacent = adjacent.ToDictionary(kv => kv.Key, 
                                          kv => new Collections.Hashing.HashSet<TNode>(kv.Value, NodeEqualityComparer), 
                                          NodeEqualityComparer);

        EdgeEqualityComparer = edgeEqualityComparer;
        IEqualityComparer<(TNode x, TNode y)> nodeTupleEqualityComparer = new CustomEqualityComparer<(TNode x, TNode y)>(
            (tpl1, tpl2) =>
                NodeEqualityComparer.Equals(tpl1.x, tpl2.x) &&
                NodeEqualityComparer.Equals(tpl1.y, tpl2.y),
            tpl => NodeEqualityComparer.GetHashCode(tpl.x) + 2 * NodeEqualityComparer.GetHashCode(tpl.y));
        _edges = edges.ToDictionary(kv => kv.Key, 
                                    kv => new Collections.Hashing.HashSet<TEdge>(kv.Value, EdgeEqualityComparer), 
                                    nodeTupleEqualityComparer);

        Directed = directed;
    }

    public ICollection<TNode> AdjacentNodes(TNode node) => _adjacent[node];

    public ICollection<TEdge> Edges(TNode x, TNode y) => _edges[(x, y)];

    public ICollection<TEdge> Edges(TNode x) => _adjacent[x].SelectMany(y => _edges[(x, y)]).ToList();

    public ICollection<(TNode x, TNode y, TEdge edge)> Edges()
        => _edges.SelectMany(kv => kv.Value.Select(edge => (kv.Key.x, kv.Key.y, edge))).ToList();

    public int EdgesCount => Directed ? _edgesCount : _edgesCount / 2;

    public bool Directed { get; }

    public void AddNode(TNode node)
    {
        if (ContainsNode(node)) return;
        _adjacent[node] = new Collections.Hashing.HashSet<TNode>(NodeEqualityComparer);
    }

    public bool ContainsNode(TNode node) => _adjacent.ContainsKey(node);

    public bool RemoveNode(TNode node)
    {
        if (!ContainsNode(node)) return false;
        foreach (var kv in _edges.Where(kv => NodeEqualityComparer.Equals(kv.Key.y, node)))
        foreach (var edge in kv.Value)
            RemoveEdge(kv.Key.x, kv.Key.y, edge);
        _adjacent.Remove(node);
        return true;
    } 

    public int Degree(TNode node) => _adjacent[node].Count;

    public void AddEdge(TNode x, TNode y, TEdge edge) => AddEdgeImpl(x, y, edge, Directed);

    private void AddEdgeImpl(TNode x, TNode y, TEdge edge, bool directed)
    {
        if (ContainsEdge(x, y, edge)) return;
        if (!ContainsNode(x)) AddNode(x);
        if (!ContainsNode(y)) AddNode(y);
        _edges[(x, y)] = new Collections.Hashing.HashSet<TEdge>(EdgeEqualityComparer)
        {
            edge
        };
        _adjacent[x].Add(y);
        _edgesCount++;
        if (!directed) AddEdgeImpl(y, x, edge, true);
    }

    public bool ContainsEdge(TNode x, TNode y, TEdge edge) => 
        _edges.ContainsKey((x, y)) && _edges[(x, y)].Contains(edge);

    public bool RemoveEdge(TNode x, TNode y, TEdge edge) => RemoveEdgeImpl(x, y, edge, Directed);

    private bool RemoveEdgeImpl(TNode x, TNode y, TEdge edge, bool directed)
    {
        if (!ContainsEdge(x, y, edge)) return false;
        _edges[(x, y)].Remove(edge);
        if (_edges[(x, y)].Count == 0) _edges.Remove((x, y));
        _edgesCount--;
        if (!directed) RemoveEdgeImpl(y, x, edge, true);
        return true;
    }

    public ICollection<TNode> Nodes => _adjacent.Keys;
    public int NodesCount => _adjacent.Count;
}