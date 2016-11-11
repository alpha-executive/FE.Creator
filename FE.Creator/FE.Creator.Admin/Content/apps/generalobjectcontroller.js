angular.module("ngObjectRepository").config(function (uiSelectConfig) {
    uiSelectConfig.theme = 'bootstrap';
});

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

angular.module("ngObjectRepository")
.filter('objectValueFilter', function () {
    return function (items, field) {
        var out = "";
        if (angular.isArray(items)) {
            for (var idx = 0; idx < items.length; idx++) {
                if (items[idx].keyName == field.objectDefinitionFieldName) {
                    switch(field.generalObjectDefinitionFiledType){
                        case 0:
                            out = '' + items[idx].value.value;
                            break;
                        case 1:
                            out = '' + items[idx].value.objectFieldID;
                            break;
                        case 2:
                            out = '' + items[idx].value.selectedItemID;
                            break;
                        case 3:
                            out = '' + items[idx].value.fileName;
                            break;
                        default:
                            break;
                    }

                    //if found, end the search.
                    break;
                }//end if
            }//end for
        }//end if

        return out;
    }
});

angular.module("ngObjectRepository")
    .controller('GeneralObjectListController', GeneralObjectListController);

GeneralObjectListController.$inject = ["$scope", "ObjectRepositoryDataService"];

function GeneralObjectListController($scope, ObjectRepositoryDataService) {
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
    
    vm.viewMode = 'list';
    vm.currentObjectDefinition = {};
    vm.ObjectDefintions = [];
    vm.ObjectDefGroups = [];
    vm.disabled = false;
    vm.GetObjectDefinitionGroup = GetObjectDefinitionGroup;
    vm.onDefinitionChanged = onDefinitionChanged;
    vm.viewObjectDetails = viewObjectDetails;
    vm.getObjectFieldTemplateUrl = getObjectFieldTemplateUrl;
    vm.editObject = editObject;
    vm.deleteObject = deleteObject;
    vm.currentGeneralObject = {};
    vm.ServiceObjectList = [];

    vm.setViewMode = function (view) {
        vm.viewMode = view;
    }

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

    function viewObjectDetails(objectid) {
        findObjectAndSetView(objectid, 'view')
    }

    function editObject(objectid) {
        findObjectAndSetView(objectid, 'edit');
    }

    function deleteObject(objectid) {
        vm.ServiceObjectList.forEach(function (item, index, arr) {
            if (item.objectID == objectid) {
                vm.ServiceObjectList.splice(index, 1);
            }
        });
    }

    function findObjectAndSetView(objectid, view) {
        for (var idx = 0; idx < vm.ServiceObjectList.length; i++) {
            if (vm.ServiceObjectList[idx].objectID == objectid) {
                vm.currentGeneralObject = vm.ServiceObjectList[idx];

                vm.setViewMode(view);
                break;
            }
        }
    }

    function GetObjectDefinitionGroup(objdef) {
        var foundItem = {};
        vm.ObjectDefGroups.forEach(function (item, index, arr) {
            if (objdef.objectDefinitionGroupID == item.groupID) {
                foundItem = item;
            }
        }
        );

        return foundItem.groupName || "Unknown Group";
    }

    function onDefinitionChanged($item, $model) {
        if ($item != null) {

            var searchColumns = [];
            vm.ObjectDefintions.forEach(function (item, index, arr) {
                if ($item.objectDefinitionID == item.objectDefinitionID) {
                    item.objectFields.forEach(function(of, idx, ar){
                        searchColumns.push(of.objectDefinitionFieldName);
                    });
                }
            });

            ObjectRepositoryDataService.getServiceObjects($item.objectDefinitionID, searchColumns.toString())
                .then(function (data) {
                    vm.ServiceObjectList = data;

                    return vm.ServiceObjectList;
                });
        }
    }

    function getObjectFieldTemplateUrl(objectDefinitionFieldId) {

    }
}