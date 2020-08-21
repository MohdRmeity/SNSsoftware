window.dxDemo = window.dxDemo || {};

dxDemo.colorSchemeIcon = '<svg id="colorSchemeIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><defs><style>.dx_gray{fill:#7b7b7b;}</style></defs><title>Themes copy</title><path class="dx_gray" d="M12,3a9,9,0,0,0,0,18c7,0,1.35-3.13,3-5,1.4-1.59,6,4,6-4A9,9,0,0,0,12,3ZM5,10a2,2,0,1,1,2,2A2,2,0,0,1,5,10Zm3,7a2,2,0,1,1,2-2A2,2,0,0,1,8,17Zm3-8a2,2,0,1,1,2-2A2,2,0,0,1,11,9Zm5,1a2,2,0,1,1,2-2A2,2,0,0,1,16,10Z" /></svg>';

dxDemo.State = {
    isMobileView: false,
    isDesignerMode: false,
    getColorSchema: function () {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0] === "colorSchema") { return pair[1]; }
        }
        return "light";
    }
};

dxDemo.Navigation = {
    replaceUrlValue: function (uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        var newParameterValue = value ? key + "=" + encodeURIComponent(value) : "";
        var newUrl;
        if (uri.match(re)) {
            var separator = !!newParameterValue ? '$1' : "";
            newUrl = uri.replace(re, separator + newParameterValue + '$2');
        }
        else if (!!newParameterValue) {
            newUrl = uri + separator + newParameterValue;
        }
        return newUrl;
    },
    saveToUrl: function (key, value) {
        var uri = location.href;
        var newUrl = this.replaceUrlValue(uri, key, value);
        if (newUrl) {
            if (newUrl.length > 2000) {
                newUrl = this.replaceUrlValue(uri, key, null);
            }
            history.replaceState({}, "", newUrl);
        }
    },
    navigate: function (baseLink) {
        window.location = baseLink + window.location.search;
        window.event.preventDefault ? window.event.preventDefault() : (window.event.returnValue = false);
        return false;
    }
};

function onDashboardTitleToolbarUpdated(s, e) {
    
    var colorSchemaList = {
        "light": "Light",
        "dark": "Dark",
        "carmine": "Carmine",
        "darkmoon": "Dark Moon",
        "greenmist": "Green Mist",
        "darkviolet": "Dark Violet",
        "softblue": "Soft Blue",

        "light.compact": "Light Compact",
        "dark.compact": "Dark Compact",
        
        "light-blue": "Light Blue",
        "dark-blue": "Dark Blue "
    };

    if (dxDemo.Sidebar && DevExpress.devices.real().phone) {
        e.Options.actionItems.unshift(dxDemo.Sidebar.getToolbarItem(s.component));
    }

    e.Options.actionItems.unshift({
        type: "menu",
        icon: "colorSchemeIcon",
        hint: "Theme",
        menu: {
            items: Object.keys(colorSchemaList).map(function (key) { return colorSchemaList[key] }),
            type: 'list',
            selectionMode: 'single',
            selectedItems: [colorSchemaList[dxDemo.State.getColorSchema()]],
            itemClick: function (data, element, index) {
                var newTheme = Object.keys(colorSchemaList)[index];
                dxDemo.Navigation.saveToUrl("colorSchema", newTheme);
                location.reload();
            },
            itemTemplate: function (itemData, itemIndex, itemElement) {
                let theme = Object.keys(colorSchemaList)[itemIndex];
                let container = document.createElement('div');
                container.classList.add('dx-dashboard-flex-parent');

                let imageDiv = document.createElement('div');
                let colorScheme = theme.split(".")[0];
                let sizeScheme = theme.split(".")[1];
                let themeClass = 'dx-dashboard-' + colorScheme;
                imageDiv.classList.add('dx-dashboard-fixed');
                imageDiv.classList.add('dx-dashboard-circle');
                imageDiv.classList.add(themeClass);
                if (sizeScheme === 'compact') {
                    imageDiv.classList.add("dx-dashboard-compact");
                }

                container.appendChild(imageDiv);

                let textDiv = document.createElement('div');
                textDiv.innerText = colorSchemaList[theme];
                container.appendChild(textDiv);

                return container;
            }
        }
    });
}

var panelExtension;

function onBeforeRender(sender) {
    var dashboardControl = sender.GetDashboardControl();
    DevExpress.Dashboard.ResourceManager.registerIcon(dxDemo.colorSchemeIcon);
    dxDemo.Navigation.saveToUrl("mode", dashboardControl.isDesignMode() ? "designer" : "viewer");
    dashboardControl.isDesignMode.subscribe(function (isDesignValue) {
        dxDemo.Navigation.saveToUrl("mode", isDesignValue ? "designer" : "viewer");
        dxDemo.State.isDesignerMode = isDesignValue;
    });

    var svgIcon = ' <svg  id="exportItemIcon" xmlns="http://www.w3.org/2000/svg" x="0px" y="5px"width="15" height="15"viewBox="0 0 172 172"style=" fill:#000000;"><g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal"><path d="M0,172v-172h172v172z" fill="none"></path><g fill="#707070"><path d="M143.33333,0c-15.84505,0 -28.66667,12.82162 -28.66667,28.66667c0,1.98763 0.27995,3.94726 0.67188,5.82292l-65.17187,32.69792c-5.26302,-6.01888 -12.8776,-9.85417 -21.5,-9.85417c-15.84505,0 -28.66667,12.82162 -28.66667,28.66667c0,15.84506 12.82162,28.66667 28.66667,28.66667c8.6224,0 16.23698,-3.83528 21.5,-9.85417l65.17188,32.69792c-0.39192,1.87565 -0.67187,3.83528 -0.67187,5.82292c0,15.84506 12.82162,28.66667 28.66667,28.66667c15.84506,0 28.66667,-12.82161 28.66667,-28.66667c0,-15.84505 -12.82161,-28.66667 -28.66667,-28.66667c-8.6224,0 -16.23698,3.83528 -21.5,9.85417l-65.17187,-32.69792c0.39193,-1.87565 0.67188,-3.83528 0.67188,-5.82292c0,-1.98763 -0.27994,-3.94726 -0.67187,-5.82292l65.17188,-32.69792c5.26302,6.01888 12.8776,9.85417 21.5,9.85417c15.84506,0 28.66667,-12.82161 28.66667,-28.66667c0,-15.84505 -12.82161,-28.66667 -28.66667,-28.66667z"></path></g></g></svg>'



    DevExpress.Dashboard.ResourceManager.registerIcon(svgIcon);

    panelExtension = new DevExpress.Dashboard.DashboardPanelExtension(dashboardControl, { dashboardThumbnail: "./Content/DashboardThumbnail/{0}.png" });
    dashboardControl.registerExtension(panelExtension);

    dashboardControl.registerExtension(new SaveAsDashboardExtension(dashboardControl));
    dashboardControl.registerExtension(new DeleteDashboardExtension(sender));
}
function onExpand() {
    var control = webDesigner.GetDashboardControl();
    panelExtension.showPanelAsync({}).done(function (e) {
        control.surfaceLeft(e.surfaceLeft);
    });
    $(".collapseButton").show();
    $(".expandButton").hide();
    $(".ExpandCollapseStatus").text("Collapse List");
}
function onCollapse() {
    var control = webDesigner.GetDashboardControl();
    panelExtension.hidePanelAsync({}).done(function (e) {
        control.surfaceLeft(e.surfaceLeft);
    });
    $(".collapseButton").hide();
    $(".expandButton").show();
    $(".ExpandCollapseStatus").text("Expand List");
}

function initializeControls() {
    $("#buttonContainer").dxButton({
        text: "",
        onClick: function (param) {
            var selectedDashboardID = webDesigner.GetDashboardId();
            var dashboardState = webDesigner.GetDashboardState();
            var parameters = "ExportDashboard" + "|" + selectedDashboardID + "|" + dashboardState;
            webDesigner.PerformDataCallback(parameters, null);
        }
    });
}

function getDashboardIDs() {
    return webDashboard.cpGetDashboardIDs;
};

function dashboardExportedSuccess(args) {
    DevExpress.ui.notify('A dashboard was exported to ' + args.result, 'success', 5000);
};

function ItemCaptionToolbarUpdated(s, e) {
    var formInstance = null;
    e.Options.actionItems.push({
        type: "button",
        text: "Email",
        icon: "exportItemIcon",
        click: function () {
            var selectedDashboardID = webDesigner.GetDashboardId();
            var parameters = "ExportItem" + "|" + selectedDashboardID + "|" + e.ItemName;
            $("#popup").dxPopup({
                visible: true,
                title: 'SHARE',
                width: 400,
                height: 350,
                //position: { my: "left top", at: "left top", of: $("#ctl00_TableNotloggedIn"), offset: "700 200" },
                position: {
                    my: 'center',
                    at: 'center',
                    of: '#ctl00_TableNotloggedIn'
                },
                dragEnabled: true,
                contentTemplate: function (e) {
                    var formContainer = $("<div id='form'>");
                    formContainer.dxForm({
                        readOnly: false,
                        showColonAfterLabel: false,
                        labelLocation: "top",
                        minColWidth: 300,
                        showValidationSummary: true,
                        colCount: 1,
                        onInitialized: function (e) {
                            formInstance = e.component;
                        },
                        items: [{
                            dataField: "TO",
                            validationRules: [{
                                type: "required",
                                message: "Email is required"
                            }],
                            editorType: "dxTextBox"
                        }, {
                            dataField: "SUBJECT",
                            editorType: "dxTextBox",
                            editorOptions: {
                                value: "Portal Dashboard Export",
                                showClearButton: true
                            }
                        }, {
                            dataField: "BODY",
                            editorType: "dxTextArea"
                        }, {
                            itemType: "button",
                            alignment: "left",
                            buttonOptions: {
                                text: "Send Email",
                                // useSubmitBehavior: true,
                                onClick: function () {
                                    var result = formInstance && formInstance.validate();
                                    //if (result) { alert("Validation executed"); }
                                    //else { alert("Validation NOT executed!"); }

                                    if (result && result.isValid) {
                                        var To = $("#form").dxForm("instance").getEditor("TO");
                                        var sub = $("#form").dxForm("instance").getEditor("SUBJECT");
                                        var body = $("#form").dxForm("instance").getEditor("BODY");

                                        parameters = parameters + "|" + To._changedValue + "|" + sub._changedValue + "|" + body._changedValue;
                                        var dd = s.PerformDataCallback(parameters);
                                        $("#popup").dxPopup("hide");
                                        //DevExpress.ui.notify({
                                        //    message: "Your mail has been sent successfully",
                                        //    position: {
                                        //        my: "center bottom",
                                        //        at: "center bottom",
                                        //        of: '#popup'
                                        //    }
                                        //}, "success", 2000);
                                    }
                                }
                            }
                        }]
                    }).dxForm("instance");

                    e.append(formContainer);
                }
            });
        }
    });
};

function DashboardTitleToolbarUpdated(s, e) {
    onDashboardTitleToolbarUpdated(s,e);
    var formInstance = null;
    e.Options.actionItems.push({
        type: "button",
        text: "Email",
        icon: "exportItemIcon",
        click: function () {
            var selectedDashboardID = webDesigner.GetDashboardId();
            var parameters = "ExportDashboard" + "|" + selectedDashboardID;
            $("#popup").dxPopup({
                visible: true,
                title: 'SHARE',
                width: 400,
                height: 350,
                //position: { my: "left top", at: "left top", of: $("#ctl00_TableNotloggedIn"), offset: "700 200" },
                position: {
                    my: 'center',
                    at: 'center',
                    of: '#ctl00_TableNotloggedIn'
                },
                dragEnabled: true,
                contentTemplate: function (e) {
                    var formContainer = $("<div id='form'>");
                    formContainer.dxForm({
                        readOnly: false,
                        showColonAfterLabel: false,
                        labelLocation: "top",
                        minColWidth: 300,
                        showValidationSummary: true,
                        colCount: 1,
                        onInitialized: function (e) {
                            formInstance = e.component;
                        },
                        items: [{
                            dataField: "TO",
                            validationRules: [{
                                type: "required",
                                message: "Email is required"
                            }],
                            editorType: "dxTextBox"
                        }, {
                            dataField: "SUBJECT",
                            editorType: "dxTextBox",
                            editorOptions: {
                                value: "Portal Dashboard Export",
                                showClearButton: true
                            }
                        }, {
                            dataField: "BODY",
                            editorType: "dxTextArea"
                        }, {
                            itemType: "button",
                            alignment: "left",
                            buttonOptions: {
                                text: "Send Email",
                                // useSubmitBehavior: true,
                                onClick: function () {
                                    var result = formInstance && formInstance.validate();
                                    //if (result) { alert("Validation executed"); }
                                    //else { alert("Validation NOT executed!"); }

                                    if (result && result.isValid) {
                                        var To = $("#form").dxForm("instance").getEditor("TO");
                                        var sub = $("#form").dxForm("instance").getEditor("SUBJECT");
                                        var body = $("#form").dxForm("instance").getEditor("BODY");

                                        parameters = parameters + "|" + To._changedValue + "|" + sub._changedValue + "|" + body._changedValue;
                                        var dd = s.PerformDataCallback(parameters);
                                        $("#popup").dxPopup("hide");
                                        //DevExpress.ui.notify({
                                        //    message: "Your mail has been sent successfully",
                                        //    position: {
                                        //        my: "center bottom",
                                        //        at: "center bottom",
                                        //        of: '#popup'
                                        //    }
                                        //}, "success", 2000);
                                    }
                                }
                            }
                        }]
                    }).dxForm("instance");

                    e.append(formContainer);
                }
            });
        }
    });
};