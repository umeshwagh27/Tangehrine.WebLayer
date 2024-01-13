Dropzone.autoDiscover = false;
$(document).ready(function () {
    $("#summernote").summernote({
        height: 330,
    });
    $(".repeater").repeater({
        isFirstItemUndeletable: true,
    });

    var buttonPlus = $(".qty-bttn-plus");
    var buttonMinus = $(".qty-bttn-minus");
    var incrementPlus = buttonPlus.click(function () {
        var $n = $(this).parent().find(".qty");
        $n.val(Number($n.val()) + 1);
    });
    var incrementMinus = buttonMinus.click(function () {
        var $n = $(this).parent().find(".qty");
        var amount = Number($n.val());
        if (amount > 0) {
            $n.val(amount - 1);
        }
    });
    // Dropzone has been added as a global variable.
    $(".my-dropzone").dropzone({ url: "/file/post" });
});

function OpenAddEditVisitPopup(Id, patientId) {
    $.ajax({
        url: '/Admin/VisitProfile/AddOrEditVisit',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);
            $('#AddVisitModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Visit");
            }
        },    
    })
}
//Delete patient visit record
function DeletePatientVisit(Id) {
    swal({
        title: "Are you sure you want to delete Visit?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/VisitProfile/DeleteVisit',
                data: { Id: Id },
                type: 'POST',
                success: function (data) {
                    if (data) {
                        toastr.success(
                            '',
                            'Deleted Successfully',
                            {
                                timeOut: 2000,
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
//Opening pop up while adding previsit note
function OpenAddEditPreVisitNoteModel(Id, patientId) {    
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrEditPreVisitNote',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {            
            $('#divPreVisitNoteModel').empty();
            $('#divPreVisitNoteModel').html(result);
            $('#AddNewPreVisitNoteModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Pre-Visit Note");
            }
        }
      
    })
}
//Delete pre visit note record
function DeletePreVisitNote(Id) {
    swal({
        title: "Are you sure you want to delete Visit?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/VisitProfile/DeletePreVisitNote',
                data: { Id: Id },
                type: 'POST',
                success: function () {               
                    toastr.success('Deleted Successfully');
                    ViewPriVisitNotes();
                },
                error: function () {
                    toastr.error('Pre-Visit  Not Deleted');
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
//Opening pop up while add
function OpenAddEditOfIME(Id, patientId) {
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrUpdateIMEs',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);
            $('#AddNewIMEsModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit IME");
            }
        },
        error: function (xhr, status) {

        }
    })
}
//Delete IMEs record
function DeleteIMEs(Id) {
    swal({
        title: "Are you sure you want to delete IMEs?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/VisitProfile/DeleteIMEs',
                data: { Id: Id },
                type: 'POST',
                success: function () {                  
                        toastr.success('Deleted Successfully')                      
                        ViewIme();
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
//Opening pop up while add
function OpenAddEditOfPatientLabReport(Id, patientId) {
    
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrUpdatePatientLabReport',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);
            $('#AddLabReportModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Lab Record");
            }
        },
        error: function (xhr, status) {

        }
    })
}
//Delete Lab report record of patient
function DeleteLabReportOfPatient(Id) {
    swal({
        title: "Are you sure you want to delete Lab Report?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/VisitProfile/DeletePatientLabReport',
                data: { Id: Id },
                type: 'POST',
                success: function() {             
                    toastr.success('Deleted Successfully');
                    ViewLabReport();
                },
                error: function () {
                    toastr.error('Lab Repot Not Deleted');
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
//Opening pop up while add
function OpenAddEditVital(Id, patientId) {
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrUpdateVital',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);
            $('#AddVitalsModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Vital");
            }
        },
     
    })
}
//Delete Vital record of patient
function DeleteVitalOfPatient(Id) {
    swal({
        title: "Are you sure  to delete Vital?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/VisitProfile/DeleteVital',
                data: { Id: Id },
                type: 'POST',
                success: function () {                
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
                },
                error: function () {
                    toastr.error('Vital Not Deleted');
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}
//For downloading pdf and images
function PrintToPdf(Id) {
    $(".loader").fadeIn();
    $.ajax({
        type: "GET",
        url: "/Admin/VisitProfile/DownloadPdf/" + Id,
        data: {},
        success: function (result) {
            $(".loader").fadeOut();
            var sampleArr = base64ToArrayBuffer(result.base64String);
            saveByteArray(result.fileExtenstion, sampleArr);

        },
        error: function (result) {
            $(".loader").fadeOut();
            toastr.error('Lab report does not exist');
        }
    });

}

function DownloadPrevisitNote(Id) {    
    $(".loader").fadeIn();
    $.ajax({
        type: "GET",
        url: "/Admin/VisitProfile/DownloadPrevisitNote/" + Id,
        data: {},
        success: function (result) {
            $(".loader").fadeOut();
            var sampleArr = base64ToArrayBuffer(result.base64String);
            saveByteArray(result.fileExtenstion, sampleArr);
        },
        error: function (result) {
            $(".loader").fadeOut();
            toastr.error('File does not exist');
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
//Opening pop up while add
function OpenAddEditMedicalState(id, patientId, Date) {    
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrUpdatePatientMedicine',
        data: {
            "Date": Date,
            "Id": id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {            
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);     
            $('#AddMedicalStateModal').modal('show');          
        },
        error: function (xhr, status) {

        }
    })
}

function GetChartData(patientId) {  
    $.ajax({
        url: "/Admin/VisitProfile/GetDataForStatistics?PatientId=" + patientId,
        type: "GET",
        success: function (response) {
            
            am4core.useTheme(am4themes_animated);
            var chart = am4core.create("chartdiv", am4charts.XYChart);            
            chart.dateFormatter.dateFormat = "MM/dd/yy";
            var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
                dateAxis.renderer.grid.template.location = 0;
                dateAxis.renderer.minGridDistance = 65;
                dateAxis.renderer.labels.template.location = 0.0001;
                dateAxis.periodChangeDateFormats.setKey("date", "MM/dd/yy");
                dateAxis.periodChangeDateFormats.setKey("month", "MM/dd/yy");
                dateAxis.periodChangeDateFormats.setKey("year", "MM/dd/yy");
                dateAxis.dateFormats.setKey("date", "MM/dd/yy");
                dateAxis.dateFormats.setKey("month", "MM/dd/yy");
                dateAxis.dateFormats.setKey("year", "MM/dd/yy");
                
           

            var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
           
            for (let i = 0; i < response[0].patientMedicineCharts.length; i++)
            {
                createSeries(response[0].patientMedicineCharts[i]);  
            }          

            function createSeries(data) {
                var series = chart.series.push(new am4charts.LineSeries());
                series.dataFields.valueY = "dose";
                series.dataFields.dateX = "medicineDate";
                series.name = data.medicineName;
                series.tooltipText = data.medicineName;
                series.strokeWidth = 2;
                series.data = data.medicineDateDoses;
                var bullet = series.bullets.push(new am4charts.CircleBullet());
                bullet.circle.stroke = am4core.color("#fff");
                bullet.circle.strokeWidth = 2;
              
            }
            chart.responsive.enabled = true;
            chart.legend = new am4charts.Legend();
            chart.cursor = new am4charts.XYCursor();
            chart.legend.position = "right";
            chart.legend.maxHeight = 150;
            chart.legend.scrollable = true;
            chart.events.on("ready", function (ev) {
                valueAxis.min = valueAxis.minZoomed;
                valueAxis.max = valueAxis.maxZoomed;
            });
            var shapelenght = document.querySelectorAll("g[shape-rendering='auto']");
          
            if (shapelenght.length != undefined || shapelenght > 0) {                
                shapelenght[0].remove();
                shapelenght[1].remove();
            }
            //// Use theme
            //am4core.useTheme(am4themes_animated);

            //// Create chart instance
            //var chart = am4core.create("chartdiv", am4charts.XYChart);
            //chart.dateFormatter.dateFormat = "MM/dd/yyyy";
            //chart.dragGrip.position = "left";
            //chart.dragGrip.height = am4core.percent(60);
            //// Create axes
            //var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
            //dateAxis.renderer.minGridDistance = 50;
            //dateAxis.renderer.grid.template.location = 0.5;
            //dateAxis.startLocation = 0.5;
            //dateAxis.endLocation = 0.5;

            //// Create value axis
            //var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

            //// Create series
            //var series1 = chart.series.push(new am4charts.LineSeries());
            //series1.dataFields.valueY = "dose";
            //series1.dataFields.dateX = "medicineDate";
            //series1.strokeWidth = 2;
            //series1.tensionX = 0.8;
            //series1.bullets.push(new am4charts.CircleBullet());
            //series1.data = ;

            //var series2 = chart.series.push(new am4charts.LineSeries());
            //series2.dataFields.valueY = "dose";
            //series2.dataFields.dateX = "medicineDate";
            //series2.strokeWidth = 2;
            //series2.tensionX = 0.8;
            //series2.bullets.push(new am4charts.CircleBullet());
            //series2.data = [{
            //    "medicineDate": new Date(2018, 3, 20),
            //    "dose": 90
            //}, {
            //    "medicineDate": new Date(2018, 3, 23),
            //    "dose": 125
            //}, {
            //    "medicineDate": new Date(2018, 3, 26),
            //    "dose": 77
            //}, {
            //    "medicineDate": new Date(2018, 3, 28),
            //    "dose": 113
            //}];

            //var series2 = chart.series.push(new am4charts.LineSeries());
            //series2.dataFields.valueY = "dose";
            //series2.dataFields.dateX = "medicineDate";
            //series2.strokeWidth = 3;
            //series2.tensionX = 0.8;
            //series2.bullets.push(new am4charts.CircleBullet());
            //series2.data = [{
            //    "date": new Date(2018, 3, 22),
            //    "value1": 101
            //}, {
            //    "date": new Date(2018, 3, 23),
            //    "value1": 79
            //}, {
            //    "date": new Date(2018, 3, 24),
            //    "value1": 90
            //}, {
            //    "date": new Date(2018, 3, 25),
            //    "value1": 60
            //}, {
            //    "date": new Date(2018, 3, 26),
            //    "value1": 115
            //}];
            /*Chart code*/
            //am5.ready(function () {
            //    // Create root element
            //    // https://www.amcharts.com/docs/v5/getting-started/#Root_element
            //    var root = am5.Root.new("chartdiv");

            //    // Set themes
            //    // https://www.amcharts.com/docs/v5/concepts/themes/
            //    root.setThemes([am5themes_Animated.new(root)]);

            //    // Create chart
            //    // https://www.amcharts.com/docs/v5/charts/xy-chart/
            //    var chart = root.container.children.push(
            //        am5xy.XYChart.new(root, {
            //            panX: true,
            //            panY: true,
            //            wheelX: "panX",
            //            wheelY: "zoomX",
            //            layout: root.verticalLayout,
            //        })
            //    );

            //    // Add cursor
            //    // https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
            //    var cursor = chart.set(
            //        "cursor",
            //        am5xy.XYCursor.new(root, {
            //            behavior: "none",
            //        })
            //    );
            //    cursor.lineY.set("visible", false);
            //    var colorSet = am5.ColorSet.new(root, {});            
            //    data = response;        
            //    // Create axes
            //    // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
            //    var xRenderer = am5xy.AxisRendererX.new(root, {});
            //    xRenderer.grid.template.set("location", 0.5);
            //    xRenderer.labels.template.setAll({ location: 0.5, multiLocation: 0.5 });

            //    var xAxis = chart.xAxes.push(
            //        am5xy.DateAxis.new(root, {
            //            maxDeviation: 35.0,
            //            baseInterval: { timeUnit: "day", count: 1 },
            //            renderer: xRenderer,
            //            tooltip: am5.Tooltip.new(root, {}),
            //        })
            //    );
            //    var yRenderer = am5xy.AxisRendererY.new(root, {});
            //    yRenderer.grid.template.set("forceHidden", true);
            //    yRenderer.labels.template.set("minPosition", 0.05);

            //    var yAxis = chart.yAxes.push(
            //        am5xy.ValueAxis.new(root, {
            //            //maxPrecision: 0,
            //            //extraMin: 0.1,
            //            min: 0,
            //            max: 1000,
            //            renderer: yRenderer,
            //        })
            //    );
            //    var series = chart.series.push(
            //        am5xy.LineSeries.new(root, {
            //            xAxis: xAxis,
            //            yAxis: yAxis,
            //            valueYField: "dose",
            //            valueXField: "medicineDate",
                     
            //            maskBullets: false,
            //            tooltip: am5.Tooltip.new(root, {
            //                pointerOrientation: "vertical",
            //                dy: -20,
            //                labelText: "{medicineName}",                           
            //            }),
            //        })
            //    );
            //    // Set up data processor to parse string dates
            //    // https://www.amcharts.com/docs/v5/concepts/data/#Pre_processing_data
            //    series.data.processor = am5.DataProcessor.new(root, {
            //        dateFormat: "dd/MM/yyyy",
            //        dateFields: ["medicineDate"],
            //    });
            //    series.strokes.template.setAll({ strokeDasharray: [3, 3], strokeWidth: 2 });
            //    var i = -1;
            //    series.bullets.push(function () {
            //        i++;
            //        if (i > 7) {
            //            i = 0;
            //        }
            //        var container = am5.Container.new(root, {
            //            centerX: am5.p50,
            //            centerY: am5.p50,
            //        });
            //        container.children.push(am5.Circle.new(root, { radius:10, fill: series.get("fill") }));
            //        container.children.push(
            //            am5.Picture.new(root, {
            //                centerX: am5.p50,
            //                centerY: am5.p50,
            //                width: 23,
            //                height: 23,
            //                src: "",
            //            })
            //        );
            //        return am5.Bullet.new(root, {
            //            sprite: container,
            //        });
            //    });
            //    series.data.setAll(data);
            //    series.appear(1000);
            //    // Make stuff animate on load
            //    // https://www.amcharts.com/docs/v5/concepts/animations/
            //    chart.appear(1000, 100);
            //});
            // end am5.ready()
        }
    });
 
}
function OpenAddEditPreStatementTrendsModel(Id, patientId) {
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrEditPatientStatementTrends',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $('#divPreStatementTrendsModel').empty();
            $('#divPreStatementTrendsModel').html(result);
            $('#AddNewPreStatementTrendsModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Statement");
            }
        },
        error: function (xhr, status) {

        }
    })
}

//Opening pop up while add
function OpenAddEditCheckupRecords(id, patientId, Date) {   
   
    $.ajax({
        url: '/Admin/VisitProfile/_AddOrUpdatePatientCheckupRecord',
        data: {
            "Date": Date,
            "Id": id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);
            $('#AddCheckupRecordsModal').modal('show');
           
        },
        error: function (xhr, status) {            
        }
    })
}

//Opening pop up while add
function OpenAddEditOfPatientNotes(Id, patientId) {

    $.ajax({
        url: '/Admin/VisitProfile/_AddOrUpdatePatientNotes',
        data: {
            "Id": Id,
            "PatientId": patientId
        },
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        success: function (result) {
            $('#divVisitContent').empty();
            $('#divVisitContent').html(result);
            $('#AddNoteModal').modal('show');
            if (Id > 0) {
                $("#ModalTitle").html("Edit Note");
            }
        },
        error: function (xhr, status) {

        }
    })
}

//Delete notes record of patient
function DeleteNotesOfPatient(Id) {
    swal({
        title: "Are you sure you want to delete Note?",
        text: "",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: '/Admin/VisitProfile/DeletePatientNote',
                data: { Id: Id },
                type: 'POST',
                success: function () {
                    toastr.success('Note deleted successfully');
                    ViewNotes();
                },
                error: function () {
                    toastr.error('Note not deleted');
                }
            });
        }
        else {
            /*swal("Saved");*/
        }
    });
}

//Download notes record of patient
function DownloadNote(Id) {
    $(".loader").fadeIn();
    $.ajax({
        type: "GET",
        url: "/Admin/VisitProfile/DownloadNotes/" + Id,
        data: {},
        success: function (result) {
            $(".loader").fadeOut();
            var sampleArr = base64ToArrayBuffer(result.base64String);
            saveByteArray(result.fileExtenstion, sampleArr);
        },
        error: function (result) {
            $(".loader").fadeOut();
            toastr.error('File does not exist');
        }
    });
}