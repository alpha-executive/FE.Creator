(function () {
    "use strict";

    angular
        .module('ngObjectRepository')
          .controller("BooksController", BooksController);

    BooksController.$inject = ["$scope", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService", "Upload"];

    function BooksController($scope, ObjectRepositoryDataService, Notification, PagerService, objectUtilService, Upload) {
        var vm = this;
        vm.currentBookCategory = null;
        vm.categoryEditButtonLabel = "New";
        vm.searchText = "";
        vm.books = [];
        vm.bookCategories = [];
        vm.currentEditingBook = null;
        vm.objectDefinitions = [];
        vm.categoryEditMode = "list";
        vm.displayMode = "list";
        vm.viewMode = "listView";
        vm.cancelObject = {};

        init();

        function init() {
            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
                function (data) {
                    vm.objectDefinitions = data;

                    return vm.objectDefinitions;
                }).then(function (data) {
                    //get all the categories and books.
                    vm.reloadBookCategories();
                    vm.reloadBooks();
                });
        }

        function createNewBookCategoryObject(objectName) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("BookCategory");
            tempObj.objectName = objectName;
            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        function createNewBookObject(objectName, bookCategory) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("Books");
            tempObj.objectName = objectName;
            var categoryId = bookCategory == null ? 0 : bookCategory.objectID;

            objectUtilService.addFileProperty(tempObj, "bookFile", null);
            objectUtilService.addStringProperty(tempObj, "bookDesc", null);
            objectUtilService.addStringProperty(tempObj, "bookAuthor", null);
            objectUtilService.addStringProperty(tempObj, "bookVersion", null);
            objectUtilService.addObjectRefProperty(tempObj, "bookCategory", categoryId);
            objectUtilService.addIntegerProperty(tempObj, "bookSharedLevel", 0);
            objectUtilService.addStringProperty(tempObj, "bookISBN", null);

            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        vm.setViewMode = function (viewMode) {
            if (viewMode != vm.viewMode)
                vm.viewMode = viewMode;
        }

        vm.bookEditing = function (book) {
            vm.displayMode = "editing";

            vm.currentEditingBook = book;
            if (vm.currentEditingBook == null) {
                var tempObj = createNewBookObject("New Book", vm.currentBookCategory);
                vm.books.push(tempObj);
                vm.currentEditingBook = tempObj;
            }

            vm.cancelObject = objectUtilService.cloneJsonObject(vm.currentEditingBook);
        }

        vm.bookDelete = function (book) {
            if (book.objectID != 0 && book.objectID != null) {
                ObjectRepositoryDataService.deleteServiceObject(book.objectID);
            }

            var index = vm.books.indexOf(book);
            if (index >= 0) {
                vm.books.splice(index, 1);
            }
        }

        vm.switchCategory = function (category) {
            if (vm.currentBookCategory != category) {
                vm.currentBookCategory = category;

                if (vm.currentBookCategory == null)
                {
                    vm.categoryEditMode = "list";
                }

                vm.reloadBooks();
            }
        }

        vm.categoryEditing = function (category) {
            //it's a new create directory
            if (category == null) {
                var tempObj = createNewBookCategoryObject("New Category");
                vm.currentBookCategory = tempObj;
                vm.bookCategories.push(tempObj);
                vm.categoryEditButtonLabel = "Add";
            }
            else {
                vm.currentBookCategory = category;
                vm.categoryEditButtonLabel = "Update";
            }

            vm.categoryEditMode = "edit";

            //vm.cancelObject = objectUtilService.cloneJsonObject(vm.currentBookCategory);
        }

        vm.deleteCategory = function (category) {
            //skip the root directory.
            if (category.objectID == null || category.objectID == 0)
                return;

            //delete the files under this folder
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Books",
                 "bookCategory",
                 null,
                 null,
                 "bookCategory," + category.objectID
             ).then(function (data) {
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var f = data[i];
                         if (f.objectID != 0 && f.objectID != null) {
                             ObjectRepositoryDataService.deleteServiceObject(f.objectID);
                         }
                     }
                 }
             }).then(function (data) {
                     if (category.objectID != null && category.objectID != 0) {
                         ObjectRepositoryDataService.deleteServiceObject(category.objectID);
                           }

                           //delete the directory object from directory list.
                     var index = vm.directories.indexOf(category);
                           if (index >= 0) {
                               vm.directories.splice(index, 1);
                           }
                   });
        }

        vm.saveBookCategory = function () {
            if (vm.currentBookCategory == null)
                return;

            objectUtilService.saveServiceObject(vm.currentBookCategory, function (data) {
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
                        var tmpCategory = objectUtilService.parseServiceObject(data);
                        var index = vm.bookCategories.indexOf(vm.currentBookCategory);
                        if (index >= 0) {
                            vm.bookCategories.splice(index, 1, tmpCategory);
                        }

                        vm.currentBookCategory = tmpCategory;
                        vm.categoryEditMode = "list";
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

        //for file upload handler.
        vm.uploadFiles = function (file, errFiles, bookobj) {
            vm.f = file;
            vm.errFile = errFiles && errFiles[0];
            if (file) {
                file.showprogress = true;

                file.upload = Upload.upload({
                    url: '/api/Files',
                    data: { file: file }
                });

                file.upload.then(function (response) {
                    file.result = response.data;
                    if (file.result.files.length > 0) {
                        bookobj.objectName = objectUtilService.cloneJsonObject(file.result.files[0].fileName);
                        bookobj.properties.bookFile.fileName = file.result.files[0].fileName;
                        bookobj.properties.bookFile.fileUrl = file.result.files[0].fileUrl;
                        bookobj.properties.bookFile.fileCRC = file.result.files[0].fileCRC;
                        bookobj.properties.bookFile.fileExtension = file.result.files[0].fileExtension;
                        bookobj.properties.bookFile.created = file.result.files[0].created;
                        bookobj.properties.bookFile.updated = file.result.files[0].updated;
                        bookobj.properties.bookFile.freated = file.result.files[0].created;
                        bookobj.properties.bookFile.fileSize = file.result.files[0].fileSize;
                        bookobj.properties.bookFile.fileFullPath = file.result.files[0].fileFullPath;
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

        vm.saveBook = function () {
            objectUtilService.saveServiceObject(vm.currentEditingBook, function (data) {
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
                        var tmpbook = objectUtilService.parseServiceObject(data);
                        var index = vm.books.indexOf(vm.currentEditingBook);
                        if (index >= 0) {
                            vm.books.splice(index, 1, tmpbook);
                        }
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
        vm.cancelBookEditing = function () {
            var index = vm.books.indexOf(vm.currentEditingBook);
            if (index >= 0) {
                vm.cancelObject.objectID == null || vm.cancelObject.objectID == 0 ? vm.books.splice(index, 1) :
                        vm.books.splice(index, 1, vm.cancelObject);
            }

            vm.displayMode = "list";
        }

        vm.reloadBookCategories = function () {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                  "BookCategory",
                  null,
                  null,
                  null,
                  null
              ).then(function (data) {
                  vm.bookCategories.splice(0, vm.bookCategories.length);
                  if (Array.isArray(data) && data.length > 0) {

                      for (var i = 0; i < data.length; i++) {
                          var bookCategory = objectUtilService.parseServiceObject(data[i]);
                          vm.bookCategories.push(bookCategory);
                      }
                  }

                  return vm.bookCategories;
              });
        }

        vm.reloadBooks = function () {
            var categoryFilters = vm.currentBookCategory != null ?
                "bookCategory," + vm.currentBookCategory.objectID : null;

            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Books",
                 ["bookFile", "bookDesc", "bookProfileImageUrl", "bookAuthor", "bookVersion", "bookSharedLevel", "bookCategory", "bookISBN"].join(),
                 null,
                 null,
                 categoryFilters
             ).then(function (data) {
                 vm.books.splice(0, vm.books.length);
                 if (Array.isArray(data) && data.length > 0) {

                     for (var i = 0; i < data.length; i++) {
                         var book = objectUtilService.parseServiceObject(data[i]);
                         vm.books.push(book);
                     }
                 }
                 return vm.books;
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