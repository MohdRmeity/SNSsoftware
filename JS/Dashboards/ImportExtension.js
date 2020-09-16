function ImportXMLDashboardExtension(_wrapper) {
    var _this = this;
    this._wrapper = _wrapper;
    this._control = _wrapper.GetDashboardControl();
    this._toolbox = this._control.findExtension('toolbox');
    template: "dx-Import-form",
        this.name = "dxdde-Import-dashboard";

    this._menuItem = {
        id: this.name,
        title: "Import From XML",
        template: "dx-Import-form",
        selected: ko.observable(true),
        disabled: ko.computed(function () { return !_this._control.dashboard(); }),
        index: 113,
        data: _this
    };
}

ImportXMLDashboardExtension.prototype.isExtensionAvailable = function () {
    return this._toolbox !== undefined;
};

ImportXMLDashboardExtension.prototype.start = function () {
    if (this.isExtensionAvailable())
        this._toolbox.menuItems.push(this._menuItem);
};
ImportXMLDashboardExtension.prototype.stop = function () {
    if (this.isExtensionAvailable())
        this._toolbox.menuItems.remove(this._menuItem);
};