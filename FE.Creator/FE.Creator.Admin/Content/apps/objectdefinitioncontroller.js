(function(){
    "use strict";

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
                .when('/objdefs/:objdefid/edit',{
                    templateUrl: '/ngView/ObjectRepository/ObjectDefintionEdit',
                    controller: 'ObjectDefintionEditController'
                });

                // configure html5 to get links working on jsfiddle
                $locationProvider.html5Mode({
                    enabled: true,
                    requireBase: false
                });
            });

    ObjectDefinitionController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService"];
    ObjectDefintionListController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService"];
    ObjectDefintionEditController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService"];

    function ObjectDefintionEditController($scope, $route, $routeParams, $location, ObjectRepositoryDataService){
        var scopeContext = $scope;
        scopeContext.$route = $route;
        scopeContext.$location = $location;
        scopeContext.$routeParams = $routeParams;
        scopeContext.objectDefId = $routeParams.objdefid;
        scopeContext.CurrentObjectDefinition = {};
        scopeContext.AvailableObjectDefinitions = [];
        scopeContext.getFieldTemplateUrl = getFieldTemplateUrl;
        scopeContext.PrimaryTypes = ['String','Integer','Long','Datetime','Number','Binary'];
        scopeContext.addPrimaryField = addPrimaryField;
        scopeContext.addFileUploadField = addFileUploadField;
        scopeContext.addObjectReferenceField = addObjectReferenceField;
        scopeContext.addSingleSelectionField = addSingleSelectionField;
        scopeContext.deleteSingleSelectionItem = deleteSingleSelectionItem;
        scopeContext.addSingleSelectionItem = addSingleSelectionItem;

        Initialize();

        function Initialize() {
            if (scopeContext.CurrentObjectDefinition.objectFields == null) {
                scopeContext.CurrentObjectDefinition.objectFields = new Array();
            }

            ObjectRepositoryDataService.getObjectDefinitionById(scopeContext.objectDefId)
                               .then(function (data) {
                                   scopeContext.CurrentObjectDefinition = data;
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
            var primaryDataType =  {
                objectDefinitionFieldName: scopeContext.PrimaryTypes[primeDataType],
                objectDefinitionFieldKey: scopeContext.PrimaryTypes[primeDataType],
                generalObjectDefinitionFiledType: 0,
                primeDataType: primeDataType
            };

            scopeContext.CurrentObjectDefinition.objectFields.push(primaryDataType);
        }

        function addFileUploadField() {
            var field = {
                objectDefinitionFieldName: 'File Upload',
                objectDefinitionFieldKey: 'File Upload',
                generalObjectDefinitionFiledType: 3
            };
            scopeContext.CurrentObjectDefinition.objectFields.push(field);
        }
        function addObjectReferenceField() {
            var field = {
                objectDefinitionFieldName: 'Object Reference',
                objectDefinitionFieldKey: 'Object Reference',
                generalObjectDefinitionFiledType: 1
            };
            scopeContext.CurrentObjectDefinition.objectFields.push(field);
        }

        function addSingleSelectionField() {
            var field = {
                objectDefinitionFieldName: 'File Upload',
                objectDefinitionFieldKey: 'File File Upload',
                generalObjectDefinitionFiledType: 2
            };
            scopeContext.CurrentObjectDefinition.objectFields.push(field);
        }

        function deleteSingleSelectionItem(field, item){
            var itemIndex = field.selectionItems.indexOf(item);
            if(itemIndex >= 0){
                field.selectionItems.splice(itemIndex, 1);
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
    }

    function ObjectDefintionListController($scope, $route, $routeParams, $location, ObjectRepositoryDataService) {
        var scopeContext = $scope;
        scopeContext.$route = $route;
        scopeContext.$location = $location;
        scopeContext.$routeParams = $routeParams;
        scopeContext.groupId = $routeParams.groupId;
        scopeContext.ObjectDefintions = [];

        Activate();

        function Activate(){
            return ObjectRepositoryDataService.getObjectDefintionsbyGroup(scopeContext.groupId)
                    .then(function(data){
                        scopeContext.ObjectDefintions = data;

                        return scopeContext.ObjectDefintions;
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