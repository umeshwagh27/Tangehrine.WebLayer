$(document).ready(function () {
    $("#summernote").summernote({
        height: 200,
    });
});
// Add Medicine
function validationClear() {
    $('#validationMadicine').text('');
    $('#validationLabReport').text('');
    $('#validationRelationship').text('');
    $('#validationLetter').text('');

}
function myFunction() {
   
    var name = $("#MedicineName").val();
    if (name == null || name == "") {
        $('#validationMadicine').text('please enter Medicine name');
    }
    else {
        var formData = new FormData();
        formData.append("Id", $("#MedicineId").val());
        formData.append("MedicineName", $("#MedicineName").val());
        $.ajax({
            url: `/Admin/Medicine/AddEditMedicine`,
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (response) {
                if (response.isSuccess == true) {
                    //location.reload();
                    //$("#nav-MedicineMaster").load(location.href + " #nav-MedicineMaster>*", "");
                    $('#cancel-btn-medicine').click();
                    toastr.success(response.message);
                    ViewMedicine();
                }
                else {
                    $('#validationMadicine').text(response.message);
                    ViewMedicine();
                    //$('#cancel-btn-medicine').click();
                }
            },
            error: function (response) {
                toastr.error(response.message);
                //$('#cancel-btn-medicine').click();
            },
        });
    }

}
// Fetch medicine detail by id and append into Modal popup
function AddMedications() {
    cancelMedicine();
    $('#AddMedicineModalHeading').text('Add Medicine'); 
}

function myEdit(Id) {
    $('#validationMadicine').text('');
    $.ajax({
        type: "GET",
        url: "/Admin/Medicine/AddEditMedicine/" + Id,
        data: {},
        success: function (result) {
            ViewMedicine();
            $('#AddMedicineModal').modal('show');
            $('#AddMedicineModalHeading').text('Edit Medicine');
            $('#MedicineName').val(result.medicineName);
            $('#MedicineId').val(result.id);
            $('#medicineList').DataTable().ajax.reload();
        },
        error: function (result) {
            toastr.error('Medicine detail is not available');
            $('#cancel-btn-medicine').click();
        }
    });
}
// Delete medicine
function deleteMedicine(Id) {    
    swal({
        title: "Are you sure you want to delete Medicine?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/Medicine/DeleteMedicine/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                        //$("#nav-MedicineMaster").load(location.href + " #nav-MedicineMaster>*", "");
                        $('#cancel-btn-medicine').click();
                        toastr.success('Medicine record deleted successfully');
                    }
                    ViewMedicine();
                    $('#medicineList').DataTable().ajax.reload();
                },
                error: function (result) {
                    ViewMedicine();
                    toastr.error('Medicine record does not deleted');
                    $('#cancel-btn-medicine').click();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
// cancel medicine
function cancelMedicine() { 
    $('#validationMadicine').text('');
    document.getElementById('MedicineName').value = '';
    document.getElementById('MedicineId').value = 0;
}
// Add Report
function saveReport() {
    var name = $("#ReportName").val();
    if (name == null || name == "") {
        $('#validationLabReport').text('please enter Report name');
    }
    else if (name.length <= 0 || name.length >= 15) {
        $('#validationLabReport').text('Lab Report name must be more than 1 and less then 15 characters');
    }
    else {
        var formData = new FormData();
        formData.append("Id", $("#ReportId").val());
        formData.append("LabReportName", $("#ReportName").val());
        $.ajax({
            url: `/Admin/LabReport/AddEditLabReport`,
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (response) {
                if (response.isSuccess == true) {
                    //location.reload();
                    //$("#nav-LabReportMaster").load(location.href + " #nav-LabReportMaster>*", "");
                    $('#cancel-btn-report').click();
                    toastr.success(response.message);
                    ViewLabReport();
                }
                else {
                    $('#validationLabReport').text(response.message);
                    ViewLabReport();
                    //$('#cancel-btn-report').click();
                }
            },
            error: function (response) {
                toastr.error(response.message);
                //$('#cancel-btn-report').click();
            },
        });
    }
}

// Fetch report detail by id and append into Modal popup
function AddReport() {
    cancelReport();
    $('#AddLabReportModalHeading').text('Add Lab Report');

}
function labReportEdit(Id) {
    $('#validationLabReport').text('');
    $.ajax({
        type: "GET",
        url: "/Admin/LabReport/AddEditLabReport/" + Id,
        data: {},
        success: function (result) {
            $('#AddLabReportModal').modal('show');
            $('#AddLabReportModalHeading').text('Edit Lab Report')
            $('#ReportName').val(result.labReportName);
            $('#ReportId').val(result.id);
        },
        error: function (result) {
            toastr.error('Lab report detail is not available');
            $('#cancel-btn-report').click();
        }
    });
}

// Delete report
function labReportDelete(Id) {
    swal({
        title: "Are you sure you want to delete lab report?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/LabReport/DeleteLabReport/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                        //$("#nav-LabReportMaster").load(location.href + " #nav-LabReportMaster>*", "");
                        $('#cancel-btn-report').click();
                        toastr.success('Lab report record deleted successfully');
                        ViewLabReport();
                    }
                },
                error: function (result) {
                    toastr.error('Lab report record does not deleted');
                    $('#cancel-btn-report').click();
                    ViewLabReport();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });        
}

// cancel report
function cancelReport() {
    $('#validationLabReport').text('');
    document.getElementById('ReportName').value = '';
    document.getElementById('ReportId').value = 0;   
}
// Add Relationship
function AddRelationships() {
    cancelRelationship();
    $('#AddRelationshipModalHeading').text('Add Relationship');
}
function saveRelationship() {
    var name = $("#RelationshipName").val();
    if (name == null || name == "") {
        $('#validationRelationship').text('please enter relation name');
    }
    else if (name.length <= 0 || name.length >= 10) {
        $('#validationRelationship').text('Relationship name must be more than 1 and less then 10 characters');
    }
    else {
       
        var formData = new FormData();
        formData.append("Id", $("#RelationshipId").val());
        formData.append("RelationshipName", $("#RelationshipName").val());
        $.ajax({
            url: `/Admin/Relationship/AddEditRelationship`,
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (response) {
                if (response.isSuccess == true) {
                   // $("#nav-RelationshipMaster").load(location.href + " #nav-RelationshipMaster>*", "");
                    $('#cancel-btn-relationhip').click();
                    toastr.success(response.message);
                    ViewRelationShip();
                }
                else {
                    $('#validationRelationship').text(response.message);
                    ViewRelationShip();
                    //$('#cancel-btn-relationhip').click();
                }
            },
            error: function (response) {
                toastr.error(response.message);
                ViewRelationShip();
                //$('#cancel-btn-relationhip').click();
            },
        });
    }
}
// Fetch relationship detail by id and append into Modal popup
function relationshipEdit(Id) {
    $('#validationRelationship').text('');
    $.ajax({
        type: "GET",
        url: "/Admin/Relationship/AddEditRelationship/" + Id,
        data: {},
        success: function (result) {
            $('#AddRelationshipModal').modal('show');
            $('#AddRelationshipModalHeading').text('Edit Relationship');
            $('#RelationshipName').val(result.relationshipName);
            $('#RelationshipId').val(result.id);
        },
        error: function (result) {
            toastr.error('Relationship detail is not available');
            $('#cancel-btn-relationhip').click();
        }
    });
}
// Delete relationship
function relationshipDelete(Id) {
    swal({
        title: "Are you sure you want to delete relationship?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/Relationship/DeleteRelationship/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                       // $("#nav-RelationshipMaster").load(location.href + " #nav-RelationshipMaster>*", "");
                        $('#cancel-btn-relationhip').click();
                        toastr.success('Relationship record deleted successfully');
                        ViewRelationShip();
                    }
                },
                error: function (result) {
                    toastr.error('Relationship record does not deleted');
                    $('#cancel-btn-relationhip').click();
                    ViewRelationShip();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });     
}
// cancel relationship
function cancelRelationship() {
    $('#validationRelationship').text('');
    document.getElementById('RelationshipName').value = '';
    document.getElementById('RelationshipId').value = 0;
}
// Add Letter Format

function saveLetterFormat() {    
    var name = $("#TitleName").val();
    var content = $("#summernote").val();
    var emailReg = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var nameReg = /^[A-Za-z]+$/;
    var fullName = $("#fullname").val();
    if(!nameReg.test(fullName)) {
       $('#validationfullname').text('please enter valid name');       
    }
    else if (!emailReg.test($("#Email2").val())) {
        $('#validationEmail2').text('please enter valid email');
        
    }
    else if (!emailReg.test($("#Email").val())) {
        $('#validationEmail').text('please enter valid email');
    }    
    else if (name == null || name == "") {
        $('#validationLetter').text('please enter letter title');
    }
    else if (name.length <= 0 || name.length >= 50) {
        $('#validationLetter').text('please enter more than 1 and less then 50 characters');
    }
    else {
        var formData = new FormData();
        formData.append("Id", $("#TitleId").val());
        formData.append("LetterDate", $("#date").val());
        formData.append("Address", $("#Address").val());
        formData.append("Phone1", $("#Phone1").val());
        formData.append("Phone2", $("#Phone2").val());
        formData.append("Website", $("#Website").val());
        formData.append("Email", $("#Email").val());
        formData.append("ReciverFullName", fullName);
        formData.append("ReciverPossition", $("#Possition").val());
        formData.append("ReciverAddress", $("#Address2").val());
        formData.append("ReciverEmail", $("#Email2").val());
        formData.append("ReciverWebsite", $("#Website2").val());
        formData.append("ReciverPhone", $("#Phone").val());
        formData.append("LetterFormatTitle", $("#TitleName").val());
        formData.append("LetterFormatContent", $("#summernote").val());
        $.ajax({
            url: `/Admin/LetterFormat/AddEditLetterFormat`,
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (response) {
                if (response.isSuccess == true) {
                    $('#cancel-btn-letter').click();
                    toastr.success(response.message);
                    ViewLetterFormats();
                }
                else {
                    $('#validationLetter').text(response.message);
                    ViewLetterFormats();                  
                }
            },
            error: function (response) {
                toastr.error(response.message);
                ViewLetterFormats();
            },
        });
    }
}

// Fetch letter format detail by id and append into Modal popup
function letterFormatEdit(Id) {
    $('#validationLetter').text('');
 
    $.ajax({
        type: "GET",
        url: "/Admin/LetterFormat/AddEditLetterFormat/" + Id,
        data: {},
        success: function (result) {
            $('#AddletterFormatModal').modal('show');
            $('#AddletterFormatModalHeading').text('Edit Letter Format');
            $('#TitleName').val(result.letterFormatTitle);
            $("#date").val(result.date)
            $("#Address").val(result.address);
            $("#Phone1").val(result.phone1);
            $("#Phone2").val(result.phone2);
            $("#Website").val(result.website);
            $("#Email").val(result.email);
            $("#fullname").val(result.reciverFullName);
            $("#Possition").val(result.reciverPossition);
            $("#Address2").val(result.reciverAddress);
            $("#Email2").val(result.reciverEmail);
            $("#Website2").val(result.reciverWebsite);
            $("#Phone").val(result.reciverPhone);
            $('#summernote').summernote('code', result.letterFormatContent);
            $('#TitleId').val(result.id);
        },
        error: function (result) {
            toastr.error('Letter format detail is not available');
            $('#cancel-btn-letter').click();
        }
    });
}
// Delete letter format
function letterFormatDelete(Id) {
    swal({
        title: "Are you sure you want to delete letter format?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: "/Admin/LetterFormat/DeleteLetterFormat/" + Id,
                data: {},
                success: function (result) {
                    if (result == true) {
                        //$("#nav-LetterFormats").load(location.href + " #nav-LetterFormats>*", "");
                        $('#cancel-btn-letter').click();
                        toastr.success('Letter format record deleted successfully');
                        ViewLetterFormats();
                    }
                },
                error: function (result) {
                    toastr.error('Letter format record does not deleted');
                    $('#cancel-btn-letter').click();
                    ViewLetterFormats();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });   
   
}

// cancel letter format
function cancelLetterFormat() {
    $('#validationLetter').text('');
    $('#validationEmail2').text('');
    $('#validationEmail').text('');
    $('#validationfullname').text('');
    $('#TitleName').val('');
    $('#summernote').summernote('reset');
    $('#TitleId').value= 0;
    $("#date").val('');
    $("#Address").val('');
    $("#Phone1").val('');
    $("#Phone2").val('');
    $("#Website").val('');
    $("#Email").val('');
    $("#fullname").val('');
    $("#Possition").val('');
    $("#Address2").val('');
    $("#Email2").val('');
    $("#Website2").val('');
    $("#Phone").val('');
}

//Open letter format popup
function openLetterFormatPopup() {
    cancelLetterFormat();
    $('#AddletterFormatModal').modal('show');
    $('#AddletterFormatModalHeading').text('Add Letter Format');
  
}

// show letter format
function showLetterFormat(Id) {
    cancelLetterFormat();
    $.ajax({
        type: "GET",
        url: "/Admin/LetterFormat/AddEditLetterFormat/" + Id,
        data: {},
        success: function (result) {
            var htmlContent = result.htmlString; // headerHtml + headerMiddleHtml + middleHtml + result.letterFormatTitle + result.letterFormatContent + footerHtml;

            $('.cstm-card').hide();
            $('header').hide();
            $('main').hide();
            $('.admin-sidebar').hide();
            $('#letterFormatPublish').show();
            $('#letterFormatPublish').html(htmlContent);

        },
        error: function (result) {
            toastr.error('Not able to show letter format preview');
        }
    });
}

///Letter format show with back button start/////////////////////

//header html
var headerHtml = "<!DOCTYPE html> <html dir='ltr' lang='en-US'>     <head>         <title>Letter Format | TangEHRine</title>         <link type='image/x-icon' rel='shortcut icon' href='../assets/images/favicon.png' />         <!-- Required meta tags -->         <meta charset='UTF-8' />         <meta name='HandheldFriendly' content='true' />         <meta name='viewport' content='width=device-width, initial-scale=1.0' />          <link rel='preconnect' href='https://fonts.googleapis.com' />         <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin />         <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@200;300;400;500;600;700;800;900&display=swap' rel='stylesheet' />          <style type='text/css'>             /*********************  Default-CSS  *********************/              input[type='file']::-webkit-file-upload-button {                 cursor: pointer;             }              input[type='file']::-moz-file-upload-button {                 cursor: pointer;             }              input[type='file']::-ms-file-upload-button {                 cursor: pointer;             }              input[type='file']::-o-file-upload-button {                 cursor: pointer;             }              input[type='file'],             a[href],             input[type='submit'],             input[type='button'],             input[type='image'],             label[for],             select,             button,             .pointer {                 cursor: pointer;             }              ::-moz-focus-inner {                 border: 0px solid transparent;             }              ::-webkit-focus-inner {                 border: 0px solid transparent;             }              *::-moz-selection {                 color: #fff;                 background: #000;             }              *::-webkit-selection {                 color: #fff;                 background: #000;             }              *::-webkit-input-placeholder {                 color: #333333;                 opacity: 1;             }              *:-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *::-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *:-ms-input-placeholder {                 color: #333333;                 opacity: 1;             }             html {                 font-size: 18px;             }             html body {                 font-family: 'Poppins', sans-serif;                 margin: 0;                 line-height: 1.5;                 font-size: 1rem;             }              a,             div a:hover,             div a:active,             div a:focus,             button {                 text-decoration: none;                 -webkit-transition: all 0.5s ease 0s;                 -moz-transition: all 0.5s ease 0s;                 -ms-transition: all 0.5s ease 0s;                 -o-transition: all 0.5s ease 0s;                 transition: all 0.5s ease 0s;             }              a,             span,             div a:hover,             div a:active,             button {                 text-decoration: none;             }              *::after,             *::before,             * {                 -webkit-box-sizing: border-box;                 -moz-box-sizing: border-box;                 -ms-box-sizing: border-box;                 -o-box-sizing: border-box;                 box-sizing: border-box;             }              .no-list li,             .no-list ul,             .no-list ol,             footer li,             footer ul,             footer ol,             header li,             header ul,             header ol {                 list-style: inside none none;             }              .no-list ul,             .no-list ol,             footer ul,             footer ol,             header ul,             header ol {                 margin: 0;                 padding: 0;             }              a {                 outline: none;                 color: #555;             }              a:hover {                 color: #000;             }              body .clearfix,             body .clear {                 clear: both;                 line-height: 100%;             }              body .clearfix {                 height: auto;             }              * {                 outline: none !important;             }              table {                 border-collapse: collapse;                 border-spacing: 0;             }              ul:after,             li:after,             .clr:after,             .clearfix:after,             .container:after,             .grve-container:after {                 clear: both;                 display: block;                 content: '';             }              div input,             div select,             div textarea,             div button {                 font-family: 'Poppins', sans-serif;             }              body h1,             body h2,             body h3,             body h4,             body h5,             body h6 {                 font-family: 'Poppins', sans-serif;                 line-height: 1.5;                 color: #333;                 font-weight: 600;                 margin: 0 0 15px;             }              body h1:last-child,             body h2:last-child,             body h3:last-child,             body h4:last-child,             body h5:last-child,             body h6:last-child {                 margin-bottom: 0;             }              .h2,             h2 {                 font-size: 1.7rem;             }              div select {                 overflow: hidden;                 text-overflow: ellipsis;                 white-space: nowrap;             }              img,             svg {                 margin: 0 auto;                 max-width: 100%;                 max-height: 100%;                 width: auto;                 height: auto;                 vertical-align: top;             }              body p {                 margin: 0 0 25px;                 padding: 0;             }              body p:empty {                 margin: 0;                 line-height: 0;             }              body p:last-child {                 margin-bottom: 0;             }              p strong {                 font-weight: 600;             }              strong {                 font-weight: 600;             }              .a-left {                 text-align: left;             }              .a-right {                 text-align: right;             }              .a-center {                 text-align: center;             }              .hidden {                 display: none !important;             }              body .container .container {                 width: 100%;                 max-width: 100%;             }              /*********************  Default-CSS close  *********************/                    .back-btn {                 width: 48px;                 fill: #fff;                 margin-right: auto;             }             .back-btn:hover,             .action-btns:hover {                 opacity: 0.5;             }             .action-btns {                 background: transparent;                 border: none;                 height: 30px;                 fill: #fff;                 padding: 0 25px;             }             main {                 padding: 5% 10%;             }             footer img {                 width: 100%;                 vertical-align: top;             }   #headerLetter {                 background: rgba(0, 0, 0, 0.5);                 padding: 30px;                 display: flex;                 align-items: center;             }             .back-btn {                 width: 48px;                 fill: #fff;                 margin-right: auto;             }             .back-btn:hover,             .action-btns:hover {                 opacity: 0.5;             }             .action-btns {                 background: transparent;                 border: none;                 height: 30px;                 fill: #fff;                 padding: 0 25px;             }             main {                 padding: 5% 10%;             }             footer img {                 width: 100%;                 vertical-align: top;             }      </style>     </head>     <body>";

var headerMiddleHtml = "<header id='headerLetter'>             <a href='#' onclick='HideLetterFormat()' class='back-btn'>                 <svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 48 30' style='enable-background: new 0 0 48 30;' xml:space='preserve'>                     <path d='M13.3,29.4l-12.7-13c-0.8-0.8-0.8-2.1,0-2.9l12.7-13c0.8-0.8,2-0.8,2.8,0c0.8,0.8,0.8,2.1,0,2.9L6.8,13H46 c1.1,0,2,0.9,2,2s-0.9,2-2,2H6.8l9.3,9.5c0.8,0.8,0.8,2.1,0,2.9C15.4,30.2,14.1,30.2,13.3,29.4z' />                 </svg>             </a>              <button class='action-btns'>                 <svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 33 29' style='enable-background: new 0 0 33 29;' xml:space='preserve'>                     <path                         d='M28,8.1H4.9C2.2,8.1,0,10.2,0,12.9v9.7h6.6V29h19.8v-6.4H33v-9.7C33,10.2,30.8,8.1,28,8.1z M23.1,25.8H9.9v-8.1 h13.2V25.8z M28,14.5c-0.9,0-1.6-0.7-1.6-1.6s0.7-1.6,1.6-1.6c0.9,0,1.7,0.7,1.7,1.6S29,14.5,28,14.5z M26.4,0H6.6v6.4h19.8V0z'                     />                 </svg>             </button>             <button class='action-btns'>                 <svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 25 30' style='enable-background: new 0 0 25 30;' xml:space='preserve'>                     <path d='M25,10.6h-7.1V0H7.1v10.6H0l12.5,12.4L25,10.6z M0,26.5V30h25v-3.5H0z' />                 </svg>             </button>         </header>";
//middle body
var middleHtml = "<main>             <table style='font-family: 'Poppins', sans-serif; font-weight: 400; width: 100%; border-collapse: collapse; border-spacing: 0; color: #333; line-height: 2;'>                 <tbody>                     <tr style='vertical-align: middle;'>                         <td><img src='../assets/images/logo.png' alt='logo' /></td>                         <td style='text-align: right;'>                             <div style='margin-bottom: 15px;'>                                 <span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>                                     +000 1234 5678 <br />                                     +000 1234 5678                                 </span>                                 <span style='vertical-align: middle; display: inline-block;'><img src='../assets/images/file-telephone.jpg' alt='file-telephone' /></span>                             </div>                             <div style='margin-bottom: 15px;'>                                 <span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>                                     urwebsite.com <br />                                     urwebsite@mail.com                                 </span>                                 <span style='vertical-align: middle; display: inline-block;'><img src='../assets/images/file-mail.jpg' alt='file-mail' /></span>                             </div>                             <div>                                 <span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>                                     Street Address Here <br />                                     Singapore, 222                                 </span>                                 <span style='vertical-align: middle; display: inline-block;'><img src='../assets/images/file-address.jpg' alt='file-address' /></span>                             </div>                         </td>                     </tr>                     <tr>                         <td height='100' colspan='2'></td>                     </tr>                     <tr style='vertical-align: bottom;'>                         <td>                             <div><strong style='color: #f17d2e; font-weight: 600; vertical-align: top;'>James Doe</strong></div>                             <div><small style='vertical-align: top;'>Chief Director</small></div>                             <div><strong style='color: #f17d2e; vertical-align: top;'>A : </strong>45-1, Anson Road Singapore - 8989</div>                             <div><strong style='vertical-align: top;'>W : </strong>email@mailid.com, www.myweb.com</div>                             <div><strong style='color: #f17d2e; vertical-align: top;'>P : </strong>+880 - 12345 -6789</div>                         </td>                         <td style='text-align: right;'>                             Date, 10th September, 2019                         </td>                     </tr>                     <tr>                         <td height='100' colspan='2'></td>                     </tr>                     <tr>                         <td colspan='2'>";


//footer html
var footerHtml = " </td>                     </tr>                     <tr>                         <td height='100' colspan='2'></td>                     </tr>                     <tr>                         <td colspan='2'>                             <div><img src='../assets/images/signature.png' alt='signature' style='vertical-align: top;' /></div>                             <div><strong style='vertical-align: top;'>John Smeeth</strong></div>                             <div><small style='vertical-align: top;'>Manager</small></div>                         </td>                     </tr>                 </tbody>             </table>         </main>   <footer>             <img src='../assets/images/file-footer.png' alt='file-footer' />         </footer>     </body> </html>";

///Letter format show with back button end/////////////////////


// Letter format content for download starts
function printToPDF(Id) {
    $(".loader").fadeIn();
    $.ajax({
        type: "GET",
        url: "/Admin/LetterFormat/LetterFormatDownload/" + Id,
        data: {},
        success: function (result) {
            $(".loader").fadeOut();
            var sampleArr = base64ToArrayBuffer(result.message);
            saveByteArray("Report", sampleArr);

        },
        error: function (result) {
            $(".loader").fadeOut();
            toastr.error('Letter format detail is not available');
            $('#cancel-btn-letter').click();
        }
    });
}
function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}
function saveByteArray(reportName, byte) {
    var blob = new Blob([byte], { type: "application/pdf" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
};
function HideLetterFormat() {

    $('#letterFormatPublish').hide();
    $('main').show();
    $('header').show();
    $('.cstm-card').show(); 
    $('.admin-sidebar').show();   
}
