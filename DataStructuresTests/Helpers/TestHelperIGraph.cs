using System;
using System.Collections.Generic;
using System.Linq;
using OpenDataStructures.Graph;
using OpenDataStructures.Graph.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenDataStructures.Tests.Helpers;

public static class TestHelperIGraph
{

    [TestMethod]
    public static void CONTAINS_NODE_SUCCESSFUL<TNode, TEdge>(Graph<TNode, TEdge> graph,
        TNode addingNode) where TNode : notnull
    {
        if (graph.NodesCount > 0) throw new InvalidOperationException("graph must be empty.");
        graph.AddNode(addingNode);
        Assert.IsTrue(graph.ContainsNode(addingNode));

    }

    [TestMethod]
    public static void CONTAINS_NODE_UNSUCCESSFUL<TNode, TEdge>(Graph<TNode, TEdge> graph,
        TNode checkingNode) where TNode : notnull
    {
        if (graph.NodesCount > 0) throw new InvalidOperationException("graph must be empty.");
        Assert.IsFalse(graph.ContainsNode(checkingNode));
    }

    public static void ADD_SINGLE_NODE_SUCCESSFUL<TNode, TEdge>(IMutableGraph<TNode, TEdge> graph, TNode x)
        where TNode : notnull
    {
        if (graph.ContainsNode(x)) throw new InvalidOperationException("graph already contains the specified node.");
        var count = graph.NodesCount;
        graph.AddNode(x);
        Assert.IsTrue(graph.ContainsNode(x));
        Assert.AreEqual(count + 1, graph.NodesCount);
    }

    public static void REMOVE_SINGLE_NODE_SUCCESSFUL<TNode, TEdge>(IMutableGraph<TNode, TEdge> graph, TNode x)
        where TNode : notnull
    {
        if (!graph.ContainsNode(x)) throw new InvalidOperationException("graph doesn't contain specified node.");
        var count = graph.NodesCount;
        Assert.IsTrue(graph.RemoveNode(x));
        Assert.IsFalse(graph.ContainsNode(x));
        Assert.AreEqual(count - 1, graph.NodesCount);
    }

    public static void REMOVE_NODE_SUCCESSFUL<TNode, TEdge>(IMutableGraph<TNode, TEdge> graph, TNode x)
    {
        if (graph.Degree(x) == 0)
            throw new InvalidOperationException("there must be outcoming edges from specified node");
        var adjacentNodes = graph.AdjacentNodes(x);
        var edges = adjacentNodes.SelectMany(y => graph.Edges(x, y).Select(edge => new { X = x, Y = y, Edge = edge }))
            .ToList();
        var backEdges = adjacentNodes
            .SelectMany(y => graph.Edges(y, x).Select(edge => new { X = y, Y = x, Edge = edge })).ToList();

        var count = graph.NodesCount;
        Assert.IsTrue(graph.RemoveNode(x));
        Assert.IsFalse(graph.ContainsNode(x));
        Assert.AreEqual(count - 1, graph.NodesCount);
        foreach (var edge in edges)
            Assert.IsFalse(graph.ContainsEdge(edge.X, edge.Y, edge.Edge));
        foreach (var backEdge in backEdges)
            Assert.IsFalse(graph.ContainsEdge(backEdge.X, backEdge.Y, backEdge.Edge));
    }

    public static void REMOVE_UNDIRECTED_EDGE_SUCCESSFUL<TNode, TEdge>(IMutableGraph<TNode, TEdge> graph,
        TNode x, TNode y, TEdge edge)
    {
        if (!graph.ContainsEdge(x, y, edge))
            throw new InvalidOperationException("graph must contain specified edge.");
        if (graph.Directed) throw new InvalidOperationException("graph must be undirected.");
        var edgeCount = graph.EdgesCount;
        Assert.IsTrue(graph.RemoveEdge(x, y, edge));
        Assert.IsFalse(graph.ContainsEdge(x, y, edge));
        Assert.IsFalse(graph.ContainsEdge(y, x, edge));
        Assert.AreEqual(edgeCount - 1, graph.EdgesCount);
    }

    public static void REMOVE_DIRECTED_EDGE_SUCCESSFUL<TNode, TEdge>(IMutableGraph<TNode, TEdge> graph, 
                                                                     TNode x, TNode y, TEdge edge)
    {
        if (!graph.ContainsEdge(x, y, edge) || !graph.ContainsEdge(y, x, edge))
            throw new InvalidOperationException("graph must contain specified edge and back edge.");
        if (!graph.Directed) throw new InvalidOperationException("graph must be undirected.");
        var edgeCount = graph.EdgesCount;
        Assert.IsTrue(graph.RemoveEdge(x, y, edge));
        Assert.IsFalse(graph.ContainsEdge(x, y, edge));
        Assert.IsTrue(graph.ContainsEdge(y, x, edge));
        Assert.AreEqual(edgeCount - 1, graph.EdgesCount);
    }

    public static void GET_OUTCOMING_EDGES<TNode, TEdge>(IGraph<TNode, TEdge> graph, 
                                                         IDictionary<(TNode x, TNode y), TEdge> testData)
    {
        var groups = testData.GroupBy(kv => kv.Key.x);
        foreach (var group in groups)
            Assert.IsTrue(Enumerable.SequenceEqual(group.Select(kv => kv.Value), graph.Edges(group.Key)));
    }
}