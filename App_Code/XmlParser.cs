using System.Data;
using System.Xml;
using System.IO;

/// <summary>
/// Summary description for XmlParser
/// </summary>
public class XmlParser
{
	public XmlParser()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable ReadXml(string fileNamePath)
    {
        DataTable _dtVals = new DataTable();
        _dtVals.Columns.Add("ID");
        _dtVals.Columns.Add("TAG");
        _dtVals.Columns.Add("NAME");
        _dtVals.Columns.Add("OWNER");

        string owner = string.Empty;

        XmlDocument doc = new XmlDocument();
        using (StreamReader sr = new StreamReader(fileNamePath))
        {
            doc.Load(sr);
        }

        XmlNode xmlRoot = doc.DocumentElement;
        XmlNodeList SgHeadNode = xmlRoot.ChildNodes;
        foreach (XmlNode sgNode in SgHeadNode)
        {
            ReadAttributes(sgNode, ref _dtVals, ref owner);
        }

        return _dtVals;
    }

    private static void ReadAttributes(XmlNode node, ref DataTable dtVals, ref string owner)
    {
        dtVals.Rows.Add();

        string nodeName = node.Name;

        if (nodeName.Equals("GROUP")) owner = nodeName + node.Attributes["ID"].Value;

        dtVals.Rows[dtVals.Rows.Count - 1]["TAG"] = nodeName;
        dtVals.Rows[dtVals.Rows.Count - 1]["OWNER"] = owner;
        foreach (XmlAttribute attrib in node.Attributes)
        {
            string attribName = attrib.Name;
            string attribValue = attrib.Value;
            dtVals.Rows[dtVals.Rows.Count - 1][attribName] = attribValue;
        }

        XmlNodeList sgNodes = node.ChildNodes;
        foreach (XmlNode sgNode in sgNodes)
        {
            ReadAttributes(sgNode, ref dtVals, ref owner);
        }
    }
}