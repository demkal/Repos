<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="TestMobileAppForAuth.TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1> Instagram Authentication Sample</h1>  
<div>  
   <asp:Button ID="btnAuthenticate" runat="server" Text="Authenticate Instagram" OnClick="btnAuthenticate_Click" />  
</div> 
        </div>
    </form>
</body>
</html>
