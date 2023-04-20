using OpenDataStructures.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenDataStructures.Tests;

[TestClass]
public class RebBlackTreeTests
{
	[TestMethod]
	public void ROTATION()
	{
		var tree = new RedBlackTree<int> { 6 };
		Assert.IsNotNull(tree._root);
		var root = tree._root;

		var l = new RedBlackTree<int>.RedBlackNode(root, 4, null, null, false);
		root.Left = l;
		var ll = new RedBlackTree<int>.RedBlackNode(l, 3, null, null, false);
		root.Left.Left = ll;
		var lr = new RedBlackTree<int>.RedBlackNode(l, 5, null, null, false);
		l.Right = lr;
		var r1 = new RedBlackTree<int>.RedBlackNode(root, 8, null, null, false);
		root.Right = r1;
		var rl = new RedBlackTree<int>.RedBlackNode(r1, 7, null, null, false);
		root.Right.Left = rl;
		var rr = new RedBlackTree<int>.RedBlackNode(r1, 9, null, null, false);
		root.Right.Right = rr;

		tree.RotateLeft(root);

		Assert.AreEqual(r1, tree._root);
		Assert.AreEqual(r1.Left, root);
		Assert.AreEqual(r1.Right, rr);
		Assert.AreEqual(root.Left, l);
		Assert.AreEqual(root.Right, rl);
		Assert.AreEqual(l.Left, ll);
		Assert.AreEqual(l.Right, lr);

		Assert.AreEqual(ll.Left, null);
		Assert.AreEqual(ll.Right, null);

		Assert.AreEqual(rl.Left, null);
		Assert.AreEqual(rl.Right, null);

		Assert.AreEqual(rr.Left, null);
		Assert.AreEqual(rr.Right, null);

		Assert.AreEqual(ll.Parent, l);
		Assert.AreEqual(lr.Parent, l);

		Assert.AreEqual(l.Parent, root);
		Assert.AreEqual(rl.Parent, root);

		Assert.AreEqual(root.Parent, r1);

		Assert.AreEqual(r1.Parent, null);

		Assert.AreEqual(rr.Parent, r1);
	}

	[TestMethod]
	public void RED_BLACK_INSERTION()
	{
		var tree    = new RedBlackTree<int> { 33 };
		Assert.IsNotNull(tree._root);
		var oldRoot = tree._root;

		var l = new RedBlackTree<int>.RedBlackNode(oldRoot, 13, null, null, false);
		oldRoot.Left = l;
		var ll = new RedBlackTree<int>.RedBlackNode(oldRoot.Left, 11, null, null, true);
		oldRoot.Left.Left = ll;
		var lr = new RedBlackTree<int>.RedBlackNode(oldRoot.Left, 21, null, null, true);
		oldRoot.Left.Right = lr;
		var lrl = new RedBlackTree<int>.RedBlackNode(oldRoot.Left.Right, 15, null, null, false);
		oldRoot.Left.Right.Left = lrl;
		var lrr = new RedBlackTree<int>.RedBlackNode(oldRoot.Left.Right, 31, null, null, false);
		oldRoot.Left.Right.Right = lrr;
		var r = new RedBlackTree<int>.RedBlackNode(oldRoot, 53, null, null, true);
		oldRoot.Right = r;
		var rl = new RedBlackTree<int>.RedBlackNode(oldRoot.Right, 41, null, null, false);
		oldRoot.Right.Left = rl;
		var rr = new RedBlackTree<int>.RedBlackNode(oldRoot.Right, 61, null, null, false);
		oldRoot.Right.Right = rr;

		tree.Add(20);

		Assert.IsNotNull(lr.Left);
		Assert.IsNotNull(lr.Left.Right);
		Assert.IsNotNull(lr.Left.Right.Right);
		Assert.IsNotNull(lr.Right);
		Assert.IsNotNull(lr.Right.Right);

		Assert.AreEqual(lr, tree._root);
		Assert.AreEqual(lr.Left, l);
		Assert.AreEqual(lr.Left.Left, ll);
		Assert.AreEqual(lr.Left.Right, lrl);
		Assert.AreEqual(lr.Left.Right.Right.Value, 20);

		Assert.AreEqual(lr.Right, oldRoot);
		Assert.AreEqual(lr.Right.Left, lrr);
		Assert.AreEqual(lr.Right.Right, r);
		Assert.AreEqual(lr.Right.Right.Left, rl);
		Assert.AreEqual(lr.Right.Right.Right, rr);

		Assert.IsNotNull(lrl.Right);

		Assert.AreEqual(lr.Parent, null);
		Assert.AreEqual(l.Parent, lr);
		Assert.AreEqual(ll.Parent, l);
		Assert.AreEqual(lrl.Parent, l);
		Assert.AreEqual(lrl.Right.Parent, lrl);
		Assert.AreEqual(oldRoot.Parent, lr);
		Assert.AreEqual(lrr.Parent, oldRoot);

		Assert.AreEqual(r.Parent, oldRoot);
		Assert.AreEqual(rl.Parent, r);
		Assert.AreEqual(rr.Parent, r);

		Assert.IsTrue(lr.IsBlack);
		Assert.IsTrue(ll.IsBlack);
		Assert.IsTrue(lrl.IsBlack);
		Assert.IsTrue(lrr.IsBlack);
		Assert.IsTrue(r.IsBlack);

		Assert.IsFalse(l.IsBlack);
		Assert.IsFalse(oldRoot.IsBlack);
		Assert.IsFalse(lrl.Right.IsBlack);
		Assert.IsFalse(rl.IsBlack);
		Assert.IsFalse(rr.IsBlack);
	}
}
