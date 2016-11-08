(function(){
    "use strict";

    var PrimaryTypes = ['String', 'Integer', 'Long', 'Datetime', 'Number', 'Binary'];

    angular
         .module('ngObjectRepository')
           .controller("ObjectDefinitionController", ObjectDefinitionController)
           .controller("ObjectDefintionListController", ObjectDefintionListController)
           .controller('ObjectDefintionEditController', ObjectDefintionEditController);

    angular.module('ngObjectRepository')
            .config(function ($routeProvider, $locationProvider) {
                $routeProvider
                 .when('/objdefs/:groupId', {
                     templateUrl: '/ngView/ObjectRepository/ObjectDefintionList',
                     controller: 'ObjectDefintionListController'
                 })
                .when('/objdefs/:groupId/:objdefid/edit',{
                    templateUrl: '/ngView/ObjectRepository/ObjectDefintionEdit',
                    controller: 'ObjectDefintionEditController'
                });

                // configure html5 to get links working on jsfiddle
                $locationProvider.html5Mode({
                    enabled: true,
                    requireBase: false
                });
            });

    angular.module('ngObjectRepository').filter("fieldTypeNameFilter", function () {
        return function (field) {
            switch (field.generalObjectDefinitionFiledType) {
                case 0:
                    return PrimaryTypes[field.primeDataType];
                case 1:
                    return 'Reference';
                case 2:
                    return 'Select';
                case 3:
                    return 'File';
                default:
                    return 'Unknown';
            }
        };
    });

    ObjectDefinitionController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService"];
    ObjectDefintionListController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService"];
    ObjectDefintionEditController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService", "Notification"];

    function ObjectDefintionEditController($scope, $route, $routeParams, $location, ObjectRepositoryDataService, Notification) {
        var scopeContext = $scope;
        scopeContext.$route = $route;
        scopeContext.$location = $location;
        scopeContext.$routeParams = $routeParams;
        scopeContext.objectDefId = $routeParams.objdefid;
        scopeContext.groupId = $routeParams.groupId;
        scopeContext.CurrentObjectDefinition = {};
        scopeContext.AvailableObjectDefinitions = [];
        scopeContext.getFieldTemplateUrl = getFieldTemplateUrl;
        scopeContext.addPrimaryField = addPrimaryField;
        scopeContext.addFileUploadField = addFileUploadField;
        scopeContext.addObjectReferenceField = addObjectReferenceField;
        scopeContext.addSingleSelectionField = addSingleSelectionField;
        scopeContext.deleteSingleSelectionItem = deleteSingleSelectionItem;
        scopeContext.deleteObjectDefinitionField = deleteObjectDefinitionField;
        scopeContext.addSingleSelectionItem = addSingleSelectionItem;
        scopeContext.saveChanges = saveChanges;

        Initialize();

        function Initialize() {
            ObjectRepositoryDataService.getObjectDefinitionById(scopeContext.objectDefId)
                               .then(function (data) {
                                   scopeContext.CurrentObjectDefinition = data;

                                   if (scopeContext.CurrentObjectDefinition == null) {
                                       scopeContext.CurrentObjectDefinition = {};
                                   }

                                   if (scopeContext.CurrentObjectDefinition.objectFields == null) {
                                       scopeContext.CurrentObjectDefinition.objectFields = new Array();
                                   }

                                   scopeContext.CurrentObjectDefinition.objectDefinitionGroupID = scopeContext.groupId;
                               });

            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(function (data) {
                scopeContext.AvailableObjectDefinitions = data;
            });
        }

        function getFieldTemplateUrl(typeid) {
            switch (typeid) {
                case 1:  //ObjectReference
                    return "/ngView/ObjectRepository/ObjRefDefinitionField";
                case 2:  //SingleSelection
                    return "/ngView/ObjectRepository/SingleSDefinitionField";
                default: //for File, PrimeType
                    return "/ngView/ObjectRepository/GeneralDefinitionField";
            }
        }

        function addPrimaryField(primeDataType) {
            var primaryDataType = {
                objectDefinitionFieldName: PrimaryTypes[primeDataType],
                objectDefinitionFieldKey: PrimaryTypes[primeDataType],
                generalObjectDefinitionFiledType: 0,
                primeDataType: primeDataType
            };

            scopeContext.CurrentObjectDefinition.objectFields.splice(0,0,primaryDataType);
        }

        function addFileUploadField() {
            var field = {
                objectDefinitionFieldName: 'File Upload',
                objectDefinitionFieldKey: 'File Upload',
                generalObjectDefinitionFiledType: 3
            };
            scopeContext.CurrentObjectDefinition.objectFields.splice(0, 0, field);
        }
        function addObjectReferenceField() {
            var field = {
                objectDefinitionFieldName: 'Object Reference',
                objectDefinitionFieldKey: 'Object Reference',
                generalObjectDefinitionFiledType: 1
            };
            scopeContext.CurrentObjectDefinition.objectFields.splice(0, 0, field);
        }

        function addSingleSelectionField() {
            var field = {
                objectDefinitionFieldName: 'Single Selection Field',
                objectDefinitionFieldKey: 'Single selection Field',
                generalObjectDefinitionFiledType: 2
            };
            scopeContext.CurrentObjectDefinition.objectFields.splice(0, 0, field);
        }

        function deleteSingleSelectionItem(field, item){
            var itemIndex = field.selectionItems.indexOf(item);
            if (itemIndex >= 0) {

                if (item.selectItemID != null) {
                    try {
                        ObjectRepositoryDataService.deleteSingleSelectionFieldItem(item.selectItemID)
                        .then(function (data) {
                            //update the UI
                            field.selectionItems.splice(itemIndex, 1);
                        })
                    }
                    catch (e) {
                        Notification.error({ message: 'Failed: ' + e.message, delay: 5000, positionY: 'bottom', positionX: 'right' });
                    }
                }
                else {
                    field.selectionItems.splice(itemIndex, 1);
                }
            }
        }

        function addSingleSelectionItem(field) {
            if (field.selectionItems == null) {
                field.selectionItems = new Array();
            }

            field.selectionItems.push({
                selectDisplayName: "New Item",
                selectItemKey: "New Item Key"
            });
        }

        function deleteObjectDefinitionField(field) {
            try{
                if (field != null) {
                    var index = scopeContext.CurrentObjectDefinition.objectFields.indexOf(field);
                    //if found
                    if (index >= 0) {

                        //if object defintion is already on server, then delete it.
                        if (field.objectDefinitionFieldID != null) {
                            ObjectRepositoryDataService.deleteObjectDefintionField(field.objectDefinitionFieldID)
                                .then(function (data) {
                                    scopeContext.CurrentObjectDefinition.objectFields.splice(index, 1);
                                });
                        }
                        else {
                            scopeContext.CurrentObjectDefinition.objectFields.splice(index, 1);
                        }
                    }
                }
            }
            catch (e) {
                Notification.error({ message: 'Delete Faild: ' + e.message, delay: 5000, positionY: 'bottom', positionX: 'right' });
            }
        }

        function saveChanges() {
            try{
                ObjectRepositoryDataService.createOrUpdateObjectDefintion(scopeContext.CurrentObjectDefinition.objectDefinitionID,
                    scopeContext.CurrentObjectDefinition)
                .then(function (data) {
                    Notification.success({ message: 'Change Saved!', delay: 3000, positionY: 'bottom', positionX: 'right' });
                });
            }
            catch (e) {
                Notification.error({ message: 'Change Faild: ' + e.message, delay: 5000, positionY: 'bottom', positionX: 'right' });
            }
        }
    }

    function ObjectDefintionListController($scope, $route, $routeParams, $location, ObjectRepositoryDataService) {
        var scopeContext = $scope;
        scopeContext.$route = $route;
        scopeContext.$location = $location;
        scopeContext.$routeParams = $routeParams;
        scopeContext.groupId = $routeParams.groupId;
        scopeContext.ObjectDefintions = [];
        scopeContext.deleteObjectDefintion = deleteObjectDefintion;

        Activate();

        function Activate(){
            return ObjectRepositoryDataService.getObjectDefintionsbyGroup(scopeContext.groupId)
                    .then(function(data){
                        scopeContext.ObjectDefintions = data;

                        return scopeContext.ObjectDefintions;
                    });
        }

        function deleteObjectDefintion(defintionId) {
            ObjectRepositoryDataService.deleteObjectDefintion(defintionId)
            .then(function () {
                    //delete the Object defintion in the list model, angular will update the UI automatically.
                    if(scopeContext.ObjectDefintions != null){
                        scopeContext.ObjectDefintions.forEach(function(item, index){
                            if (item.objectDefinitionID == defintionId) {
                                scopeContext.ObjectDefintions.splice(index, 1);
                            }
                        });
                    }
            });
        }
    }

    function ObjectDefinitionController($scope, $route, $routeParams, $location, ObjectRepositoryDataService) {

        var scopeContext = $scope;
        scopeContext.$route = $route;
        scopeContext.$location = $location;
        scopeContext.$routeParams = $routeParams;
        scopeContext.DefinitionGroups = [];

        Activate(null);

        function Activate(parentGroupId) {
            return ObjectRepositoryDataService.getObjectDefinitionGroups(parentGroupId)
                        .then(function (data) {
                            scopeContext.DefinitionGroups = data;

                            return scopeContext.DefinitionGroups;
                        });
        }
    };

})();