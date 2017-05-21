;(function () {
    'use strict'
    angular.module("ngObjectRepository").filter('dateformatFilter', function () {
        return function (dateval, format) {
            var dateformatted = moment(dateval).format(format);

            return dateformatted;
        }
    });

    angular
      .module('ngObjectRepository')
        .controller("ArticleController", ArticleController);

    ArticleController.$inject = ["$scope", "$sce", "$timeout", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService"];

    function ArticleController($scope, $sce, $timeout, ObjectRepositoryDataService, Notification, PagerService, objectUtilService) {
        var vm = this;
        vm.articles = [];
        vm.articlegroups = [];
        vm.pager = {};
        vm.pageSize = 7;
        vm.onPageClick = onPageClick;
        vm.cancelObject = null;
        vm.objectDefinitions = [];
        vm.currentArticleGroup = null;
        vm.currentEditingArticleGroup = null;
        vm.currentEditingArticle = null;
        vm.displayMode = "articleList";

        init();

        function init() {
            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
              function (data) {
                  vm.objectDefinitions = data;

                  return vm.objectDefinitions;
              }).then(function (data) {
                  onPageClick(1, true);
              });
        }

        function onPageClick(pageIndex, force) {
            if (vm.pager.currentPage == pageIndex && !force)
                return;

            onPageChange(pageIndex);
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
                vm.reloadArticles(pageIndex);
            });
        }

        function createArticle(objectName) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("Article");
            tempObj.objectName = objectName;
            var groupId = vm.currentArticleGroup == null ? 0 : vm.currentArticleGroup.objectID;
           
            objectUtilService.addStringProperty(tempObj, "articleDesc", null);
            objectUtilService.addIntegerProperty(tempObj, "isOriginal", 0);
            objectUtilService.addFileProperty(tempObj, "articleImage", null);
            objectUtilService.addStringProperty(tempObj, "articleContent", null);
            objectUtilService.addIntegerProperty(tempObj, "articleSharedLevel", 0);
            objectUtilService.addObjectRefProperty(tempObj, "articleGroup", groupId);

            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        function createArticleGroup(objectName) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("ArticleGroup");
            tempObj.objectName = objectName;
            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }
        vm.setDisplayMode = function (mode) {
            if (mode != vm.displayMode) {
                vm.displayMode = mode;
            }
        }

        vm.switchArticleGroup = function(group){
            vm.currentArticleGroup = group;
        }

        
        vm.createOrEditArticle = function (article) {
            if (article == null) {
               vm.currentEditingArticle = createArticle("New Post");
            }
            else {
                vm.currentEditingArticle = article;
            }
            vm.cancelObject = objectUtilService.cloneJsonObject(vm.currentEditingArticle);
            vm.setDisplayMode("articleEditing");
            $timeout(function () {
                initializeckEditor('articleEditor');
            }, 100)
        }

        vm.saveArticle = function(){

        }

        vm.deleteArticle = function (article) {
          
        }

        vm.cancelArticleEditing = function () {
            var index = vm.articles.indexOf(vm.currentEditingArticle);
            if (index >= 0) {
                vm.articles.splice(index, 1, vm.cancelObject);
            }

            vm.setDisplayMode("articleList");
        }

        vm.createOrEditArticleGroup = function (articleGroup) {
            if (articleGroup == null) {
                vm.currentEditingArticleGroup = createArticleGroup("New Group");
            }
            else {
                vm.currentEditingArticleGroup = articleGroup;
            }

            vm.cancelObject = objectUtilService.cloneJsonObject(vm.currentEditingArticleGroup);

            vm.setDisplayMode("articleGroupEditing");
        }

        vm.saveArticleGroup = function () {

        }
        vm.deleteArticleGroup = function () {

        }
        vm.cancelArticleGroupEditing = function () {
            var index = vm.articlegroups.indexOf(vm.currentEditingArticleGroup);
            if (index >= 0) {
                vm.articlegroups.splice(index, 1, vm.cancelObject);
            }
        }

        vm.reloadArticles = function (pageIndex) {
            var filter = vm.currentArticleGroup == null ? null : "articleGroup," + vm.currentArticleGroup.objectID;
            return ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Article",
                 ["articleDesc", "isOriginal", "articleImage", "articleSharedLevel", "articleGroup"].join(),
                 pageIndex,
                 vm.pageSize,
                 filter
             ).then(function (data) {
                 vm.articles.splice(0, vm.articles.length);
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var article = objectUtilService.parseServiceObject(data[i]);
                         //diary.properties.diaryContent.value = $sce.trustAsHtml(article.properties.diaryContent.value);
                         vm.articles.push(article);
                     }
                 }

                 return vm.articles;
             });
        }
    }
})();