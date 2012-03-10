<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#rowsOnePartition').change(function () {
                var url = "/Home/FillWithDataOnePartitionKey?numberOfRows=";
                url += $(this).val();
                $('#hrefOnePartition').attr('href', url);
            });

            $('#rowsMultiplePartitions').change(function () {
                var url = "/Home/FillWithDataDifferentPartitionKeys?numberOfRows=";
                url += $(this).val();
                $('#hrefMultiplePartitions').attr('href', url);
            });

            $('#btnQueryByRowkeyAndPartitionKey').click(function () {
                var url = "/Home/QueryByRowkeyAndPartitionKey?partitionKey=";
                url += $('#txtPartitionKey1').val();
                url += "&rowKey=";
                url += $('#txtRowKey1').val();
                $('#hrefQueryByRowkeyAndPartitionKey').attr('href', url);
            });

            $('#btnQueryByRowKey').click(function () {
                var url = "/Home/QueryByRowKey?rowKey=";
                url += $('#txtRowKey2').val();
                $('#hrefQueryByRowKey').attr('href', url);
            });

            $('#btnQueryByIdColumn').click(function () {
                var url = "/Home/QueryByIdColumn?id=";
                url += $('#txtColumnId').val();
                $('#hrefQueryByIdColumn').attr('href', url);
            });

            $('#btnQueryByLastNameColumn').click(function () {
                var url = "/Home/QueryByLastNameColumn?lastName=";
                url += $('#txtLastName').val();
                $('#hrefQueryByLastNameColumn').attr('href', url);
            });
        });
      
    </script> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <a href="/Home/DropTable" >DropTable</a><br />
    <a href="/Home/CreateTable" >CreateTable</a><br />


    <a href="/Home/FillWithDataOnePartitionKey?numberOfRows=100" id="hrefOnePartition">FillWithDataOnePartitionKey</a> - Rows to create: <input type="text" id="rowsOnePartition" value="150" /> <br />
    <a href="/Home/FillWithDataDifferentPartitionKeys?numberOfRows=200" id="hrefMultiplePartitions">FillWithDataDifferentPartitionKeys</a> - Rows to create: <input type="text" id="rowsMultiplePartitions" value="150" /> <br /><br />

    <hr />

    <a href="/Home/QueryByRowkeyAndPartitionKey?partitionKey=X&rowKey=2520710564779428731BqfIbeWABkC0PDs7Q8uh4A" id="hrefQueryByRowkeyAndPartitionKey">QueryByRowkeyAndPartitionKey</a> - PartitionKey: <input type="text" id="txtPartitionKey1" value="" />, RowKey: <input type="text" id="txtRowKey1" value="" /> <input type="button" value="set" id="btnQueryByRowkeyAndPartitionKey" /><br />
    <a href="/Home/QueryByRowKey?rowKey=2520710564779428731GHJ7yJqY_0_Rt6RIK4lBeA" id="hrefQueryByRowKey">QueryByRowKey</a> - RowKey: <input type="text" id="txtRowKey2" value="" /> <input type="button" value="set" id="btnQueryByRowKey" /><br />
    <a href="/Home/QueryByIdColumn?id=X" id="hrefQueryByIdColumn">QueryByIdColumn</a> - ColumnId: <input type="text" id="txtColumnId" value="" /> <input type="button" value="set" id="btnQueryByIdColumn" /><br />
    <a href="/Home/QueryByLastNameColumn?lastName=X" id="hrefQueryByLastNameColumn">QueryByLastNameColumn</a><br /> - LastName: <input type="text" id="txtLastName" value="" /> <input type="button" value="set" id="btnQueryByLastNameColumn" /><br />
    <hr />
    <%if (ViewData["Data"] != null)
      { %>
    <ul>
    <%foreach (var item in (IEnumerable<MvcWebRole1.Controllers.TestEntity>)ViewData["Data"])
      {
          %>
          <li><%=item.PartitionKey + " | " + item.RowKey + " | " + item.FirstName%></li>
          <%
      }%>  
      </ul>
      <%} %>

      <br /><br />
      <hr />
      Elapsed: <%=ViewData["Elapsed"]%>
</asp:Content>
