using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

public partial class _Default : System.Web.UI.Page
{
    static string xmlPath;
    static DataTable dtVals;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            #region Loading and parsing of XML file
            xmlPath = Server.MapPath("~/Data/XMLFile.xml");
            LoadData();
            #endregion

            #region Filling list of graph nodes
            CreateLinks();
            #endregion

            #region Loading of graph page
            frmGraph.Attributes.Add("src", "Graph.aspx");
            #endregion
        }
    }

    protected void LoadData()
    {
        dtVals = XmlParser.ReadXml(xmlPath);
        gvXmlData.DataSource = dtVals;
        gvXmlData.DataBind();
    }

    private void CreateLinks()
    {
        IEnumerable<DataRow> lstAllSg2 = dtVals.Select("TAG='GROUP'");
        IEnumerable<DataRow> lstSgIn2 = dtVals.Select("TAG='INCOND'");
        IEnumerable<DataRow> lstSgOut2 = dtVals.Select("TAG='OUTCOND'");

        IEnumerable<DataRow> lstOrphans = lstAllSg2;
        lstOrphans = lstOrphans.Where(p => !lstSgIn2.Any(p2 => p2["OWNER"] == p["OWNER"]));
        lstOrphans = lstOrphans.Where(p => !lstSgOut2.Any(p2 => p2["OWNER"] == p["OWNER"]));

        int nbColsO = 4;
        decimal nbLigs = Convert.ToDecimal(lstOrphans.Count()) / nbColsO;
        decimal nbLigsO = Math.Ceiling(nbLigs);

        ObjectsClass.lstGraphNodes = new List<ObjectsClass.GraphNode>();

        GraphHelper.CreateBmpGraph();

        // Set-up of root node
        ObjectsClass.GraphNode rootNode = GraphHelper.CreateGraphNode("root", "root", "root",
            Convert.ToInt32(nbLigsO) + 1, nbColsO / 2, ObjectsClass.RootNode, true);

        int numColOP = 1;
        int numColP = 0;
        int numRowO = 1;
        int numRowP = rootNode.row + 1;
        foreach (DataRow sg in lstAllSg2)
        {
            string idSg = Convert.ToString(sg["OWNER"]);
            string nameSg = Convert.ToString(sg["NAME"]);

            IEnumerable<DataRow> queryIn =
                from lstSgIn in lstSgIn2.AsEnumerable()
                where lstSgIn.Field<string>("OWNER") == idSg
                select lstSgIn;
            DataTable dansIn = new DataTable();
            if (queryIn.Count() > 0) dansIn = queryIn.CopyToDataTable<DataRow>();

            IEnumerable<DataRow> queryOut =
                from lstSgOut in lstSgOut2.AsEnumerable()
                where lstSgOut.Field<string>("OWNER") == idSg
                select lstSgOut;
            DataTable dansOut = new DataTable();
            if (queryOut.Count() > 0) dansOut = queryOut.CopyToDataTable<DataRow>();

            if (dansIn.Rows.Count == 0 && dansOut.Rows.Count == 0)
            {
                // Orphan node
                if (numColOP > nbColsO)
                {
                    numRowO++;
                    numColOP = 1;
                }

                ObjectsClass.GraphNode sgNodeO = GraphHelper.CreateGraphNode(idSg, nameSg, string.Empty,
                    numRowO, numColOP++, ObjectsClass.OrphanNode);
                rootNode.children.Add(sgNodeO);
                ObjectsClass.lstGraphNodes.Add(sgNodeO);
            }
            else if (dansIn.Rows.Count == 0 && dansOut.Rows.Count > 0)
            {
                // "Father" node
                numColP++;
                numRowP = rootNode.row + 1;

                ObjectsClass.GraphNode sgNodeP = GraphHelper.CreateGraphNode(idSg, nameSg, string.Empty,
                    numRowP, numColP, ObjectsClass.FatherNode, true);
                rootNode.children.Add(sgNodeP);

                foreach (DataRow sgO in dansOut.Rows)
                {
                    string outCond = Convert.ToString(sgO["NAME"]);
                    GetChildren(outCond, ref sgNodeP, ref lstSgIn2, ref lstSgOut2, ref numRowP, ref numColP, ref ObjectsClass.lstGraphNodes);
                }

                bool estDansLst = ObjectsClass.lstGraphNodes.Any(p => p.id == sgNodeP.id);
                if (!estDansLst) ObjectsClass.lstGraphNodes.Add(sgNodeP);
            }
        }

        GraphHelper.DestroyBmpGraph();

        ObjectsClass.lstGraphNodes.Add(rootNode);
    }

    private static void GetChildren(string outCond, ref ObjectsClass.GraphNode sgNode, ref IEnumerable<DataRow> lstSgIn2, ref IEnumerable<DataRow> lstSgOut2,
            ref int numRow, ref int numColP, ref List<ObjectsClass.GraphNode> lstGraphNodes)
    {
        string idSgIn = sgNode.id;
        IEnumerable<DataRow> queryIn =
                        from lstSgIn in lstSgIn2.AsEnumerable()
                        where lstSgIn.Field<string>("NAME") == outCond
                        && lstSgIn.Field<string>("OWNER") != idSgIn
                        select lstSgIn;
        DataTable dtSgFils = new DataTable();
        if (queryIn.Count() > 0)
        {
            sgNode.nodecolor = ObjectsClass.FatherNode;
            dtSgFils = queryIn.CopyToDataTable<DataRow>();
        }

        foreach (DataRow sgF in dtSgFils.Rows)
        {
            string idSg = Convert.ToString(sgF["OWNER"]);

            List<ObjectsClass.GraphNode> lstSgTarget = lstGraphNodes.Where(p => p.id == idSg).ToList();
            if (lstSgTarget.Count > 0)
            {
                #region To add link between existing nodes
                ObjectsClass.GraphNode sgNodeF = GraphHelper.CreateGraphNode(lstSgTarget[0].id, lstSgTarget[0].name,
                    lstSgTarget[0].title, lstSgTarget[0].row, lstSgTarget[0].col, lstSgTarget[0].nodecolor);
                sgNode.children.Add(sgNodeF);
                #endregion
            }
            else
            {
                numRow++;

                string nameSg = Convert.ToString(sgF["OWNER"]);
                string ssTitreSg = string.Empty;

                ObjectsClass.GraphNode sgNodeF = GraphHelper.CreateGraphNode(idSg, nameSg, ssTitreSg,
                    sgNode.row + 1, sgNode.col + sgNode.children.Count, ObjectsClass.ChildNode);

                bool estDansLst = lstGraphNodes.Any(p => p.id == sgNodeF.id);
                if (!estDansLst) lstGraphNodes.Add(sgNodeF);

                IEnumerable<DataRow> queryOut =
                    from lstSgOut in lstSgOut2.AsEnumerable()
                    where lstSgOut.Field<string>("OWNER") == idSg
                    select lstSgOut;
                DataTable dtSgOut = new DataTable();
                if (queryOut.Count() > 0) dtSgOut = queryOut.CopyToDataTable<DataRow>();

                if (dtSgOut.Rows.Count > 0)
                {
                    sgNodeF.children = new List<ObjectsClass.GraphNode>();

                    foreach (DataRow sgO in dtSgOut.Rows)
                    {
                        GetChildren(Convert.ToString(sgO["NAME"]), ref sgNodeF, ref lstSgIn2, ref lstSgOut2, ref numRow, ref numColP, ref lstGraphNodes);

                        numRow = sgNodeF.row + 1;
                        numColP = sgNodeF.col + (sgNodeF.children.Count == 0 ? 0 : 1);
                    }
                }
                else
                    numColP = sgNodeF.col;

                sgNode.children.Add(sgNodeF);
            }
        }
    }
}