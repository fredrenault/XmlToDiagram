using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// Summary description for ObjectsClass
/// </summary>
public class ObjectsClass
{
	public ObjectsClass()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    #region Classes for graph
    public static string ChildNode = "purple";
    public static string OrphanNode = "blue";
    public static string FatherNode = "red";
    public static string RootNode = "green";

    public static Font ftNode = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
    public static Font ftTitle = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Italic);

    public static List<GraphNode> lstGraphNodes;

    public class GraphNode
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public int? idparent { get; set; }
        public string nodecolor { get; set; }

        public float hautTitre { get; set; }
        public float largTitre { get; set; }
        public float hautSsTitre { get; set; }
        public float largSsTitre { get; set; }

        public float hgX { get; set; }
        public float hgY { get; set; }
        public float bgX { get; set; }
        public float bgY { get; set; }
        public float hdX { get; set; }
        public float hdY { get; set; }
        public float bdX { get; set; }
        public float bdY { get; set; }

        public List<GraphNode> children { get; set; }
    }

    public class CoordRect
    {
        public float xCoord { get; set; }
        public float yCoord { get; set; }
        public float hCoord { get; set; }
        public float wCoord { get; set; }
    }

    public class GraphRects
    {
        public int ligRect { get; set; }
        public int colRect { get; set; }
        public CoordRect coordRect { get; set; }
    }
    #endregion
}