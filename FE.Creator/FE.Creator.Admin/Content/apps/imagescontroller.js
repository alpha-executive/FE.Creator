(function () {
    "use strict";

    angular
       .module('ngObjectRepository')
         .controller("ImagesController", ImagesController);

    ImagesController.$inject = ["$scope", "$q", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService", "Upload"];

    function ImagesController($scope, $q, ObjectRepositoryDataService, Notification, PagerService, objectUtilService, Upload) {
        var vm = this;
        vm.DEFAULT_IMAGE_ALBURM = 0;
        vm.displayMode = "imageList";
        vm.objectDefinitions = [];
        vm.cancelObject = null;
        vm.editingImageObject = null;
        vm.editingAlburmObject = null;
        vm.images = [];
        vm.imageRows = [];
        vm.alburms = [];
        vm.pager = {};
        vm.pageSize = 20;
        vm.onPageClick = onPageClick;
        vm.alburmTypes = [];
        //by default, it's show all image mode.
        vm.currentAlburm = null;

        init();
        function init() {
            //$scope.$watch("vm.imageRows", null, true);
            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
              function (data) {
                  vm.objectDefinitions = data;

                  return vm.objectDefinitions;
              }).then(function (data) {
                  //get all the categories and books.
                  //vm.reloadImages();
                  initAlburmTypes();
                  onPageChange(1);
                  vm.reloadAlburms();
              });
        }

        function initAlburmTypes() {
            vm.alburmTypes.push({
                typeName: "Standard List",
                typeValue: 1
            });
            vm.alburmTypes.push({
                typeName: "Slide Show",
                typeValue: 2
            });
            vm.alburmTypes.push({
                typeName: "Walter Flow",
                typeValue: 3
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
            objectUtilService.addIntegerProperty(tempObj, "imageCategoryType", 1);
            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        function sliceImages(images) {
            var colsize = 4;
            if (vm.imageRows.length > 0) {
                vm.imageRows.splice(0, vm.imageRows.length);
            }

            for (var i = 0; i < images.length; i += colsize) {
                vm.imageRows.push(images.slice(i, i + colsize));
            }
        }

        function onPageChange(pageIndex) {
            vm.reCalculatePager(pageIndex).then(function (data) {
                if (pageIndex < 1) {
                    pageIndex = 1;
                }
                if (pageIndex > vm.pager.totalPages) {
                    pageIndex = vm.pager.totalPages;
                }
                vm.pager.currentPage = pageIndex;

                vm.reloadImages();
            });
        }

        function onPageClick(pageIndex) {
            if (vm.pager.currentPage == pageIndex)
                return;

            onPageChange(pageIndex);
        }

        vm.reCalculatePager = function (pageIndex) {
            var objDefinitionId = vm.getObjectDefintionIdByName("Photos");
            return ObjectRepositoryDataService.getServiceObjectCount(
                    objDefinitionId,
                    null
                ).then(function (data) {
                    if (!isNaN(data)) {
                        //pager settings
                        if (pageIndex == null || pageIndex < 1)
                            pageIndex = 1;

                        vm.pager = PagerService.createPager(data, pageIndex, vm.pageSize, 10);
                        vm.pager.disabledLastPage = pageIndex > vm.pager.totalPages;
                        vm.pager.disabledFirstPage = pageIndex == 1;
                    }

                    return data;
                });
        }

        vm.switchAlburm = function (alburm){
            if (vm.currentAlburm != alburm) {
                vm.currentAlburm = alburm;
            }
        }
        vm.onAlburmDbClick = function (alburm) {
            vm.switchAlburm(alburm);
            //if it's standard show.
            if (alburm.properties.imageCategoryType.value == 1) {
                vm.onPageClick(1);
                vm.setDisplayMode("standardImageList");
            }
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

                        //if it's a new add operation.
                        if ((vm.editingImageObject.objectID == null || vm.editingImageObject.objectID == 0)
                            && vm.images.length >= vm.pager.pageSize) {
                            //redirect to the first page.
                            onPageChange(1);
                        }
                        else {
                            var index = vm.images.indexOf(vm.editingImageObject);
                            if (index >= 0) {
                                vm.images.splice(index, 1, tmpimg);
                            }

                            sliceImages(vm.images);
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
                        if (index >= 0) {
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
            ObjectRepositoryDataService.deleteServiceObject(image.objectID).then(function (data) {

                if (vm.pager.totalPages >= vm.pager.currentPage) {
                    var navPageIndex = vm.images.length - 1 <= 0
                           ? vm.pager.currentPage - 1 : vm.pager.currentPage;
                    onPageChange(navPageIndex);
                }
                else {
                    var index = vm.images.indexOf(image);
                    if (index >= 0) {
                        vm.images.splice(index, 1);
                        sliceImages(vm.images);
                    }
                }
            });
        }

        vm.importImages = function (files, errFiles) {
            vm.displayMode = "imageImport";
            vm.files = files;
            vm.errorFiles = errFiles;
            vm.progress = 0;

            var reloaded = false;
            var uploadcount = 0;
            var objectCreationCount = 0;
            var errorCount = 0;
            
            var calculateProgress = function (uploadCount, creationCount) {
                vm.progress = Math.floor(uploadCount * 60.0 / files.length) +
                                                 Math.floor(creationCount * 40.0 / files.length);
                if (vm.progress == 100 && !reloaded) {
                    vm.displayMode = "imageList";
                    vm.reloadImages();
                    reloaded = true;
                }
            }

            var uploadSingleImage = function (file, totalfiles) {
                var uploadPromise = Upload.upload({
                    url: '/api/Files?thumbinal=true',
                    data: {
                        file: file
                    }
                });

                var objCreationPromise = uploadPromise.then(function (response) {
                    file.result = response.data;
                    var imageobj = createImageServiceObject(vm.DEFAULT_IMAGE_ALBURM, "New Image");
                    if (file.result.files.length > 0) {
                        uploadcount += 1;
                        calculateProgress(uploadcount, objectCreationCount);

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

                    return imageobj;
                }, function (response) {
                    errorCount += 1;
                    if (response.status > 0)
                        vm.errorMsg = errorCount + " of " + totalfiles + " images was failed to be uploaded";
                }, function (evt) {
                });

                objCreationPromise.then(function (data) {
                    if (data != null && data.properties.imageFile != null) {
                        objectUtilService.saveServiceObject(data, function (data) {
                            objectCreationCount += 1;
                            calculateProgress(uploadcount, objectCreationCount);
                        });
                    }
                });
            }
            
            if (files != null && files.length > 0) {
                angular.forEach(files, function (file) {
                    uploadSingleImage(file, files.length);
                });
            }
        }

        vm.uploadImage = function (file, errFiles, imageobj) {
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

        vm.importImagesToAlburm = function () {
            vm.setDisplayMode("imageSelectionList");
        }
        vm.reloadImages = function () {
            var filter = vm.currentAlburm != null ?
                 "imageCategory," + vm.currentAlburm.objectID : null;

            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Photos",
                 ["imageFile", "imageDesc", "imageCategory", "imageSharedLevel"].join(),
                 vm.pager.currentPage,
                 vm.pageSize,
                 filter
             ).then(function (data) {
                 vm.images.splice(0, vm.images.length);
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var image = objectUtilService.parseServiceObject(data[i]);
                         vm.images.push(image);
                     }
                 }

                 //group the images into rows.
                 if (vm.currentAlburm == null || vm.currentAlburm.typeValue == 1) {
                     sliceImages(vm.images);
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