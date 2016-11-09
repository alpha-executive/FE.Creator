angular.module("ngObjectRepository").filter('propsFilter', function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            // Let the output be the input untouched
            out = items;
        }

        return out;
    };
});

angular.module("ngObjectRepository").controller('DemoCtrl', function ($scope, ObjectRepositoryDataService) {
    var vm = this;

    vm.disabled = undefined;
    vm.searchEnabled = undefined;

    vm.enable = function () {
        vm.disabled = false;
    };

    vm.disable = function () {
        vm.disabled = true;
    };

    vm.enableSearch = function () {
        vm.searchEnabled = true;
    };

    vm.disableSearch = function () {
        vm.searchEnabled = false;
    };

    vm.currentObjectDefinition = {};
    vm.ObjectDefintions = [];
    vm.ObjectDefGroups = [];
    vm.disabled = false;

    Activate();
    function Activate() {
        ObjectRepositoryDataService.getObjectDefinitionGroups()
            .then(function (data) {
                vm.ObjectDefGroups = data;

                return vm.ObjectDefGroups;
            });

        ObjectRepositoryDataService.getLightWeightObjectDefinitions()
            .then(function (data) {
                vm.ObjectDefintions = data;

                return vm.ObjectDefintions;
            });
    }
    function GetObjectDefinitionGroup(objdef) {
        vm.ObjectDefGroups.forEach(function (item, index, arr) {
                if (objdef.objectDefinitionGroupID == item.groupID) {
                    return item.groupName;
             }
            }
        );

        return "Unknown Group";
    }

    //vm.onSelectCallback = function (item, model) {
    //    vm.counter++;
    //    vm.eventResult = { item: item, model: model };
    //};

    //vm.removed = function (item, model) {
    //    vm.lastRemoved = {
    //        item: item,
    //        model: model
    //    };
    //};

    //vm.tagTransform = function (newTag) {
    //    var item = {
    //        name: newTag,
    //        email: newTag.toLowerCase() + '@email.com',
    //        age: 'unknown',
    //        country: 'unknown'
    //    };

    //    return item;
    //};
});