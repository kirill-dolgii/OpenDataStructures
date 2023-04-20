namespace DataStructures.Graph.Interfaces;

public interface IGraph<TNode, TEdge>
{
    public ICollection<TNode> Nodes { get; }
    public ICollection<TNode> AdjacentNodes(TNode node);
    public bool ContainsNode(TNode node);
    public ICollection<TEdge> Edges(TNode x, TNode y);
    public ICollection<TEdge> Edges(TNode x);
    public ICollection<(TNode x, TNode y, TEdge edge)> Edges();
    public bool ContainsEdge(TNode x, TNode y, TEdge edge);
    public int Degree(TNode node);
    public int EdgesCount { get; }
    public int NodesCount { get; }
    public bool Directed { get; }
    public IEqualityComparer<TNode> NodeEqualityComparer { get; }
    public IEqualityComparer<TEdge> EdgeEqualityComparer { get; }

}
