$(document).ready(function () {

    //Datepicker
    $("#BirthDate").datepicker({
        dateFormat: "mm-dd-yyyy",
        changeMonth: true,
        changeYear: true
    });

    //Get details of user
    getUserDataforEdit();
});


// Invoke id of image on change attribute
$("#patient-photo").on('change', function () {
    readURL(this, "#imgoutput");
});


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

function getUserDataforEdit() {
    $.ajax({
        type: "GET",
        url: "/Common/Common/GetCurrentUser",
        success: function (r) {
            let birthdate = formateDate(r.birthDate);
            if (r != null) {
                $("#Id").val(r.id);
                $("#FirstName").val(r.firstName);
                $("#LastName").val(r.lastName);
                $("#BirthDate").val(birthdate);
                $("#Phone").val(r.phoneNumber);
                $("#Email").val(r.email);
                $("#Address").val(r.address);
                var a = $('#imgoutput').val(r.profileImageUrl);
                if (a[0].value) {
                
                    document.getElementById("imgoutput").src = "/Profiles/" + a[0].value;
              
                }
                else {                    
                    document.getElementById("imgoutput").src = "/assets/images/avatar-3.png";
                  
                }
            }
            else {
                toastr.error("Not able to fetch user details");
                return;
            }
        }
    });
}

//On click of save button
$("#SaveUser").click(function (e) {
    EditAndSaveUserDetails();

});


//On click of cancel button
$("#CancelUser").click(function (e) {
     location.replace("https://tangehrine.org/Admin/");
    //location.replace("https://localhost:44309/Admin/");
});

function EditAndSaveUserDetails() {
    
    //getting value from input controls
    var Id = $("#Id").val();
    var FirstName = $("#FirstName").val();
    var LastName = $("#LastName").val();
    var BirthDate = $("#BirthDate").val();
    var Phone = $("#Phone").val();
    var Email = $("#Email").val();
    var Address = $("#Address").val();
    var ProfileImage = $("#patient-photo").get(0);
    var filter = /^[7-9][0-9]{9}$/;
    if (FirstName == null || FirstName == "") {
        toastr.error('please enter first name');
    }
    else if (LastName == null || LastName == "") {
        toastr.error('please enter last name');
    }
    else if (Email == null || Email == "") {
        toastr.error('please enter email');
    }
    else if (BirthDate == null || BirthDate == "") {
        toastr.error('please enter birth date');
    }
    else if (FirstName.length > 50) {
        toastr.error('Length should be in between 1 to 50');
    }
    else if (LastName.length > 50) {
        toastr.error('Length should be in between 1 to 50');
    }
    else if (Address.length > 200) {
        toastr.error('Length should be in between 1 to 200');
    }
    else if (!filter.test(Phone)) {
        toastr.error('Invalid Phone Number');
    }
    else {
        //object creation
        var formData = new FormData();
        formData.append("ProfileImage", ProfileImage.files[0]);
        if (ProfileImage.files[0] != null) {
            var fsize = ProfileImage.files[0].size;
            var file = Math.round((fsize / 1024));
            // The size of the file.
            if (file >= 1024) {
                toastr.error("Image is too Big, please select a file less than 1mb");
                return;
            }
        }
        formData.append("Id", Id);
        formData.append("FirstName", FirstName);
        formData.append("LastName", LastName);
        let birthdate = formateDateSql(BirthDate);
        formData.append("BirthDate", birthdate);
        formData.append("PhoneNumber", Phone);
        formData.append("Email", Email);
        formData.append("Address", Address);
        $.ajax({
            type: "POST",
            url: "/Common/Common/EditProfile",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            dataType: "json",
            success: function (r) {
                if (r.isSuccess) {
                    location.reload();
                    toastr.success(r.message);
                }
                else {
                    toastr.error(r.message)
                    getUserDataforEdit();
                }
            },
            error: function (r) {
                toastr.error(r.message);
            },
        });
    }

}

//Formate date
function formateDate(data) {
    
    var date = new Date(data);
    var month = date.getMonth() + 1;
    return (month.toString().length > 1 ? month : "0" + month) + "-" + (date.getDate().toString().length > 1 ? date.getDate() : "0" + date.getDate()) + "-" + date.getFullYear();
   
}
function formateDateSql(data) {
    var date = new Date(data);
    var month = date.getMonth() + 1;
    return date.getFullYear() + "-" + (month.toString().length > 1 ? month : "0" + month) + "-" + (date.getDate().toString().length > 1 ? date.getDate() : "0" + date.getDate());
}


