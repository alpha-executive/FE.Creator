(function () {
    "use strict";

    //for object defintion group
    angular.module('ngObjectRepository', ['ngRoute', 'ngMessages', 'ui-notification']);

    angular.module('ngObjectRepository').directive('convertToNumber', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return parseInt(val, 10);
                });
                ngModel.$formatters.push(function (val) {
                    return '' + val;
                });
            }
        };
    });

})();