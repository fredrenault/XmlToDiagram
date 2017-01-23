using System;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// This class is used to create a graph node.
/// To use her from a web page :
///     1) Call void "CreateBmpGraph"
///     2) Call function "CreateGraphNode"
///     3) Call void "DestroyBmpGraph"
/// 
/// Function "CreateGraphNode" calculates size text 
/// based on variables "ftNode" and "ftTitle" declared
/// in the class "ObjectsClass".
/// </summary>
public class GraphHelper
{
	public GraphHelper()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    #region Variables used for the creation of the nodes
    static Bitmap b;
    static Graphics g2;
    #endregion

    #region Initialization and release of those variables
    public static void CreateBmpGraph()
    {
        b = new Bitmap(1000, 1000);
        g2 = Graphics.FromImage(b);
    }

    public static void DestroyBmpGraph()
    {
        g2.Dispose();
        b.Dispose();
    }
    #endregion

    #region Set-up of a new node based on his values
    public static ObjectsClass.GraphNode CreateGraphNode(string idNode, string titleNode, string subTitleNode,
        int rowNode, int colNode, string colorNode, bool initChild = false, int? idPere = null)
    {
        #region Calculation of title and subtitle default sizes
        SizeF sizeTitre = g2.MeasureString(titleNode, ObjectsClass.ftNode);
        SizeF sizeSsTitre = g2.MeasureString(subTitleNode, ObjectsClass.ftTitle);
        #endregion

        #region Default size of the new node
        float maxLarg = sizeTitre.Width;
        float hSsTitre = sizeSsTitre.Height;
        #endregion

        #region Update node width/height with longer subtitle line (break on carriage return)
        string[] tabSsTitre = subTitleNode.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        if (tabSsTitre.Length > 1)
        {
            foreach (string ssTitre in tabSsTitre)
            {
                SizeF sizeSsTitre2 = g2.MeasureString(ssTitre, ObjectsClass.ftTitle);
                if (sizeSsTitre2.Width > maxLarg) maxLarg = sizeSsTitre2.Width;
            }
        }
        else
        {
            int nbLigSsTitre = Convert.ToInt32(Math.Ceiling(sizeSsTitre.Width / sizeTitre.Width));
            hSsTitre = (nbLigSsTitre - 1) * hSsTitre;
        }
        #endregion

        #region Node creation
        ObjectsClass.GraphNode sgNode = new ObjectsClass.GraphNode
        {
            id = idNode,
            name = titleNode,
            title = subTitleNode,
            row = rowNode,
            col = colNode,
            nodecolor = colorNode,
            hautTitre = sizeTitre.Height,
            largTitre = maxLarg,
            hautSsTitre = hSsTitre,
            largSsTitre = sizeSsTitre.Width,
            idparent = idPere
        };
        #endregion

        #region If we want, we can initialize the list of children nodes
        if (initChild) sgNode.children = new List<ObjectsClass.GraphNode>();
        #endregion

        return sgNode;
    }
    #endregion
}