;(function () {
    "use strict";

    angular
        .module('ngObjectRepository')
          .controller("DocumentController", DocumentController);

    DocumentController.$inject = ["$scope", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService", "Upload"];
    
    function DocumentController($scope, ObjectRepositoryDataService, Notification, PagerService, objectUtilService, Upload) {
        var vm = this;
        //root folder.
        vm.currentEditingDirectory = null;
        vm.currentDirectoryFlag = "0";
        vm.navpaths = [];
        vm.directories = [];
        vm.documents = [];
        vm.objectDefinitions = [];
        vm.displayMode = "list";

        init();

        function init() {
            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
                function(data){
                    vm.objectDefinitions = data;

                    return vm.objectDefinitions;
                }).then(function(data){
                    var rootFolder = createNewFolderObject("Home", vm.currentDirectoryFlag);
                    rootFolder.objectID = 0;
                    vm.currentEditingDirectory = rootFolder;
                    vm.navpaths.push(rootFolder);
                    vm.reloadDirectories();

                    return rootFoler;
                });
        }

        function createNewFolderObject(objectName, directoryFlag) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("FileDirectory");
            tempObj.objectName = objectName;
            objectUtilService.addStringProperty(tempObj, "directoryPathFlag", directoryFlag);
            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        vm.directorEditing = function (dir) {
            vm.displayMode = "dirEditing";
            //it's a new create directory
            if (dir == null) {
                var tempObj = createNewFolderObject("New Folder", vm.currentDirectoryFlag);
                vm.currentEditingDirectory = tempObj;
                vm.directories.push(tempObj);
            }
            else {
                vm.currentEditingDirectory = dir;
            }
        }

        vm.saveDirectory = function () {
            objectUtilService.saveServiceObject(vm.currentEditingDirectory, function (data) {
                if (data == null || data == "" || data.objectID != null) {
                    Notification.success({
                        message: 'Change Saved!',
                        delay: 3000,
                        positionY: 'bottom',
                        positionX: 'right',
                        title: 'Warn',
                    });

                    //update the object id.
                    if (data != null && data != "") {
                        vm.currentEditingDirectory.objectID = data.objectID;
                        vm.displayMode = "list";
                    }
                }
                else {
                    //something error happend.
                    Notification.error({
                        message: 'Change Faild: ' + data.toString(),
                        delay: 5000,
                        positionY: 'bottom',
                        positionX: 'right',
                        title: 'Error'
                    });
                }
            });
        }
        
        vm.folderdbclick = function (folder) {
            vm.displayMode = "list";
            vm.navpaths.push(folder);
            vm.currentEditingDirectory = folder;
            vm.currentDirectoryFlag = folder.objectID.toString();
            vm.reloadDirectories();
        }

        vm.navPath = function (navItem) {
            var index = vm.navpaths.indexOf(navItem);
            vm.navpaths.splice(0, index + 1);
           
            vm.reloadDirectories();
        }
        vm.fileEditing = function () {
            vm.displayMode = "fileEditing";
        }

        vm.reloadDirectories = function () {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                  "FileDirectory",
                  "directoryPathFlag",
                  null,
                  null,
                  "directoryPathFlag," + vm.currentEditingDirectory.objectID
              ).then(function (data) {
                  vm.directories.splice(0, vm.directories.length);
                  if (Array.isArray(data) && data.length > 0) {
                    
                      for (var i = 0; i < data.length; i++) {
                          var directory = objectUtilService.parseServiceObject(data[i]);
                          vm.directories.push(directory);
                      }
                  }

                  return vm.directories;
              });
        }

        vm.reloadFiles = function () {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Documents",
                 ["documentFile", "documentDirectory", "documentVersion", "documentSharedLevel"].join(),
                 null,
                 null,
                 "documentDirectory," + vm.currentFolderFlag
             ).then(function (data) {
                 vm.documents.splice(0, vm.documents.length);
                 if (Array.isArray(data) && data.length > 0) {
                    
                     for (var i = 0; i < data.length; i++) {
                         var document = objectUtilService.parseServiceObject(data[i]);
                         vm.documents.push(document);
                     }
                 }
                 return vm.documents;
             });
        }

        vm.getObjectDefintionIdByName = function (definitionName) {
            for (var i = 0; i < vm.objectDefinitions.length; i++) {
                if (vm.objectDefinitions[i].objectDefinitionName.toUpperCase() == definitionName.toUpperCase()) {
                    return vm.objectDefinitions[i].objectDefinitionID;
                }
            }

            return -1;
        }
    }
})();