(function () {
    "use strict";

    angular
        .module('ngObjectRepository')
          .controller("LicenseController", LicenseController);

    LicenseController.$inject = ["$scope", "ObjectRepositoryDataService",  "Upload", "Notification", "PagerService"];
    function LicenseController($scope, ObjectRepositoryDataService, Upload, Notification, PagerService) {
        var vm = this;
        vm.errorMsg = "";
        vm.moduleList = [];

        //for file upload handler.
        vm.uploadFiles = function (file, errFiles) {
            vm.f = file;
            vm.errFile = errFiles && errFiles[0];
            if (file) {
                file.showprogress = true;

                file.upload = Upload.upload({
                    url: '/api/License',
                    data: { file: file }
                });

                file.upload.then(function (response) {
                    file.result = response.data;
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

        init();

        function init(){
            ObjectRepositoryDataService.getLicencedModules()
                        .then(function (data) {
                            if (data != null && Array.isArray(data)
                                && data.length > 0) {
                                vm.moduleList = data;
                            }
                        });
        }
    }
})();