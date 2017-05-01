(function () {
    'use strict'
    angular
       .module('ngObjectRepository')
         .controller("ContactController", ContactController);

    ContactController.$inject = ["$scope", "$q", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService"];
    function ContactController($scope, $q, ObjectRepositoryDataService, Notification, PagerService, objectUtilService) {
        var vm = this;
        vm.displayMode = "contactList";
        vm.contacts = [];
        vm.objectDefinitions = [];

        init();

        function init() {
            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
                function (data) {
                    vm.objectDefinitions = data;
                    return vm.objectDefinitions;
                });

            reloadcontacts(null);
        }

        function createContact(contactName, contactUID) {
            var tempObj = {};
            tempObj.objectDefinitionId = vm.getObjectDefintionIdByName("GeneralContact");
            tempObj.objectName = contactName;

            objectUtilService.addStringProperty(tempObj, "contactUID", contactUID);
            objectUtilService.addIntegerProperty(tempObj, "tapLevel", 0);
            objectUtilService.addStringProperty(tempObj, "relationship", null);

            tempObj = objectUtilService.parseServiceObject(tempObj);

            return tempObj;
        }

        vm.newContact = function () {
            ObjectRepositoryDataService.getUUID().then(function (data) {
                var contact = createContact('New Contact', data);
                objectUtilService.saveServiceObject(contact, function (data) {
                    var svccontact = objectUtilService.parseServiceObject(data);
                    vm.contacts.unshift(svccontact);
                    Notification.success({
                        message: 'New Contact Added!',
                        delay: 3000,
                        positionY: 'bottom',
                        positionX: 'right',
                        title: 'Warn',
                    });
                });

            });
        }

        function reloadcontacts(pageIndex) {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                "GeneralContact",
                ["contactUID", "tapLevel", "relationship"].join(),
                pageIndex,
                vm.pageSize,
                null
            ).then(function (data) {
                vm.contacts.splice(0, vm.contacts.length);
                if (Array.isArray(data) && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var contact = objectUtilService.parseServiceObject(data[i]);
                        vm.contacts.push(contact);
                    }
                }

                return vm.contacts;
            })
           .then(function(contacts){
               if (Array.isArray(contacts) && contacts.length > 0) {
                   var refIds = [];
                   for (var i = 0; i < contacts.length; i++) {
                       refIds.push(contacts[i].properties.contactUID.value);
                   }

                   ObjectRepositoryDataService.getServiceObjectsWithFilters(
                        "UserInfo",
                        ["firstName", "lastName", "birthDate", "gender", "ID", "image", "userExternalId"].join(","),
                        null,
                        null,
                        "userExternalId," + refIds.join()
                   ).then(function (profiles) {
                       if (Array.isArray(profiles)) {
                           for (var i = 0; i < profiles.length; i++) {
                               var profile = objectUtilService.parseServiceObject(profiles[i]);
                               for (var j = 0; j < vm.contacts.length; j++) {
                                   var contact = vm.contacts[j];
                                   if (profile.properties.UserExternalId.value == contact.properties.contactUID.value) {
                                       contact.properties.profile = profile.properties;
                                   }
                               }
                           }
                       }
                   });
               }
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