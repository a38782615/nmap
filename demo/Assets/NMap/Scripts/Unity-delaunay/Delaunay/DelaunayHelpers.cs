using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{	

	public class Node
	{
		public static Stack<Node> pool = new Stack<Node> ();
		
		public Node parent;
		public int treeSize;
	}

	public enum KruskalType
	{
		MINIMUM,
		MAXIMUM
	}

	public static class DelaunayHelpers
	{
		public static List<GeoLineSegment> VisibleLineSegments (List<Edge> edges)
		{
			List<GeoLineSegment> segments = new List<GeoLineSegment> ();
			
			for (int i = 0; i<edges.Count; i++) {
				Edge edge = edges [i];
				if (edge.visible) {
					Nullable<float2> p1 = edge.clippedEnds [LRSide.LEFT];
					Nullable<float2> p2 = edge.clippedEnds [LRSide.RIGHT];
					segments.Add (new GeoLineSegment (p1, p2));
				}
			}
			
			return segments;
		}

		public static List<Edge> SelectEdgesForSitePoint (float2 coord, List<Edge> edgesToTest)
		{
			return edgesToTest.FindAll (delegate (Edge edge) {
				return ((edge.leftSite != null && edge.leftSite.Coord.Equals(coord))
					|| (edge.rightSite != null && edge.rightSite.Coord.Equals(coord)));
			});
		}

		public static List<Edge> SelectNonIntersectingEdges (/*keepOutMask:BitmapData,*/List<Edge> edgesToTest)
		{
//			if (keepOutMask == null)
//			{
			return edgesToTest;
//			}
			
//			var zeroPoint:Point = new Point();
//			return edgesToTest.filter(myTest);
//			
//			function myTest(edge:Edge, index:int, vector:Vector.<Edge>):Boolean
//			{
//				var delaunayLineBmp:BitmapData = edge.makeDelaunayLineBmp();
//				var notIntersecting:Boolean = !(keepOutMask.hitTest(zeroPoint, 1, delaunayLineBmp, zeroPoint, 1));
//				delaunayLineBmp.dispose();
//				return notIntersecting;
//			}
		}

		public static List<GeoLineSegment> DelaunayLinesForEdges (List<Edge> edges)
		{
			List<GeoLineSegment> segments = new List<GeoLineSegment> ();
			Edge edge;
			for (int i = 0; i < edges.Count; i++) {
				edge = edges [i];
				segments.Add (edge.DelaunayLine ());
			}
			return segments;
		}
		
		/**
		*  Kruskal's spanning tree algorithm with union-find
		 * Skiena: The Algorithm Design Manual, p. 196ff
		 * Note: the sites are implied: they consist of the end points of the line segments
		*/
		public static List<GeoLineSegment> Kruskal (List<GeoLineSegment> lineSegments, KruskalType type = KruskalType.MINIMUM)
		{
			Dictionary<Nullable<float2>,Node> nodes = new Dictionary<Nullable<float2>,Node> ();
			List<GeoLineSegment> mst = new List<GeoLineSegment> ();
			Stack<Node> nodePool = Node.pool;
			
			switch (type) {
			// note that the compare functions are the reverse of what you'd expect
			// because (see below) we traverse the lineSegments in reverse order for speed
			case KruskalType.MAXIMUM:
				lineSegments.Sort (delegate (GeoLineSegment l1, GeoLineSegment l2) {
					return GeoLineSegment.CompareLengths (l1, l2);
				});
				break;
			default:
				lineSegments.Sort (delegate (GeoLineSegment l1, GeoLineSegment l2) {
					return GeoLineSegment.CompareLengths_MAX (l1, l2);
				});
				break;
			}
			
			for (int i = lineSegments.Count; --i > -1;) {
				GeoLineSegment geoLineSegment = lineSegments [i];

				Node node0 = null;
				Node rootOfSet0;
				if (!nodes.ContainsKey (geoLineSegment.p0)) {
					node0 = nodePool.Count > 0 ? nodePool.Pop () : new Node ();
					// intialize the node:
					rootOfSet0 = node0.parent = node0;
					node0.treeSize = 1;
					
					nodes [geoLineSegment.p0] = node0;
				} else {
					node0 = nodes [geoLineSegment.p0];
					rootOfSet0 = Find (node0);
				}
				
				Node node1 = null;
				Node rootOfSet1;
				if (!nodes.ContainsKey (geoLineSegment.p1)) {
					node1 = nodePool.Count > 0 ? nodePool.Pop () : new Node ();
					// intialize the node:
					rootOfSet1 = node1.parent = node1;
					node1.treeSize = 1;
					
					nodes [geoLineSegment.p1] = node1;
				} else {
					node1 = nodes [geoLineSegment.p1];
					rootOfSet1 = Find (node1);
				}
				
				if (rootOfSet0 != rootOfSet1) {	// nodes not in same set
					mst.Add (geoLineSegment);
					
					// merge the two sets:
					int treeSize0 = rootOfSet0.treeSize;
					int treeSize1 = rootOfSet1.treeSize;
					if (treeSize0 >= treeSize1) {
						// set0 absorbs set1:
						rootOfSet1.parent = rootOfSet0;
						rootOfSet0.treeSize += treeSize1;
					} else {
						// set1 absorbs set0:
						rootOfSet0.parent = rootOfSet1;
						rootOfSet1.treeSize += treeSize0;
					}
				}
			}
			foreach (Node node in nodes.Values) {
				nodePool.Push (node);
			}
			
			return mst;
		}

		private static Node Find (Node node)
		{
			if (node.parent == node) {
				return node;
			} else {
				Node root = Find (node.parent);
				// this line is just to speed up subsequent finds by keeping the tree depth low:
				node.parent = root;
				return root;
			}
		}
	}


}