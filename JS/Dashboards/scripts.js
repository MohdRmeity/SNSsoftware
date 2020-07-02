﻿window.dxDemo = window.dxDemo || {};

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

function onDashboardTitleToolbarUpdated(args) {
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
        //"carmine.compact": "Carmine Compact",
        //"darkmoon.compact": "Dark Moon Compact",
        //"greenmist.compact": "Green Mist Compact",
        //"darkviolet.compact": "Dark Violet Compact",
        //"softblue.compact": "Soft Blue Compact"

        "light-blue": "Light Blue",
        "dark-blue": "Dark Blue "
    };

    if (dxDemo.Sidebar && DevExpress.devices.real().phone) {
        args.options.actionItems.unshift(dxDemo.Sidebar.getToolbarItem(args.component));
    }

    args.options.actionItems.unshift({
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
    $(".ExpandCollapseStatus").text("Expand List");
}
function onCollapse() {
    var control = webDesigner.GetDashboardControl();
    panelExtension.hidePanelAsync({}).done(function (e) {
        control.surfaceLeft(e.surfaceLeft);
    });
    $(".collapseButton").hide();
    $(".expandButton").show();
    $(".ExpandCollapseStatus").text("Collapse List");
}