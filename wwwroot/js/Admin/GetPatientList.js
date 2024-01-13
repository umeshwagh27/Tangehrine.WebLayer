
//Opening pop up while add 
function OpenAddEditPopup(Id) {       
    $.ajax({
        url: '/Admin/Patient/_AddOrEditPatient?Id=' + Id,
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {           
            $('#divcontent').empty();
            $('#divcontent').html(result);
            $('#AddNewPatientModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Patient");
            }
        },
        error: function (xhr, status) {
        }
    })
}
function SharePatientPopup(Id) {    
    $.ajax({     
        url: "/Admin/Patient/_SharePatient?patientId=" + Id,
        type: "GET",
        dataType: 'html',
        contentType: 'application/html; charset=utf-8',
        success: function (result) {
            $('#divcontent').empty();
            $('#divcontent').html(result);
            $('#ShareRecoerd').modal('show');          
        },
        error: function (result) {
            toastr.error('Doctor detail is not available');
        }
    });
}
//Delete patient record
function DeletePatient(Id) {    
    swal({
        title: "Are you sure you want to delete Patient?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/Patient/DeletePatient',
                data: { patientId: Id },
                type: 'POST',
                success: function (data) {
                    if (data) {
                        toastr.success(
                            '',
                            'Deleted Successfully',
                            {
                                timeOut: 1000,
                                fadeOut: 1000,
                                onHidden: function () {
                                    window.location.reload();
                                }
                            }
                        );
                    }
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}

$(document).on('change', '#ProfileImage', function () {
    readURL(this, "#imgoutput");
});
//Read and check uploaded image validation
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

function printReportAsPDF(Id) {
    $(".loader").fadeIn();
    $.ajax({
        type: "GET",
        url: "/Admin/Patient/PatientReportDownload/" + Id,
        data: {},
        success: function (result) {            
            $(".loader").fadeOut();
            var sampleArr = base64ToArrayBuffer(result.message);
            saveByteArray("PatientReport", sampleArr);

        },
        error: function (result) {
            $(".loader").fadeOut();
            toastr.error('Patient report detail is not available');
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