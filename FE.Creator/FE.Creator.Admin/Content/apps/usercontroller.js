(function () {
    "use strict";

    angular
        .module('ngObjectRepository')
          .controller("UserController", UserController);

    UserController.$inject = ["$scope", "ObjectRepositoryDataService", "Notification", "PagerService"];
    function UserController($scope, ObjectRepositoryDataService, Notification, PagerService) {
        var vm = this;
        vm.users = {};
        vm.ResetPassword = ResetPassword;

        init();
        function init() {
            ObjectRepositoryDataService.getUsers().then(
                  function (data) {
                      vm.users = data;
                      return data;
                  });
        }


        function ResetPassword(user) {
            ObjectRepositoryDataService.resetPassword(user.id)
            .then(function (data) {
                Notification.success({
                    message: AppLang.USERMGR_PASS_SAVE_DONE,
                    delay: 3000,
                    positionY: 'bottom',
                    positionX: 'right',
                    title:  AppLang.COMMON_DLG_TITLE_WARN,
                });
            });
        }
    }

})();