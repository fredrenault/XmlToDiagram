<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Diagram drawing from XML File</title>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td style="font-size:large; font-weight:bold;">Data</td>
                <td style="font-size:large; font-weight:bold;">Diagram</td>
            </tr>
            <tr>
                <td><asp:GridView ID="gvXmlData" runat="server" AutoGenerateColumns="true"></asp:GridView></td>
                <td><iframe runat="server" id="frmGraph" width="825" height="625"></iframe></td>
            </tr>
        </table>
    </form>
</body>
</html>
