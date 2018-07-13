(function () {
    "use strict";

    angular
        .module('fePortal')
          .controller("SharedBooksController", SharedBooksController);

    angular
       .module('fePortal')
         .controller("SharedArticlesController", SharedArticlesController);

    angular
       .module('fePortal')
         .controller("SharedImagesController", SharedImagesController);


    SharedBooksController.$inject = ["$scope", "ObjectRepositoryDataService", "PagerService", "objectUtilService"];
    SharedArticlesController.$inject = ["$scope", "ObjectRepositoryDataService","PagerService", "objectUtilService"];
    SharedImagesController.$inject = ["$scope", "ObjectRepositoryDataService", "PagerService", "objectUtilService", "Lightbox"];

    //books
    function SharedBooksController($scope, ObjectRepositoryDataService, PagerService, objectUtilService) {
        var vm = this;

        vm.pager = {};  //for page purpose.
        vm.onPageClick = onPageClick;
        vm.pageSize = 10;
        vm.books = [];

        vm.reCalculatePager = function (pageIndex) {
            return ObjectRepositoryDataService.getSharedBookCount()
                .then(function (data) {
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

        vm.reloadBooks = function () {
            ObjectRepositoryDataService.getSharedBooks(
                 vm.pager.currentPage,
                 vm.pageSize
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

        init();

        function init() {
            onPageChange(1);
        }
        function onPageClick(pageIndex) {
            if (vm.pager.currentPage == pageIndex)
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

                vm.reloadBooks();
            });
        }
       
    }

    //articles (posts)
    function SharedArticlesController($scope, ObjectRepositoryDataService, PagerService, objectUtilService) {
        var vm = this;

        vm.pager = {};  //for page purpose.
        vm.onPageClick = onPageClick;
        vm.pageSize = 10;
        vm.articles = [];


        vm.reCalculatePager = function (pageIndex) {
            return ObjectRepositoryDataService.getSharedArticleCount()
                .then(function (data) {
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

        vm.reloadArticles = function () {
            ObjectRepositoryDataService.getSharedArticles(
                 vm.pager.currentPage,
                 vm.pageSize
             ).then(function (data) {
                 vm.articles.splice(0, vm.articles.length);
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var article = objectUtilService.parseServiceObject(data[i]);
                         vm.articles.push(article);
                     }
                 }

                 return vm.articles;
             });
        }

        init();

        function init() {
            onPageChange(1);
        }
        function onPageClick(pageIndex) {
            if (vm.pager.currentPage == pageIndex)
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

                vm.reloadArticles();
            });
        }
    }

    //images.
    function SharedImagesController($scope, ObjectRepositoryDataService, PagerService, objectUtilService, Lightbox) {
        var vm = this;

        vm.pager = {};  //for page purpose.
        vm.onPageClick = onPageClick;
        vm.pageSize = 10;
        vm.images = [];

        vm.reCalculatePager = function (pageIndex) {
            return ObjectRepositoryDataService.getSharedImageCount()
                .then(function (data) {
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

        vm.showLightboxImage = function (index) {
            Lightbox.openModal(vm.images, index);
        }

        vm.reloadImages = function () {
            ObjectRepositoryDataService.getSharedImages(
                 vm.pager.currentPage,
                 vm.pageSize
             ).then(function (data) {
                 vm.images.splice(0, vm.images.length);
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var image = objectUtilService.parseServiceObject(data[i]);

                         vm.images.push({
                             url: '/api/custom/SharedObjects/DownloadSharedImage/' + image.objectID,
                             thumbUrl: '/api/custom/SharedObjects/DownloadSharedImage/' + image.objectID + '?thumbinal=true',
                             caption: image.properties.imageDesc.value
                         });
                     }
                 }

                 return vm.images;
             });
        }

        init();

        function init() {
            onPageChange(1);
        }
        function onPageClick(pageIndex) {
            if (vm.pager.currentPage == pageIndex)
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

                vm.reloadImages();
            });
        }

    }
})();