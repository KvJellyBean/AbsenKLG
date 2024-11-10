<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Absen._Default" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Absensi</title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 120px;
        }
        #datepicker {
            width: 216px;
        }
    </style>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <link rel="stylesheet" href="/resources/demos/style.css">
    <script src="https://code.jquery.com/jquery-3.7.1.js"></script>
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
    <script>
      $( function() {
          $("#datepicker").datepicker({
              dateFormat: 'dd/mm/yy',
              showOn: 'both',
              buttonText: 'Calendar',
              changeMonth: true,
              changeYear: true,
          });
      } );
    </script>
</head>
<body>
    <h1>Absensi</h1>
    <form id="FormAbsensi" runat="server">
        <table class="auto-style1">
            <tr>
                <td class="auto-style2">NIK - Nama :</td>
                <td>
                    <asp:DropDownList ID="DropDownNikNama" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">Tgl Absen</td>
                <td>
                   <asp:TextBox ID="datepicker" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td>
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="saveButton_Click" />
                </td>
            </tr>
        </table>

        <asp:GridView ID="GridView1" runat="server" style="margin-top: 25px" Width="350px" OnRowDataBound="GridView1_RowDataBound">
        </asp:GridView>

        <asp:GridView ID="GridView2" runat="server" style="margin-top: 25px; white-space: nowrap" >
        </asp:GridView>
        <asp:GridView ID="GridView3" runat="server" style="margin-top: 25px" Width="350px">
        </asp:GridView>
    </form>
</body>
</html>
