using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenDataStructures.Graph;
using OpenDataStructures.Graph.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDataStructures.Tests.Helpers;

namespace OpenDataStructures.Tests;

[TestClass]
public class GraphTests
{
    private IEnumerable<IList<int>> _testData = new List<IList<int>>();
    private IMutableGraph<int, int> _graph = new Graph<int, int>(false);

    [TestInitialize]
    public void Initialize()
    {
        var asemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var testDataPath = Path.Combine(asemblyPath, @"Data/GraphTests/graph.txt");
        
        _testData = File.ReadAllLines(testDataPath).Select(raw => raw.Split(", ").Select(int.Parse).ToList()).ToList();

        _graph = new Graph<int, int>(false);
        foreach (var raw in _testData)
            _graph.AddEdge(raw[0], raw[1], raw[2]);
    }

    [TestMethod]
    public void CONTAINS_NODE_SUCCESSFUL() =>
        TestHelperIGraph.CONTAINS_NODE_SUCCESSFUL(new Graph<int, int>(false), 0);

    [TestMethod]
    public void CONTAINS_NODE_UNSUCCESSFUL() =>
        TestHelperIGraph.CONTAINS_NODE_SUCCESSFUL(new Graph<int, int>(false), 0);

    [TestMethod]
    public void ADD_SINGLE_NODE_SUCCESSFUL()
    {
        var graph = new Graph<int, int>(false);
        TestHelperIGraph.ADD_SINGLE_NODE_SUCCESSFUL(graph, 5);
    }

    [TestMethod]
    public void REMOVE_SINGLE_NODE_SUCCESSFUL()
    {
        var graph = new Graph<int, int>(false);
        graph.AddNode(5);
        TestHelperIGraph.REMOVE_SINGLE_NODE_SUCCESSFUL(graph, 5);
    }

    [TestMethod]
    public void REMOVE_NODE_SUCCESSFUL() =>
        TestHelperIGraph.REMOVE_NODE_SUCCESSFUL(_graph, _graph.Nodes.MaxBy(n => _graph.Degree(n)));

    [TestMethod]
    public void REMOVE_UNDIRECTED_EDGE_SUCCESSFUL()
    {
        var x = _graph.Nodes.First();
        var y = _graph.AdjacentNodes(x).First();
        var edge = _graph.Edges(x, y).First();
        TestHelperIGraph.REMOVE_UNDIRECTED_EDGE_SUCCESSFUL(_graph, x, y, edge);
    }

    [TestMethod]
    public void REMOVE_DIRECTED_EDGE_SUCCESSFUL()
    {
        _graph = new Graph<int, int>(true);
        foreach (var raw in _testData)
            _graph.AddEdge(raw[0], raw[1], raw[0]);
        var x = _graph.Nodes.First();
        var y = _graph.AdjacentNodes(x).First();
        var edge = _graph.Edges(x, y).First();
        _graph.AddEdge(y, x, edge);
        TestHelperIGraph.REMOVE_DIRECTED_EDGE_SUCCESSFUL(_graph, x, y, edge);
    }

    [TestMethod]
    public void GET_OUTCOMING_EDGES()
    {
        var graph = new Graph<int, int>(true);
        foreach (var raw in _testData)
            graph.AddEdge(raw[0], raw[1], raw[2]);
        var edgesDict = _testData.ToDictionary(raw => (raw[0], raw[1]), raw => raw[2]);
        TestHelperIGraph.GET_OUTCOMING_EDGES(graph, edgesDict);
    }
}