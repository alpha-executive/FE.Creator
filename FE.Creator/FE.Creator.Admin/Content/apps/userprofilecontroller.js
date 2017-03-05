//user profile controller
; (function () {
    "use strict";

    angular
        .module('ngObjectRepository')
          .controller("UserProfileController", UserProfileController);

    UserProfileController.$inject = ["$scope", "ObjectRepositoryDataService", "Notification", "PagerService", "objectUtilService", "Upload"];

    function UserProfileController($scope, ObjectRepositoryDataService, Notification, PagerService, objectUtilService, Upload) {
        var vm = this;

        vm.basicinfo = {};
        vm.contact = {};
        vm.addresses = [];
        vm.workexperiences = [];
        vm.skills = [];
        vm.educations = [];
        vm.objectDefinitions = [];

        vm.isEditBasicInfo = false;
        vm.isEditContact = false;
        vm.isEditWorkExperiences = false;
        vm.isEditSkills = false;
        vm.isEditAddress = false;
        vm.isEditEducation = false;


        init(CurrentLoginUserID);

        function init(userProfileId) {

            ObjectRepositoryDataService.getLightWeightObjectDefinitions().then(
                function(data){
                    vm.objectDefinitions = data;
                    return vm.objectDefinitions;
            });

            //UserProfile
            reloadUserInfo(userProfileId);

            //Contact
            reloadContact(userProfileId);

            //addresses
            reloadAddresses(userProfileId);
            
            //skills
            reloadSkills(userProfileId);

            //workexperiences
            reloadWorkExperience(userProfileId);

            //Education
            reloadEducations(userProfileId);
        }


        vm.errorMsg = "";
        //for file upload handler.
        vm.uploadFiles = function (file, errFiles, objfield) {
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
                        objfield.fileName = file.result.files[0].fileName;
                        objfield.fileUrl = file.result.files[0].fileUrl;
                        objfield.fileCRC = file.result.files[0].fileCRC;
                        objfield.fileExtension = file.result.files[0].fileExtension;
                        objfield.created = file.result.files[0].created;
                        objfield.updated = file.result.files[0].updated;
                        objfield.freated = file.result.files[0].created;
                        objfield.fileSize = file.result.files[0].fileSize;
                        objfield.fileFullPath = file.result.files[0].fileFullPath;
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


        vm.getObjectDefintionIdByName = function(definitionName){
            for(var i=0; i<vm.objectDefinitions.length; i++){
                if(vm.objectDefinitions[i].objectDefinitionName.toUpperCase() == definitionName.toUpperCase()){
                    return vm.objectDefinitions[i].objectDefinitionID;
                }
            }

            return -1;
        }

        vm.onBasicInfoEditing = function () {
            if (vm.basicinfo.objectID == null) {
                var tmpObj = {};
                tmpObj.objectDefinitionId = vm.getObjectDefintionIdByName("UserInfo");
                tmpObj.objectName = "UserInfo";
                objectUtilService.addStringProperty(tmpObj, "firstName", null);
                objectUtilService.addStringProperty(tmpObj, "lastName", null);
                objectUtilService.addDateTimeProperty(tmpObj, "birthDate", null);
                objectUtilService.addIntegerProperty(tmpObj, "gender", null);
                objectUtilService.addStringProperty(tmpObj, "ID", null);
                objectUtilService.addFileProperty(tmpObj, "Image", null);
                objectUtilService.addStringProperty(tmpObj, "userExternalId", CurrentLoginUserID);

                vm.basicinfo = objectUtilService.parseServiceObject(tmpObj);
            }

            vm.isEditBasicInfo = true;
        }

        vm.cancelUserInfoEdit = function () {
            reloadUserInfo(CurrentLoginUserID);
            vm.isEditBasicInfo = false;
        }

        vm.onContactEditing = function () {
            if (vm.contact.objectID == null) {
                var tmpObj = {};
                tmpObj.objectDefinitionId = vm.getObjectDefintionIdByName("Contact");
                tmpObj.objectName = "Contact";

                objectUtilService.addStringProperty(tmpObj, "phone", null);
                objectUtilService.addStringProperty(tmpObj, "mobile", null);
                objectUtilService.addStringProperty(tmpObj, "eMail", null);
                objectUtilService.addStringProperty(tmpObj, "weChat", null);
                objectUtilService.addStringProperty(tmpObj, "QQ", null);
                objectUtilService.addStringProperty(tmpObj, "tweet", null);
                objectUtilService.addStringProperty(tmpObj, "linkedIn", null);
                objectUtilService.addStringProperty(tmpObj, "facebook", null);
                objectUtilService.addStringProperty(tmpObj, "freeNote", null);
                objectUtilService.addStringProperty(tmpObj, "userExternalId", CurrentLoginUserID);

                vm.contact = objectUtilService.parseServiceObject(tmpObj);
            }

            vm.isEditContact = true;
        }
        vm.cancelContactEdit = function () {
            reloadContact(CurrentLoginUserID);
            vm.isEditContact = false;
        }
        vm.onAddressEditing = function () {
            vm.isEditAddress = true;
        }

        vm.onAddNewAddress = function () {
            var address = {};
            address.objectDefinitionId = vm.getObjectDefintionIdByName("Address");
            address.objectName = "Address";

            objectUtilService.addStringProperty(address, "country", null);
            objectUtilService.addStringProperty(address, "province", null);
            objectUtilService.addStringProperty(address, "city", null);
            objectUtilService.addStringProperty(address, "address", null);
            objectUtilService.addStringProperty(address, "address1", null);
            objectUtilService.addStringProperty(address, "postCode", null);
            objectUtilService.addStringProperty(address, "userExternalId", CurrentLoginUserID);

            vm.addresses.push(objectUtilService.parseServiceObject(address));
        }

        vm.deleteAddress = function (addr) {
            deleteObjectFromArrary(vm.addresses, addr, null);
        }

        vm.saveAddress = function () {
            if (vm.addresses.length <= 0)
                return;

            saveServiceObjects(vm.addresses, 0, function (data, currIdx) {
                if (currIdx >= vm.addresses.length)
                {
                    vm.isEditAddress = false;
            
                    if (data != null && data != "" && data.objectID != null) {
                        Notification.success({
                            message: 'Change Saved!',
                            delay: 3000,
                            positionY: 'bottom',
                            positionX: 'right',
                            title: 'Warn',
                        });
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
                }
            });
        }

        vm.cancelAddressEdit = function () {
            reloadAddresses(CurrentLoginUserID);
            vm.isEditAddress = false;
        }
        vm.onEducationEditing = function () {
            vm.isEditEducation = true;
        }
        vm.deleteEducation = function (edu) {
            deleteObjectFromArrary(vm.educations, edu, null);
        }
        vm.onAddEducation = function () {
            var education = {};

            education.objectDefinitionId = vm.getObjectDefintionIdByName("Education");
            education.objectName = "Education";

            objectUtilService.addStringProperty(education, "school", null);
            objectUtilService.addStringProperty(education, "grade", null);
            objectUtilService.addDateTimeProperty(education, "enrollDate", null);
            objectUtilService.addDateTimeProperty(education, "exitDate", null);
            objectUtilService.addStringProperty(education, "degree", null);
            objectUtilService.addStringProperty(education, "userExternalId", CurrentLoginUserID);

            vm.educations.push(objectUtilService.parseServiceObject(education));
        }
        vm.saveEductions = function () {
           
            if (vm.educations.length <= 0)
                return;

            saveServiceObjects(vm.educations, 0, function (data, currIdx) {
                if (currIdx >= vm.educations.length)
                {
                    vm.isEditEducation = false;
            
                    if (data != null && data != "" && data.objectID != null) {
                        Notification.success({
                            message: 'Change Saved!',
                            delay: 3000,
                            positionY: 'bottom',
                            positionX: 'right',
                            title: 'Warn',
                        });
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
                }
            });
        }
        vm.cancelEducationsEdit = function () {
            reloadEducations(CurrentLoginUserID);
            vm.isEditEducation = false;
        }

        vm.onSkillEditing = function () {
            vm.isEditSkills = true;
        }

        vm.onAddSkill = function () {
            var skill = {};
            skill.objectDefinitionId = vm.getObjectDefintionIdByName("Skill");
            skill.objectName = "Technical Skill";

            objectUtilService.addStringProperty(skill, "skillName", null);
            objectUtilService.addStringProperty(skill, "skillDescription", null);
            objectUtilService.addIntegerProperty(skill, "level", null);
            objectUtilService.addStringProperty(skill, "userExternalId", CurrentLoginUserID);

            vm.skills.push(objectUtilService.parseServiceObject(skill));
        }
        vm.saveSkills = function () {
            if (vm.skills.length <= 0)
                return;

            saveServiceObjects(vm.skills, 0, function (data, currIdx) {
                if (currIdx >= vm.skills.length) {
                    vm.isEditSkills = false;

                    if (data != null && data != "" && data.objectID != null) {
                        Notification.success({
                            message: 'Change Saved!',
                            delay: 3000,
                            positionY: 'bottom',
                            positionX: 'right',
                            title: 'Warn',
                        });
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
                }
            });

        }
        vm.deleteSkill = function (skill) {
            deleteObjectFromArrary(vm.skills, skill, null);
        }
        vm.cancelSkillsEdit = function () {
            reloadSkills(CurrentLoginUserID);
            vm.isEditSkills = false;
        }
        vm.onWorkExperienceEditing = function () {
            vm.isEditWorkExperiences = true;
        }
        vm.onSaveWorkExperiences = function () {
            if (vm.workexperiences.length <= 0)
                return;

            saveServiceObjects(vm.workexperiences, 0, function (data, currIdx) {
                if (currIdx >= vm.workexperiences.length) {
                    vm.isEditWorkExperiences = false;

                    if (data != null && data != "" && data.objectID != null) {
                        Notification.success({
                            message: 'Change Saved!',
                            delay: 3000,
                            positionY: 'bottom',
                            positionX: 'right',
                            title: 'Warn',
                        });
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
                }
            });

        }
        vm.onAddWorkExperience = function () {
            var workexperience = {};
            workexperience.objectDefinitionId = vm.getObjectDefintionIdByName("WorkExperience");
            workexperience.objectName = "Work Experience";

            objectUtilService.addStringProperty(workexperience, "company", null);
            objectUtilService.addStringProperty(workexperience, "companyUrl", null);
            objectUtilService.addStringProperty(workexperience, "jobTitle", null);
            objectUtilService.addDateTimeProperty(workexperience, "startDate", null);
            objectUtilService.addDateTimeProperty(workexperience, "endDate", null);
            objectUtilService.addStringProperty(workexperience, "eventDescription", null);
            objectUtilService.addStringProperty(workexperience, "userExternalId", CurrentLoginUserID);
          
            vm.workexperiences.push(objectUtilService.parseServiceObject(workexperience));
        }

        vm.deleteWorkExperience = function (exp) {
            deleteObjectFromArrary(vm.workexperiences, exp, null);
        }

        vm.cancelWorkExperiencesEdit = function () {
            vm.isEditWorkExperiences = false;
            reloadWorkExperience(CurrentLoginUserID)
        }

        vm.saveUserinfo = function () {
            saveServiceObject(vm.basicinfo,
                function (data) {
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
                            vm.basicinfo.objectID = data.objectID;
                            vm.basicinfo.onlyUpdateProperties = true;
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
                    vm.isEditBasicInfo = false;
                });
        }

        vm.saveContact = function () {
            saveServiceObject(vm.contact,
                function (data) {
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
                            vm.contact.objectID = data.objectID;
                            vm.contact.onlyUpdateProperties = true;
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
                    vm.isEditContact = false;
                }
            );
        }

        function saveServiceObject(editingObject, callback) {
            var svcObject = objectUtilService.convertAsServiceObject(editingObject);
            ObjectRepositoryDataService.createOrUpdateServiceObject(
                    svcObject.objectID,
                    svcObject
                ).then(function (data) {
                    if(callback != null)
                        callback(data);
                });
        }

        function saveServiceObjects(objArrary, currIndex, callback) {
            if (objArrary.length > 0) {
                saveServiceObject(objArrary[currIndex],
                function (data) {
                    if (data != null && data != "" && data.objectID != null) {
                        objArrary[currIndex].objectID = data.objectID;
                        objArrary[currIndex].onlyUpdateProperties = true;
                    }

                    currIndex = currIndex + 1;
                    if (currIndex < objArrary.length) {
                        saveServiceObjects(objArrary, currIndex, callback);
                    }

                    if (callback != null) {
                        callback(data, currIndex);
                    }
                })
            }
        }

        function reloadUserInfo(userProfileId) {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                    "UserInfo",
                    ["firstName", "lastName", "birthDate", "gender", "ID", "image", "userExternalId"].join(","),
                    null,
                    null,
                    "userExternalId," + userProfileId
                ).then(function (data) {
                    if (Array.isArray(data) && data.length > 0) {
                        vm.basicinfo = objectUtilService.parseServiceObject(data[0]);
                        return vm.basicinfo;
                    }
                });
        }

        function reloadSkills(userProfileId) {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                  "Skill",
                  ["skillName", "skillDescription", "level", "userExternalId"].join(),
                  null,
                  null,
                  "userExternalId," + userProfileId
              ).then(function (data) {
                  if (Array.isArray(data) && data.length > 0) {
                      for (var i = 0; i < data.length; i++) {
                          var skill = objectUtilService.parseServiceObject(data[i]);
                          vm.skills.push(skill);
                      }
                  }

                  return vm.skills;
              });
        }

        function reloadAddresses(userProfileId) {
            //clear all the existing elements.
            vm.addresses.splice(0, vm.addresses.length);
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                 "Address",
                 ["country", "province", "city", "address", "address1", "postCode", "userExternalId"].join(),
                 null,
                 null,
                 "userExternalId," + userProfileId
             ).then(function (data) {
                 if (Array.isArray(data) && data.length > 0) {
                     for (var i = 0; i < data.length; i++) {
                         var address = objectUtilService.parseServiceObject(data[i]);
                         vm.addresses.push(address);
                     }
                 }

                 return vm.addresses;
             });
        }

        function reloadEducations(userProfileId) {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                  "Education",
                  ["school", "grade", "enrollDate", "exitDate", "degree", "userExternalId"].join(),
                  null,
                  null,
                  "userExternalId," + userProfileId
              ).then(function (data) {
                  if (Array.isArray(data) && data.length > 0) {
                      vm.educations.splice(0, vm.educations.length);
                      for (var i = 0; i < data.length; i++) {
                          var education = objectUtilService.parseServiceObject(data[i]);
                          vm.educations.push(education);
                      }
                  }

                  return vm.educations;
              });
        }

        function reloadWorkExperience(userProfileId) {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                  "WorkExperience",
                  ["company", "companyUrl", "jobTitle", "startDate", "endDate", "eventDescription", "userExternalId"].join(),
                  null,
                  null,
                  "userExternalId," + userProfileId
              ).then(function (data) {
                  if (Array.isArray(data) && data.length > 0) {
                      vm.workexperiences.splice(0, vm.workexperiences.length);
                      for (var i = 0; i < data.length; i++) {
                          var workexperience = objectUtilService.parseServiceObject(data[i]);
                          //var workexperience = {};
                          //workexperience.company = objectUtilService.getStringPropertyValue(data[i], "company");
                          //workexperience.companyUrl = objectUtilService.getStringPropertyValue(data[i], "companyUrl");
                          //workexperience.jobTitle = objectUtilService.getStringPropertyValue(data[i], "jobTitle");
                          //workexperience.startDate = objectUtilService.getDateTimePropertyValue(data[i], "startDate");
                          //workexperience.endDate = objectUtilService.getDateTimePropertyValue(data[i], "endDate");
                          //workexperience.eventDescription = objectUtilService.getStringPropertyValue(data[i], "eventDescription");
                          //workexperience.userExternalId = objectUtilService.getStringPropertyValue(data[i], "userExternalId");

                          vm.workexperiences.push(workexperience);
                      }
                  }

                  return vm.workexperiences;
              });
        }

        function reloadContact(userProfileId) {
            ObjectRepositoryDataService.getServiceObjectsWithFilters(
                             "Contact",
                             ["phone", "mobile", "eMail", "weChat", "QQ", "tweet", "linkedIn", "facebook", "freeNote", "userExternalId"].join(),
                             null,
                             null,
                             "userExternalId," + userProfileId
                         ).then(function (data) {
                             if (Array.isArray(data) && data.length > 0) {
                                 vm.contact = objectUtilService.parseServiceObject(data[0]);

                                 return vm.contact;
                             }
                         });
        }

        function deleteObjectFromArrary(objectArrary, tObj, callback) {
            var idx = objectArrary.indexOf(tObj);
            if (idx >= 0) {
                if (tObj.objectID != null) {
                    ObjectRepositoryDataService.deleteServiceObject(tObj.objectID).then(function (data) {
                        if (data != null && data != "" && data.status == 204) {
                            if (idx >= 0)
                                objectArrary.splice(idx, 1);
                        }

                        if(callback != null)
                            callback(data);
                    })
                }
                else {
                    if (idx >= 0)
                        objectArrary.splice(idx, 1);
                }
            }
        }
    }

})();