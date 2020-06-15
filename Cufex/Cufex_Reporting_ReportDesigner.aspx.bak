<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Reporting_ReportDesigner.aspx.vb" Inherits="SNSsoftware.Cufex_Reporting_ReportDesigner" %>

<%@ Register Assembly="DevExpress.XtraReports.v19.2.Web.WebForms, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
    <script type="text/javascript">
        // The CustomizeMenuActions event handler.
        function CustomizeMenuActions(s, e) {
            var actions = e.Actions;

            // Register the custom delete menu command.
            actions.push({
                text: "Delete",
                imageClassName: "customButton",
                disabled: ko.observable(false),
                visible: true,
                // The clickAction function recieves the client-side report model
                // allowing you interact with the currently opened report.
                clickAction: function () {
                    if (confirm('Are you sure you want to delete this report?')) {

                        PageMethods.DeleteReport();

                        window.location = "ReportDesigner";
                    } else {
                        // Do nothing!
                    }
                },
                container: "menu"
            });

            // Register the custom Close menu command.
            actions.push({
                text: "Close",
                imageClassName: "customButton",
                disabled: ko.observable(false),
                visible: true,
                // The clickAction function recieves the client-side report model
                // allowing you interact with the currently opened report.
                clickAction: function (report) {
                    window.location = "ReportDesigner";
                },
                container: "menu"
            });

        }
        function reportSaved(s, e) {
            s.ReportStorageGetUrls()
                .done(function (urls) {
                    var newName = urls.filter(function (item) { return item.Key === e.Url })[0].Value;
                    //alert(newName);
                    e.Report.displayNameObject(newName);

                });
            s.ResetIsModified();
        }
        function init(s) {
            // Specify settings of a parameter type to be registered.
            var parameterInfo = {
                value: "System.Custom",
                displayValue: "Custom Integer",
                defaultValue: 5,
                specifics: "integer",
                valueConverter: function (val) {
                    return parseInt(val);
                }
            };

            // Obtain a standard parameter editor for the specified type.
            var parameterEditor = s.GetParameterEditor("System.Int64")

            // Register a custom parameter type.
            s.AddParameterType(parameterInfo, parameterEditor);

            // Remove an existing parameter type.
            s.RemoveParameterType("System.DateTime");
        }
    </script>
    <dx:ASPxReportDesigner ID="ASPxReportDesigner" runat="server" ClientSideEvents-CustomizeMenuActions="CustomizeMenuActions">
        <ClientSideEvents ReportSaved="reportSaved" Init="init" />
    </dx:ASPxReportDesigner>
</asp:Content>
