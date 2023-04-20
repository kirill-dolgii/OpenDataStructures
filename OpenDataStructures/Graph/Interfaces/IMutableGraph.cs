namespace OpenDataStructures.Graph.Interfaces;

public interface IMutableGraph<TNode, TEdge> : IGraph<TNode, TEdge>
{
    public void AddNode(TNode node);
    public bool RemoveNode(TNode node);
    public void AddEdge(TNode x, TNode y, TEdge edge);
    public bool RemoveEdge(TNode x, TNode y, TEdge edge);
}