$(document).ready(function () {
 
});
$("#ProfileImage").on('change', function () {
    readURL(this, "#imgoutput");
});
// Add Designation
function validationDesignationClear() {
    $('#validationDesignation').text('');
}
function saveDesignation() {
    var name = $("#DesignationName").val();
    if (name == null || name == "") {
        $('#validationDesignation').text('please enter designation');
    }
    else if (name.length <= 0 || name.length > 15) {
        $('#validationDesignation').text('Designation must be more than 1 and less then 15 characters');
    }
    else {
        
        var formData = new FormData();
        formData.append("Id", $("#RoleId").val());
        formData.append("Name", $("#DesignationName").val());
        formData.append("ShareAllPatientDetails", $("#ShareAllPatientDetails").is(':checked') ? true : false);

        $(".loader").fadeIn();
        $.ajax({
            url: `/Admin/Designation/AddEditDesignation`,
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (response) {
                $(".loader").fadeOut();
                if (response.isSuccess == true) {
                  //  $("#nav-DesignationMaster").load(location.href + " #nav-DesignationMaster>*", "");

                    $('#cancel-btn-designation').click();
                    toastr.success(response.message);
                    ViewAccessibility();
                  
                }
                else {
                    $('#validationDesignation').text(response.message);
                    ViewAccessibility();
                    //$('#cancel-btn-designation').click();
                }
            },
            error: function (response) {
                toastr.error(response.message);

                //$('#cancel-btn-designation').click();
            },
        });
    }

}
// Fetch designation detail by id and append into Modal popup
function AddAccessibility() {
    $('#AddAccessibilityHeading').text('Add Accessibility');
}
function designationEdit(Id) {
    reset();
    $.ajax({
        type: "GET",
        url: "/Admin/Designation/AddEditDesignation/" + Id,
        data: {},
        success: function (result) {
            $('#DesignationName').val(result.name);
            $('#RoleId').val(result.id);
            if (result.shareAllPatientDetails == true) {
                $("#ShareAllPatientDetails").prop('checked', true);
            }
            $('#AddDesignationModal').modal('show');
            $('#AddAccessibilityHeading').text('Edit Accessibility');
        },
        error: function (result) {
            toastr.error('Designation detail is not available');
            $('#cancel-btn-designation').click();
        }
    });
}
// cancel designation
function reset() {
    document.getElementById('DesignationName').value = '';
    $('#validationDesignation').text('');
    document.getElementById('RoleId').value = 0;
    $("#ShareAllPatientDetails").prop('checked', false);
}
// Delete designation
function deleteDesignation(Id) {
    swal({
        title: "Are you sure you want to delete designation?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/Designation/DeleteDesignation/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                        $('#cancel-btn-designation').click();
                        toastr.success('Accessibility record deleted successfully');
                        ViewAccessibility();
                    }
                },
                error: function (result) {
                    toastr.error('Accessibility record does not deleted');
                    $('#cancel-btn-designation').click();
                    ViewAccessibility();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
//Open designation popup
function openAddDesignationPopup() {
    reset();
    $('#AddDesignationModal').modal('show');
    $('#AddAccessibilityHeading').text('Add Accessibility');
}
// Invoke id of image on change attribute

// Read and check uploaded image validation
function readURL(input, imgControlName) {
    if (input != undefined) {
        var ext = input.files[0].name.substring(input.files[0].name.lastIndexOf('.'))
        if (input.files && input.files[0] && (ext.toLowerCase() == ".png" || ext.toLowerCase() == ".jpeg" || ext.toLowerCase() == ".jpg")) {
            var reader = new FileReader();
            var img = URL.createObjectURL(input.files[0]);
            $(imgControlName).attr('src', img);
            reader.onload = function (e) {
                $(imgControlName).attr('src', e.target.result);
            }
            // reader.readAsDataURL(input.files[0]);

        } else {

            toastr.error("Only Images Allowed (.png , .jpeg , .jpg)");


        }
    }

}
// clear image
function clearImage() {
    var output = document.getElementById('imgoutput');
    output.src = "../assets/images/avatar-3.png";
    output.onload = function () {
        URL.revokeObjectURL(output.src) // free memory
        $("#ProfileImage").empty();
    }
}
//Validation Associate
function validationFirstNameClear() {
    $('#validationFirstName').text('');
}
function validationLastNameClear() {
    $('#validationLastName').text(''); 
}
function validationPhoneNumberClear() {
    $('#validationPhoneNumber').text('');
}
function validationEmailClear() {
    $('#validationEmail').text('');  
}
function validationAddressClear() {
    $('#validationAddress').text('');
}
function validationDesignationAssociateClear() {
    $('#validationDesignationAssociate').text('');
}
function validationBirthDateClear() {
    $('#validationBirthDate').text('');
}
function validationImageClear() {
    $('#validationImage').text('');
}
// Add Associate
function saveUser() {    
    var firstName = $("#firstName").val();
    var lastName = $("#lastName").val();
    var phoneNumber = $("#phoneNumber").val();
    var email = $("#email").val();
    var address = $("#address").val();
    var designation = $("#designation").val();    
    var birthDate = formateDateSql($("#birthDate").val());
    var ProfileImage = $('#ProfileImage').get(0);
    var ProfileImages = ProfileImage.files;
    var emailReg = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var nameReg = /^[A-Za-z]+$/;
  
    if (firstName == null || firstName == "") {
        $('#validationFirstName').text('please enter first name');
        if (lastName == null || lastName == "") {
            $('#validationLastName').text('please enter last name');
        }
        if (phoneNumber == "" ) {
            $('#validationPhoneNumber').text('please enter phone number');
        }
        if (email == null || email == "") {
            $('#validationEmail').text('please enter email');
        }
        if (address == null || address == "") {
             $('#validationAddress').text('please enter address');
        }
        if (designation == null || designation == '') {
            $('#validationDesignationAssociate').text('please select designation');
        }
        if (birthDate == null || birthDate == "") {
            $('#validationBirthDate').text('please enter birth date');
        }
    }
    else if (!nameReg.test(firstName)) {
        $('#validationFirstName').text('please enter valid first name');       
    }
    else if (firstName.length <= 0 || firstName.length > 15) {
        $('#validationFirstName').text('First name must be more than 1 and less then 15 characters');
    }
    else if (lastName == null || lastName == "") {
        $('#validationLastName').text('please enter last name');
    }
    else if (!nameReg.test(lastName)) {
        $('#validationLastName').text('please enter valid last name');
    }
    else if (lastName.length <= 0 || lastName.length > 15) {
        $('#validationLastName').text('Last name must be more than 1 and less then 15 characters');
    }
    else if (phoneNumber == null || phoneNumber == "") {
        $('#validationPhoneNumber').text('please enter phone number');
    }
    else if (!(phoneNumber.length == 10)) {
        $('#validationPhoneNumber').text('Phone number must of 10 digits');
    }
    else if (email == null || email == "") {
        $('#validationEmail').text('please enter email');
    }
    else if (!emailReg.test(email)) {
        $('#validationEmail').text('please enter valid email');
    }
    else if (address == null || address == "") {
        $('#validationAddress').text('please enter address');
    }
    else if (address.length <= 0 || address.length >= 150) {
        $('#validationAddress').text('Address must be more than 1 and less then 150 characters');
    }  
    else if (birthDate == null || birthDate == "") {
        $('#validationBirthDate').text('please enter birth date');
    }
    else {        
        if (birthDate != null || birthDate != "") {
                
                var minDate = Date.parse("01/01/2010");
                var today = new Date();
                var DOB = Date.parse(birthDate);
                if ((DOB >= today)) {
                    $('#validationBirthDate').text('please enter valid birth date');
                }
                else
                {
                    var formData = new FormData();
                    formData.append("ProfileImage", ProfileImage.files[0]);
                    if (ProfileImage.files[0] != null) {
                        var fsize = ProfileImage.files[0].size;
                        var file = Math.round((fsize / 1024));
                        // The size of the file.
                        if (file >= 1024) {
                            $('#validationImage').text("Image is too Big, please select a file less than 1mb");
                            window.location.reload();
                        }
                    }
                    formData.append("Id", $("#UserId").val());
                    formData.append("FirstName", $("#firstName").val());
                    formData.append("LastName", $("#lastName").val());
                    formData.append("BirthDate", birthDate);
                    formData.append("PhoneNumber", $("#phoneNumber").val());
                    formData.append("Email", $("#email").val());
                    formData.append("UserName", $("#email").val());
                    formData.append("Address", $("#address").val());
                    formData.append("RoleName", $("#designation").val());
                    formData.append("BirthDate", $("#birthDate").val());
                    $(".loader").fadeIn();
                    $.ajax({
                        url: `/Admin/UserMaster/AddEditUser`,
                        type: "POST",
                        contentType: false,
                        processData: false,
                        cache: false,
                        data: formData,
                        success: function (response) {
                            $(".loader").fadeOut();
                            if (response.isSuccess == true) {                                
                                //  $("#nav-UserMaster").load(location.href + " #nav-UserMaster>*", "");
                                if (response.message == "Role has been changed") {
                                    window.location.reload();
                                }
                                else {
                                    $('#cancel-btn-user').click();
                                    toastr.success(response.message);
                                    ViewUserAccess();
                                    ViewAssociate();
                                }
                            }
                            else {
                                toastr.error(response.message);
                            }
                        },
                        error: function (response) {   
                            $(".loader").fadeOut();
                             toastr.error(response.message);
                             ViewAssociate();
                            //$('#cancel-btn-user').click();
                        },
                    });
                }
            }
    }
}
// Recover trash detail
function userRecover(id) {
    $(".loader").fadeIn();
    $.ajax({
        type: "POST",
        url: "/Admin/UserMaster/RecoverItemFromTrash/" + id,
        data: {},
        success: function (result) {            
            $(".loader").fadeOut();
            ViewRecoveryItem();
            ViewAssociate();
            ViewUserAccess();
            toastr.success('Data recover successfully');
        },
        error: function (result) {
            toastr.error('Unable to recover data');
        }
    });
}

//Empty trash
function trashDelete(id) {
    swal({
        title: "Are you sure you want to permanently delete the record?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $(".loader").fadeIn();
            $.ajax({
                type: "POST",
                url: "/Admin/UserMaster/DeleteTrash/" + id,
                data: {},
                success: function (result) {
                    $(".loader").fadeOut();
                    if (result == true) {
                        toastr.success('Trash record deleted successfully');
                        ViewRecoveryItem();
                    }
                },
                error: function (result) {
                    $(".loader").fadeOut();
                    toastr.error('Trash record does not deleted');
                    $('#cancel-btn-user').click();
                    ViewRecoveryItem();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}


function userEdit(Id) {
    resetUser();
    $.ajax({
        type: "GET",
        url: "/Admin/UserMaster/AddEditUser/" + Id,
        data: {},
        success: function (result) {
            $('#UserId').val(result.id);
            $('#firstName').val(result.firstName);
            $('#lastName').val(result.lastName);
            $('#birthDate').val(result.dateOfBirth);
            $('#phoneNumber').val(result.phoneNumber);
            $('#email').val(result.email);
            $('#address').val(result.address);
            $('#designation').val(result.role[0]);
            var a = $('#imgoutput').val(result.profileImageUrl);
            if (a[0].value) {
                document.getElementById("imgoutput").src = "../Profiles/" + a[0].value;
            }
            else {
                document.getElementById("imgoutput").src = "../assets/images/avatar-3.png";
            }
    
            $('#AddUserModal').modal('show');
            $('#AddAssociateHeading').text('Edit Associate');       
        },
        error: function (result) {
            toastr.error('User detail is not available');
            $('#cancel-btn-user').click();
        }
    });
}
// cancel Associate
function resetUser() {
    document.getElementById('firstName').value = '';
    document.getElementById('lastName').value = '';
    document.getElementById('phoneNumber').value = '';
    document.getElementById('email').value = '';
    document.getElementById('address').value = '';
    document.getElementById('designation').value = '';
    document.getElementById('birthDate').value = '';
    document.getElementById('UserId').value = 0;

    $('#validationAddress').text('');
    clearImage();
}
//Open Associate popup
function openAddUserPopup() {
    resetUser();
    $('#AddUserModal').modal('show');
    $('#validationLastName').text(''); $('#validationFirstName').text('');
    $('#validationPhoneNumber').text(''); $('#validationAddress').text('');
    $('#validationBirthDate').text('');
    $('#validationDesignationAssociate').text('');
    $('#validationEmail').text('');
   
    //$("#birthDate").datepicker({
    //    dateFormat: "dd-mm-yyyy",
    //    changeMonth: true,
    //    changeYear: true
    //});
    $('#birthDate').datepicker({ format: 'mm-dd-yyyy' });
    $('#AddAssociateHeading').text('Add Associate');
}
// Delete Associate
function deleteUser(Id) {
    swal({
        title: "Are you sure you want to delete user?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/UserMaster/DeleteUser/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                       // $("#nav-UserMaster").load(location.href + " #nav-UserMaster>*", "");
                        $('#cancel-btn-user').click();
                        toastr.success('User record deleted successfully');
                        ViewUserAccess();
                        ViewAssociate();
                        ViewRecoveryItem();
                    }
                },
                error: function (result) {
                    toastr.error('User record does not deleted');
                    $('#cancel-btn-user').click();
                    ViewAssociate();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
//Add user access
function validationUserIdClear() {
    $('#validationUserId').text('');
}
function saveUserAccess() {
    var userID = $("#userId").val();
    if (userID < 0 || userID == 0 || userID == null) {
        $('#validationUserId').text('please select user');
    }
    else {      
        var formData = new FormData();
        formData.append("Id", $("#UserAccessId").val());
        formData.append("UserId", $("#userId").val());
        formData.append("IsAllowForPatientList", $("#patientList").is(':checked') ? true : false);
        formData.append("IsAllowForPatientDetails", $("#patientDetails").is(':checked') ? true : false);
        formData.append("IsAllowForDesignationMaster", $("#designationMaster").is(':checked') ? true : false);
        formData.append("IsAllowForMedicineMaster", $("#medicineMaster").is(':checked') ? true : false);
        formData.append("IsAllowForLabReportMaster", $("#labReportMaster").is(':checked') ? true : false);
        formData.append("IsAllowForUserMaster", $("#userMaster").is(':checked') ? true : false);
        formData.append("IsAllowForRelatioshipMaster", $("#relationshipMaster").is(':checked') ? true : false);
        formData.append("IsAllowForLetterFormatMaster", $("#letterFormatMaster").is(':checked') ? true : false);
        formData.append("IsAllowForUserAccessMaster", $("#userAccessMaster").is(':checked') ? true : false);
        formData.append("IsAllowToAppoint", $("#appointmentSchedule").is(':checked') ? true : false);
        formData.append("IsAllowForTodoList", $("#todoList").is(':checked') ? true : false);
        $(".loader").fadeIn();
        $.ajax({
            url: `/Admin/UserAccess/AddEditUserAccess`,
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (response) {                
                $(".loader").fadeOut();
                if (response.isSuccess == true) {    
                    toastr.success(response.message);                                              
                } else{
                    toastr.success(response.message);
                }
                $('#cancel-btn-useraccess').click();
                ViewUserAccess();
            },
            error: function (response) {
                $(".loader").fadeOut();
                $('#cancel-btn-useraccess').click();
                toastr.error(response.message);
                ViewUserAccess();
            },
        });
    }
}

function userAccessEdit(Id) {
    resetUserAccess();
    $.ajax({
        type: "GET",
        url: "/Admin/UserAccess/AddEditUserAccess/" + Id,
        data: {},
        success: function (result) {
           
            $('#userId').val(result.userId);
            $('#UserAccessId').val(result.id);
            if (result.isAllowForPatientList == true) {
                $("#patientList").prop('checked', true);
            }
            if (result.isAllowForPatientDetails == true) {
                $("#patientDetails").prop('checked', true);
            }
            if (result.isAllowForDesignationMaster == true) {
                $("#designationMaster").prop('checked', true);
            }
            if (result.isAllowForMedicineMaster == true) {
                $("#medicineMaster").prop('checked', true);
            }
            if (result.isAllowForLabReportMaster == true) {
                $("#labReportMaster").prop('checked', true);
            }
            if (result.isAllowForUserMaster == true) {
                $("#userMaster").prop('checked', true);
            }
            if (result.isAllowForRelatioshipMaster == true) {
                $("#relationshipMaster").prop('checked', true);
            }
            if (result.isAllowForLetterFormatMaster == true) {
                $("#letterFormatMaster").prop('checked', true);
            }
            if (result.isAllowForUserAccessMaster == true) {
                $("#userAccessMaster").prop('checked', true);
            }
            if (result.isAllowToAppoint == true) {
                $("#appointmentSchedule").prop('checked', true);
            }
            if (result.isAllowForTodoList == true) {
                $("#todoList").prop('checked', true);
            }
            $('#AddUserAccessModal').modal('show');
            $('#AddUserAccessHeading').text('Edit User Access'); 
        },
        error: function (result) {
            $('#cancel-btn-useraccess').click();
            toastr.error('User access detail is not available');       
        }
    });
}
//Open user access popup
function openAddUserAccessPopup() {
    
    resetUserAccess();
    $('#AddUserAccessModal').modal('show');    
    $('#AddUserAccessHeading').text('Add User Access');
}
// cancel user access
function resetUserAccess() {
    $('#validationUserId').text('');
    document.getElementById('userId').value = 0;
    document.getElementById('UserAccessId').value = 0;
    $("#patientList").prop('checked', false);
    $("#patientDetails").prop('checked', false);
    $("#designationMaster").prop('checked', false);
    $("#medicineMaster").prop('checked', false);
    $("#labReportMaster").prop('checked', false);
    $("#userMaster").prop('checked', false);
    $("#relationshipMaster").prop('checked', false);
    $("#letterFormatMaster").prop('checked', false);
    $("#userAccessMaster").prop('checked', false);
    $("#appointmentSchedule").prop('checked', false);
    $("#todoList").prop('checked', false);
    $('#validationFirstName').text('');
    $('#validationLastName').text('');
    $('#validationPhoneNumber').text('');
    $('#validationEmail').text('');
    $('#validationAddress').text('');
    $('#validationDesignationAssociate').text('');
    $('#validationBirthDate').text('');
    $('#validationImage').text('');
}
// Delete user access
function deleteUserAccess(Id) {
    swal({
        title: "Are you sure you want to delete user access?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/UserAccess/DeleteUserAccess/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                        $('#cancel-btn-useraccess').click();
                        toastr.success('User access record deleted successfully');
                        ViewUserAccess();
                    }
                },
                error: function (result) {
                    $('#cancel-btn-useraccess').click();
                    toastr.error('User access record does not deleted');           
                    ViewUserAccess();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}

function formateDateSql(data) {
    var date = new Date(data);
    var month = date.getMonth() + 1;
    return date.getFullYear() + "-" + (month.toString().length > 1 ? month : "0" + month) + "-" + (date.getDate().toString().length > 1 ? date.getDate() : "0" + date.getDate());
}




