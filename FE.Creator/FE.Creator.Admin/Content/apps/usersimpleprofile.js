$(function () {
    $.ajax({
        url: "/api/custom/SystemUser/GetUserIdByUserLoginName",
        dataType: "json",
        success: function (data) {
            var loginUserId = data.toString();

            loadUserProfile(loginUserId);
        }
    });

    function getServiceObjectPropertyValue(svcObj, propName) {
        if (svcObj == null || svcObj.properties == null || svcObj.properties.length == 0)
            return null;

        for (var i = 0; i < svcObj.properties.length; i++) {
            var prop = svcObj.properties[i];
            if (prop.keyName.toUpperCase() == propName.toUpperCase()) {
                return prop.value;
            }
        }

        return null;
    }

    function loadUserProfile(loginUserId) {

        if (loginUserId == null || loginUserId == "")
            return;

        $.ajax({
            url: "/api/objects/FindServiceObjectsByFilter/UserInfo/"
                + ["firstName", "lastName", "birthDate", "gender", "ID", "image", "userExternalId"].join()
                + "?filters=userExternalId," + loginUserId,
            dataType: "json",
            success: function (data) {
                if (Array.isArray(data) && data.length > 0) {
                    var userinfo = data[0];
                    
                    var birthDateProperty = getServiceObjectPropertyValue(userinfo, "birthDate");
                    if (birthDateProperty != null && birthDateProperty.value != null) {
                        $("#loginUserBirthDate").text(moment(birthDateProperty.value).format("MMM Do"));
                    }

                    var imageProperty = getServiceObjectPropertyValue(userinfo, "Image");
                    if (imageProperty != null && imageProperty.fileUrl != null) {
                        $(".img-curr-login").attr("src", imageProperty.fileUrl);
                    }

                    var userFirstNameProperty = getServiceObjectPropertyValue(userinfo, "firstName");
                    var userLastNameProperty = getServiceObjectPropertyValue(userinfo, "lastName");

                    var userLoginName = "";
                    if (userFirstNameProperty != null && userFirstNameProperty.value != null)
                    {
                        userLoginName += userFirstNameProperty.value;
                    }

                    if (userLastNameProperty != null && userLastNameProperty.value != null)
                    {
                        userLoginName += "," + userLastNameProperty.value;
                    }

                    if (userLoginName != "") {
                        $(".login-user-name").text(userLoginName);
                    }

                    $("#loginUserProfileLink").attr("href", " /ngView/EditOrDisplay/SystemUsers/UserProfileIndex/" + loginUserId);
                }
            }
        });
    }
});