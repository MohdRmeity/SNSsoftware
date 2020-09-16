// Creates and implements a custom DeleteDashboardExtension class.

function ExportDashboardExtension(_wrapper) {
    var _this = this;
    this._wrapper = _wrapper;
    this._control = _wrapper.GetDashboardControl();
    this._toolbox = this._control.findExtension('toolbox');
    this.name = "dxdde-Export-dashboard";
    this.deleteDashboard = function () {
        if (_this.isExtensionAvailable()) {
            var dashboardid = _this._control.dashboardContainer().id;
            var param = JSON.stringify({ DashboardID: dashboardid, ExtensionName: _this.name });
            
            _this._toolbox.menuVisible(false);
            _this._wrapper.PerformDataCallback(param, function () {

           
                var btn = document.getElementById('ctl00_Cufex_MainContent_btnDownload');
                btn.click();

               
            });
        }
    };
    this._menuItem = {
        id: this.name,
        title: "Export To XML",
        click: this.deleteDashboard,
        selected: ko.observable(false),
        disabled: ko.computed(function () { return !_this._control.dashboard(); }),
        index: 113,
        hasSeparator: true,
        data: _this
    }; 
}

ExportDashboardExtension.prototype.isExtensionAvailable = function () {
    return this._toolbox !== undefined;
};

ExportDashboardExtension.prototype.start = function () {
    if (this.isExtensionAvailable())
        this._toolbox.menuItems.push(this._menuItem);
};
ExportDashboardExtension.prototype.stop = function () {
    if (this.isExtensionAvailable())
        this._toolbox.menuItems.remove(this._menuItem);
};