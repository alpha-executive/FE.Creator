(function () {
    "use strict";

    //for object defintion group
    angular.module('fePortal', ['ngObjectRepository', 'bootstrapLightbox']);

    angular.module('fePortal').config(function (LightboxProvider) {
        // set a custom template
        LightboxProvider.templateUrl = '/portal/portalhome/AngularLightBoxTemplate';
    });
    
})();