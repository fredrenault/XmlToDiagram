using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public partial class Graph : Page
{
    /// <summary>
    /// WARNING : to get the diagram on the aspx page, 
    /// you must add [ ContentType = "image/gif" ]
    /// to the <%@ Page ... %> tag
    /// </summary>
    /// 
    #region Values for graph drawing
    const int ligInter = 50;        // Height of the "empty row" between nodes
    const int colInter = 50;        // Width of the "empty column" between nodes
    const int decalCadre = 15;      // Value to have blank spaces around the node
    float zoom = 1f;                // Default value for the zoom (equals to 100%)
    #endregion

    #region Lists of values used for drawing
    private List<ObjectsClass.GraphNode> _wkfGraph;
    private List<string> lstErr;
    private List<ObjectsClass.GraphRects> lstRects;

    private Graphics g3;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Clear();

        Bitmap bmp = new Bitmap(800, 600);
        g3 = Graphics.FromImage(bmp);

        DrawGraph();

        bmp.Save(Response.OutputStream, ImageFormat.Gif);
        g3.Dispose();
        bmp.Dispose();
        Response.End();
    }

    private void DrawGraph()
    {
        try
        {
            g3.ScaleTransform(zoom, zoom);
            g3.SmoothingMode = SmoothingMode.AntiAlias;

            _wkfGraph = ObjectsClass.lstGraphNodes;

            #region Get number of rows and columns for the graph
            int maxCol = (_wkfGraph.OrderByDescending(x => x.col).FirstOrDefault()).col;
            int maxRow = (_wkfGraph.OrderByDescending(x => x.row).FirstOrDefault()).row;
            #endregion

            #region Get max width / height for the graph
            float maxLargTitre = (_wkfGraph.OrderByDescending(x => x.largTitre).FirstOrDefault()).largTitre;
            float maxHautTitre = (_wkfGraph.OrderByDescending(x => x.hautTitre).FirstOrDefault()).hautTitre;

            float maxLargSsTitre = (_wkfGraph.OrderByDescending(x => x.largSsTitre).FirstOrDefault()).largSsTitre;
            float maxHautSsTitre = (_wkfGraph.OrderByDescending(x => x.hautSsTitre).FirstOrDefault()).hautSsTitre;
            float maxHautTitres = maxHautTitre + maxHautSsTitre;
            #endregion

            #region Get size of 1 node
            float hautRect = maxHautTitres + (2 * decalCadre);
            float largRect = maxLargTitre + (2 * decalCadre);
            #endregion

            #region Size max of the graph
            float maxLarg = (maxCol * largRect) + ((maxCol) * colInter);
            float maxHaut = (maxRow * hautRect) + ((maxRow) * ligInter);
            #endregion

            #region Draw of a red rectangle which will contains rectangles (not used in this example)
            foreach (ObjectsClass.GraphNode node in _wkfGraph)
            {
                if (node.col == 0 && node.row == 0)
                    DrawBlock(node.name, Convert.ToInt32(node.id), hautRect, largRect);
            }
            #endregion

            #region Draw green rectangles and fill list containing their location
            lstRects = new List<ObjectsClass.GraphRects>();
            foreach (ObjectsClass.GraphNode node in _wkfGraph)
            {
                if (node.col != 0 && node.row != 0)
                {
                    ObjectsClass.CoordRect coordsRect = DrawRectangle(node, hautRect, largRect);
                    ObjectsClass.GraphRects graphRect = new ObjectsClass.GraphRects
                    {
                        ligRect = node.row,
                        colRect = node.col,
                        coordRect = coordsRect
                    };
                    lstRects.Add(graphRect);
                }
            }
            #endregion

            #region Draw titles and subtitles based on green rectangles
            lstErr = new List<string>();
            for (int n = 0; n < _wkfGraph.Count; n++)
            {
                ObjectsClass.GraphNode node = _wkfGraph[n];
                if (node.col != 0 && node.row != 0)
                {
                    DrawText(ref node, maxLargTitre, maxHautTitre, maxHautSsTitre);
                }
            }
            #endregion
        }
        catch(Exception ex)
        {
            g3.DrawString(ex.Message, ObjectsClass.ftTitle, new SolidBrush(Color.FromName("black")), 0, 0);
        }
    }

    #region Void used to calculate X / Y coordinates of rectangle
    private float CalculXY(int ligCol, float hwRect)
    {
        float result = ((((ligCol - 1) * hwRect) + (ligCol * colInter)) - decalCadre);
        return result;
    }
    #endregion

    private ObjectsClass.CoordRect DrawRectangle(ObjectsClass.GraphNode node, float hRect, float wRect)
    {
        #region X / Y calculation of green rectangle
        float xRect = CalculXY(node.col, wRect);
        float yRect = CalculXY(node.row, hRect);
        #endregion

        #region Draw of green rectangle
        g3.FillRectangle(new SolidBrush(Color.LightGreen), xRect, yRect, wRect, hRect);
        #endregion

        #region Send back location and size of green rectangle
        ObjectsClass.CoordRect coordRect = new ObjectsClass.CoordRect
        {
            xCoord = xRect,
            yCoord = yRect,
            hCoord = hRect,
            wCoord = wRect
        };
        return coordRect;
        #endregion
    }

    private void DrawText(ref ObjectsClass.GraphNode node, float maxLargTitre, float maxHautTitre,
            float maxHautSsTitre)
    {
        #region Get rectangle associated with the current node
        ObjectsClass.GraphNode node2 = node;
        List<ObjectsClass.GraphRects> rect = lstRects.Where(x => x.colRect == node2.col && x.ligRect == node2.row).ToList();
        #endregion

        #region Calculation of title's coordinates based on the rectangle
        float xTitre = rect[0].coordRect.xCoord + decalCadre;
        float yTitre = rect[0].coordRect.yCoord + decalCadre;
        #endregion

        #region Calculation of subtitle's coordinates
        float xSsTitre = xTitre;
        float ySsTitre = yTitre + maxHautTitre;
        #endregion

        #region Title's font color
        SolidBrush colorName = new SolidBrush(Color.FromName("black"));
        if (!string.IsNullOrEmpty(node.nodecolor)) colorName = new SolidBrush(Color.FromName(node.nodecolor));
        #endregion

        #region Draw of rectangles for string containing carriage return
        RectangleF rectTitre = new RectangleF(xTitre, yTitre, maxLargTitre, maxHautTitre);
        RectangleF rectSsTitre = new RectangleF(xSsTitre, ySsTitre, maxLargTitre, maxHautSsTitre + 100);
        #endregion

        #region Set-up of text's alignment
        StringFormat drawFormat = new StringFormat();
        drawFormat.Alignment = StringAlignment.Center;
        #endregion

        #region Title and subtitle writing
        g3.DrawString(node.name, ObjectsClass.ftNode, colorName, rectTitre, drawFormat);
        g3.DrawString(node.title, ObjectsClass.ftTitle, new SolidBrush(Color.FromName("black")), rectSsTitre, drawFormat);
        #endregion

        #region Update of X/Y coordinates for all the corners of the rectangle
        node.hgX = rect[0].coordRect.xCoord;
        node.hgY = rect[0].coordRect.yCoord;

        node.hdX = rect[0].coordRect.xCoord + rect[0].coordRect.wCoord;
        node.hdY = rect[0].coordRect.yCoord;

        node.bgX = rect[0].coordRect.xCoord;
        node.bgY = rect[0].coordRect.yCoord + rect[0].coordRect.hCoord;

        node.bdX = rect[0].coordRect.xCoord + rect[0].coordRect.wCoord;
        node.bdY = rect[0].coordRect.yCoord + rect[0].coordRect.hCoord;
        #endregion

        #region Draw of lines to child node
        if (node.children != null)
        {
            foreach (ObjectsClass.GraphNode child in node.children)
            {
                #region Get coordinates of child node's rectangle
                List<ObjectsClass.GraphRects> rectFils = lstRects.Where(x => x.colRect == child.col && x.ligRect == child.row).ToList();
                if (rectFils.Count > 0)
                {
                    ObjectsClass.CoordRect coordRectArr = rectFils[0].coordRect;
                    DrawLine(node, child, rect[0].coordRect, coordRectArr);
                }
                #endregion
            }
        }
        #endregion
    }

    private void DrawLine(ObjectsClass.GraphNode nodeDep, ObjectsClass.GraphNode nodeArr,
            ObjectsClass.CoordRect coordRectDep, ObjectsClass.CoordRect coordRectArr)
    {
        Pen pen1 = new Pen(Color.Gray, 1F);
        using (GraphicsPath capPath = new GraphicsPath())
        {
            #region Creation of the arrow
            capPath.AddLine(-10, 0, 10, 0);
            capPath.AddLine(-10, 0, 0, 10);
            capPath.AddLine(0, 10, 10, 0);

            pen1.CustomEndCap = new System.Drawing.Drawing2D.CustomLineCap(null, capPath);
            #endregion

            #region Initialization of arrow's coordiantes
            float xTxtDep = 0;
            float xTxtArr = 0;
            float yTxtDep = 0;
            float yTxtArr = 0;
            #endregion

            #region Default coordinates of arrow
            float xDepBase = coordRectDep.xCoord;
            float yDepBase = coordRectDep.yCoord;
            float xArrBase = coordRectArr.xCoord;
            float yArrBase = coordRectArr.yCoord;
            #endregion

            #region Final calculation of arrow's coordinates
            /// To have :
            ///     -1  if Row/Col start lesser than Row/Col end
            ///     0   if Row/Col start equals to Row/Col end
            ///     1   if Row/Col start greater than Row/Col enf
            int signLig = Math.Sign(nodeDep.row.CompareTo(nodeArr.row));
            int signCol = Math.Sign(nodeDep.col.CompareTo(nodeArr.col));

            float demiLarg = coordRectDep.wCoord / 2;
            float decalXDep = 0;
            if (nodeDep.col != 1) decalXDep = demiLarg / (nodeDep.col - 1);
            switch (signLig)
            {
                #region Row start lesser than Row end
                case -1:
                    switch (signCol)
                    {
                        #region Col start lesser than Col end
                        case -1:
                            xTxtDep = xDepBase + demiLarg + ((nodeArr.col - nodeDep.col - 1) * decalXDep);
                            yTxtDep = yDepBase + coordRectDep.hCoord;

                            xTxtArr = xArrBase;
                            yTxtArr = yArrBase;
                            break;
                        #endregion

                        #region Col start equals to Col end
                        case 0:
                            xTxtDep = xDepBase + demiLarg;
                            yTxtDep = yDepBase + coordRectDep.hCoord;

                            xTxtArr = xArrBase + demiLarg;
                            yTxtArr = yArrBase;
                            break;
                        #endregion

                        #region Col start greater than Col end
                        case 1:
                            xTxtDep = xDepBase + ((nodeArr.col - 1) * decalXDep);
                            yTxtDep = yDepBase + coordRectDep.hCoord;

                            xTxtArr = xArrBase + coordRectArr.wCoord;
                            yTxtArr = yArrBase;
                            break;
                        #endregion
                    }
                    break;
                #endregion

                #region Row start equals to Row end
                case 0:
                    switch (signCol)
                    {
                        #region Col start lesser than Col end
                        case -1:
                            xTxtDep = xDepBase + coordRectDep.wCoord;
                            yTxtDep = yDepBase + (coordRectDep.hCoord / 2);

                            xTxtArr = xArrBase;
                            yTxtArr = yArrBase + (coordRectArr.hCoord / 2);
                            break;
                        #endregion

                        #region Col start equals to Col end
                        case 0:
                            break;
                        #endregion

                        #region Col start greater then Col end
                        case 1:
                            xTxtDep = xDepBase;
                            yTxtDep = yDepBase + (coordRectDep.hCoord / 2);

                            xTxtArr = xArrBase + coordRectArr.wCoord;
                            yTxtArr = yArrBase + (coordRectArr.hCoord / 2);
                            break;
                        #endregion
                    }
                    break;
                #endregion

                #region Row start greater than Row end
                case 1:
                    switch (signCol)
                    {
                        #region Col start lesser than Col end
                        case -1:
                            xTxtDep = xDepBase + demiLarg + ((nodeArr.col - nodeDep.col - 1) * decalXDep);
                            yTxtDep = yDepBase;

                            xTxtArr = xArrBase;
                            yTxtArr = yArrBase + coordRectArr.hCoord;
                            break;
                        #endregion

                        #region Col start equals to Col end
                        case 0:
                            xTxtDep = xDepBase + demiLarg;
                            yTxtDep = yDepBase;

                            xTxtArr = xArrBase + demiLarg;
                            yTxtArr = yArrBase + coordRectArr.hCoord;
                            break;
                        #endregion

                        #region Col start greater than Col end
                        case 1:
                            xTxtDep = xDepBase + ((nodeArr.col - 1) * decalXDep);
                            yTxtDep = yDepBase;

                            xTxtArr = xArrBase + coordRectArr.wCoord;
                            yTxtArr = yArrBase + coordRectArr.hCoord;
                            break;
                        #endregion
                    }
                    break;
                #endregion
            }
            #endregion

            #region Drawing of arrow
            try
            {
                g3.DrawLine(pen1, xTxtDep, yTxtDep, xTxtArr, yTxtArr);
            }
            catch (Exception ex)
            {
                string errMsg = "Start R" + nodeDep.row + "C" + nodeDep.col + " ## End R" + nodeArr.row + "C" + nodeArr.col;
                lstErr.Add(errMsg);
            }
            #endregion
        }
    }

    private void DrawBlock(string nodetext, int idBloc, float hautRect, float largRect)
    {
        List<ObjectsClass.GraphNode> lstBlocItems = _wkfGraph.Where(x => x.idparent == idBloc).ToList();

        int minCol = (lstBlocItems.OrderBy(x => x.col).FirstOrDefault()).col;
        int minRow = (lstBlocItems.OrderBy(x => x.row).FirstOrDefault()).row;
        float xRectMin = CalculXY(minCol, largRect);
        float yRectMin = CalculXY(minRow, hautRect);

        int maxCol = (lstBlocItems.OrderByDescending(x => x.col).FirstOrDefault()).col;
        int maxRow = (lstBlocItems.OrderByDescending(x => x.row).FirstOrDefault()).row;

        int nbLigBloc = (maxRow - minRow) + 1;
        int nbColBloc = (maxCol - minCol) + 1;

        float wBloc = (nbColBloc * largRect) + ((nbColBloc - 1) * colInter) + (2 * decalCadre);
        float hBloc = (nbLigBloc * hautRect) + ((nbLigBloc - 1) * ligInter) + (2 * decalCadre);

        float xBloc = xRectMin - decalCadre;
        float yBloc = yRectMin - decalCadre;

        g3.FillRectangle(new SolidBrush(Color.Red), xBloc, yBloc, wBloc, hBloc);

        g3.DrawString(nodetext, ObjectsClass.ftNode, new SolidBrush(Color.FromName("black")), xBloc, yBloc);
    }
}