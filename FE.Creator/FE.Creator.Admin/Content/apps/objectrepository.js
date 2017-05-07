(function () {
    "use strict";

    //for object defintion group
    angular.module('ngObjectRepository', ['ngRoute', 'ngMessages', 'ui-notification', 'ui.select', 'ngFileUpload']);

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
    angular.module('ngObjectRepository').directive('autoFocus', function ($timeout) {
        return {
            restrict: 'A',
            link: function (_scope, _element) {
                $timeout(function () {
                    _element[0].focus();
                }, 0);
            }
        };
    });
    angular.module('ngObjectRepository').directive('starRating', function () {
        return {
            restrict: 'EA',
            template:
              '<ul class="star-rating" ng-class="{readonly: readonly}">' +
              '  <li ng-repeat="star in stars" class="star" ng-class="{filled: star.filled}" ng-click="toggle($index)">' +
              '    <i class="fa fa-star"></i>' + // or &#9733
              '  </li>' +
              '</ul>',
            scope: {
                ratingValue: '=ngModel',
                max: '=?', // optional (default is 5)
                onRatingSelect: '&?',
                readonly: '=?'
            },
            link: function (scope, element, attributes) {
                if (scope.max == undefined) {
                    scope.max = 5;
                }
                function updateStars() {
                    scope.stars = [];
                    for (var i = 0; i < scope.max; i++) {
                        scope.stars.push({
                            filled: i < scope.ratingValue
                        });
                    }
                };
                scope.toggle = function (index) {
                    if (scope.readonly == undefined || scope.readonly === false) {
                        scope.ratingValue = index + 1;
                        scope.onRatingSelect({
                            rating: index + 1
                        });
                    }
                };
                scope.$watch('ratingValue', function (oldValue, newValue) {
                    if (newValue || newValue === 0) {
                        updateStars();
                    }
                });
            }
        };
    });
})();