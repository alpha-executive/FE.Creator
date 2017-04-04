(function () {
    "use strict";

    angular
       .module('ngObjectRepository')
         .controller("ImagesController", ImagesController);

    ImagesController.$inject = ["$scope", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService", "Upload"];

    function ImagesController($scope, ObjectRepositoryDataService, Notification, PagerService, objectUtilService, Upload) {
        var vm = this;
        vm.DEFAULT_IMAGE_ALBURM = 0;
        vm.displayMode = "imageList";
        vm.objectDefinitions = [];
        vm.cancelObject = null;
        vm.editingImageObject = null;
        vm.editingAlburmObject = null;
        vm.images = [];
        vm.alburms = [];
        vm.pager = {};
        vm.pageSize = 48;

        init();
        function init() {
            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
              function (data) {
                  vm.objectDefinitions = data;

                  return vm.objectDefinitions;
              }).then(function (data) {
                  //get all the categories and books.
                  vm.reloadImages();
                  vm.reloadAlburms();
              });
        }

        function createImageServiceObject(alburm, imageTitle) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("Photos");
            tempObj.objectName = imageTitle;

            objectUtilService.addFileProperty(tempObj, "imageFile", null);
            objectUtilService.addStringProperty(tempObj, "imageDesc", null);
            objectUtilService.addObjectRefProperty(tempObj, "imageCategory", alburm);
            objectUtilService.addIntegerProperty(tempObj, "imageSharedLevel", 0);

            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        function createAlburmServiceObject(alburmName){
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("ImageCategory");
            tempObj.objectName = alburmName;

            objectUtilService.addIntegerProperty(tempObj, "categorySharedLevel", 0);
            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        vm.setDisplayMode = function (mode) {
            if (vm.displayMode != mode) {
                vm.displayMode = mode;
            }
        }
        vm.createOrEditImageAlburm = function (alburm) {
            var svcObj = null;
            if (alburm == null) {
                svcObj = createAlburmServiceObject("New Alburm");
            }
            else {
                svcObj = alburm;
            }

            vm.editingAlburmObject = svcObj;
            vm.cancelObject = objectUtilService.cloneJsonObject(svcObj);
            vm.displayMode = "alburmEdit";

            return svcObj;
        }

        vm.createOrEditImage = function (image) {
            var svcObj = null;
            if (image == null) {
                svcObj = createImageServiceObject(vm.DEFAULT_IMAGE_ALBURM, "New Image");
            }
            else {
                svcObj = image;
            }

            vm.editingImageObject = svcObj;
            vm.cancelObject = objectUtilService.cloneJsonObject(svcObj);
            vm.displayMode = "imageEdit";
        }

        vm.saveEditingImage = function () {
            objectUtilService.saveServiceObject(vm.editingImageObject, function (data) {
                if (data == null || data == "" || data.objectID != null) {
                    //update the object id.
                    if (data != null && data != "") {
                        var tmpimg = objectUtilService.parseServiceObject(data);

                        var index = vm.images.indexOf(vm.editingImageObject);
                        if (index > 0) {
                            vm.images.splice(index, 1, tmpimg);
                        }
                        else {
                            vm.images.unshift(tmpimg);
                        }

                        vm.displayMode = "imageList";
                        Notification.success({
                            message: 'Change Saved!',
                            delay: 3000,
                            positionY: 'bottom',
                            positionX: 'right',
                            title: 'Warn',
                        });
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

        vm.cancelEditingImage = function () {
            var index = vm.images.indexOf(vm.editingImageObject);
            if (index >= 0) {
                vm.images.splice(index, 1, vm.cancelObject);
            }
            vm.displayMode = "imageList";
        }
        
        vm.saveEditingAlburm = function () {
            objectUtilService.saveServiceObject(vm.editingAlburmObject, function (data) {
                if (data == null || data == "" || data.objectID != null) {
                    //update the object id.
                    if (data != null && data != "") {
                        var tmpalburm = objectUtilService.parseServiceObject(data);

                        var index = vm.alburms.indexOf(vm.editingAlburmObject);
                        if (index > 0) {
                            vm.alburms.splice(index, 1, tmpalburm);
                        }
                        else {
                            vm.alburms.unshift(tmpalburm);
                        }

                        vm.displayMode = "alburmList";
                        Notification.success({
                            message: 'Change Saved!',
                            delay: 3000,
                            positionY: 'bottom',
                            positionX: 'right',
                            title: 'Warn',
                        });
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

        vm.canceEditingAlburm = function () {
            var index = vm.alburms.indexOf(vm.editingAlburmObject);
            if (index >= 0) {
                vm.alburms.splice(index, 1, vm.cancelObject);
            }

            vm.displayMode = "alburmList";
        }

        vm.deleteAlburm = function (alburm) {

        }

        vm.deleteImage = function (image) {
            ObjectRepositoryDataService.deleteServiceObject(function (data) {
                var index = vm.images.indexOf(image);
                if (index > 0) {
                    vm.images.splice(index, 1);
                }
            })
        }

        vm.importImages = function (images) {

        }

        vm.uploadFiles = function (file, errFiles, imageobj) {
            vm.f = file;
            vm.errFile = errFiles && errFiles[0];
            if (file) {
                file.showprogress = true;

                file.upload = Upload.upload({
                    url: '/api/Files?thumbinal=true',
                    data: {
                        file: file
                    }
                });

                file.upload.then(function (response) {
                    file.result = response.data;
                    if (file.result.files.length > 0) {
                        imageobj.objectName = objectUtilService.cloneJsonObject(file.result.files[0].fileName);
                        imageobj.properties.imageFile.fileName = file.result.files[0].fileName;
                        imageobj.properties.imageFile.fileUrl = file.result.files[0].fileUrl;
                        imageobj.properties.imageFile.fileCRC = file.result.files[0].fileCRC;
                        imageobj.properties.imageFile.fileExtension = file.result.files[0].fileExtension;
                        imageobj.properties.imageFile.created = file.result.files[0].created;
                        imageobj.properties.imageFile.updated = file.result.files[0].updated;
                        imageobj.properties.imageFile.freated = file.result.files[0].created;
                        imageobj.properties.imageFile.fileSize = file.result.files[0].fileSize;
                        imageobj.properties.imageFile.fileFullPath = file.result.files[0].fileFullPath;
                    }

                    file.showprogress = false;
                }, function (response) {
                    if (response.status > 0)
                        vm.errorMsg = response.status + ': ' + response.data;
                }, function (evt) {
                    file.progress = Math.min(100, parseInt(100.0 *
                                             evt.loaded / evt.total));
                });
            }
        }

        vm.reloadImages = function () {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Photos",
                 ["imageFile", "imageDesc", "imageCategory", "imageSharedLevel"].join(),
                 vm.pager.currentPage,
                 vm.pageSize,
                 null
             ).then(function (data) {
                 vm.images.splice(0, vm.images.length);
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var image = objectUtilService.parseServiceObject(data[i]);
                         vm.images.push(image);
                     }
                 }

                 return vm.images;
             });
        }

        vm.reloadAlburms = function () {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "ImageCategory",
                 ["imageCategoryType", "categorySharedLevel"].join(),
                 null,
                 null,
                 null
             ).then(function (data) {
                 vm.alburms.splice(0, vm.alburms.length);
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var alburm = objectUtilService.parseServiceObject(data[i]);
                         vm.alburms.push(alburm);
                     }
                 }

                 return vm.alburms
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