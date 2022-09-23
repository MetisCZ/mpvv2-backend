function sendForm()
{
    showButtonLoading($("#load-data"));
    removeContentVehicle();
    removeContentDeparts();
    prepareShowTableVeh();
    prepareShowTableDep();
    let count = 0;
    let counted = 0;
    let showType = 0;
    if($('#show_dep').is(":checked"))
    {
        loadDepartsData(() => {
            counted++;
            if(counted === count)
                removeButtonLoading($("#load-data"));
        });
        count++;
        showType = 2;
    }
    if($('#show_veh').is(":checked"))
    {
        loadVehiclesData(() => {
            counted++;
            if(counted === count)
                removeButtonLoading($("#load-data"));
        });
        count++;
        showType = 1;
    }
    if(showType > 0)
    {
        if(getActiveTabId("all-tab")==0)
        {
            switchTab("all-tab",showType);
        }
    }
}
function setWasInDep(line,route,vehicle,date)
{
    $.post("/api/getlinedata/"+line+"/"+route+"/"+vehicle+"/"+date, {})
        .done(showWasInResponse)
        .fail(onError);
}
function setWasInVeh(veh_id)
{
    $.post("/api/getvehicledata/"+veh_id, {})
        .done(showWasInResponse)
        .fail(onError);
}
function showWasInResponse(info)
{
    //console.log(info);
    info = getJson(info);
    if(info.success === undefined)
    {
        new Notification("Neznámá chyba, zkuste to prosím znovu",10000,NotificationType.Error);
    }
    else if(info.success)
    {
        const data = info.data;
        Notification.addConfirmDialog("Opravdu jste byli dne <b>"+data.date+"</b> ve vozidle č. <b>"+data.vehicle+"</b> na lince <b>"+data.line+"</b>/"+data.route+"?",()=> {
            $.post("/api/setwasin/"+data.id_dep+"/"+data.id_veh, {}, function (info) {
                //console.log(info);
                info = getJson(info);
                if(info.success === undefined)
                {
                    new Notification("Neznámá chyba, zkuste to prosím znovu",10000,NotificationType.Error);
                }
                else if(info.success)
                {
                    new Notification(info.message,10000,NotificationType.Success);
                }
                else
                {
                    new Notification(info.message,10000,NotificationType.Error);
                }
            });
        }, ()=> {
            new Notification("Akce zrušena",5000,NotificationType.Info);
        },30000);
    }
    else
    {
        new Notification(info.message,10000,NotificationType.Error);
    }
}

var vehiclesData = {};
var departsData = {};

function loadVehiclesData(onLoad)
{
    let data = $("input, select").serializeArray();
    $("#tblVeh > tr").remove();
    showLoader($("#veh-loader"),"vehLoader");

    $.post("/search/loadvehicles", data)
        .done(function (info) {
            //console.log(info);
            hideLoader("vehLoader");

            let vehs = getJson(info);
            if(vehs === false)
            {
                onLoad();
                vehiclesData = {};
                return;
            }
            else
                vehiclesData = vehs;
            showVehiclesData();
            onLoad();
        })
        .fail(onError);
}
function showVehiclesData(data=null)
{
    if(data === null)
        data = vehiclesData;

    if($(document).width() < 1050)
    {   // Mobile version

        $("#divTblVeh").append(`
            <div id="tblVeh">
                
            </div>
        `);
        data.forEach((veh)=> {
            let color = "#000";
            switch(parseInt(veh.last_seen_type)){
                case 0: color = "#444"; break;
                case 1: color = "#080"; break;
                case 2: color = "#070"; break;
                case 3: color = "#050"; break;
                case 4: color = "#550"; break;
                case 5: color = "#506"; break;
                case 6: color = "#500"; break;
                case 7: color = "#555"; break;
                case 8: color = "#333"; break;
            }
            let lastSeen = "<div class='lass font-smaller' style='background-color: "+color+"; width: 140px; text-align: center'>"+veh.last_seen_date+"<br>"+veh.last_seen+"</div>";
            let year = "";
            if(veh.manufactured_date != "")
                year = "vyroben: "+veh.manufactured_date;
            let wasIn = "Nikdy jsi tímto vozem nejel";
            if(veh.was_in_count > 0)
            {
                wasIn = "Tímto vozem jsi jel "+veh.was_in_count+"x, naposledy "+veh.was_in_last;
            }

            $("#tblVeh").append(`
                <div class="veh-item">
                    <div class="veh-title veh-split">
                        <div>
                            <div class="veh-id">
                                <label>`+veh.reg_num+`</label>
                            </div>
                        </div>
                        <div>
                            <div class="veh-inline">
                                <div class="icon icon-home"></div>
                                <label>`+veh.carrier_name+`</label>
                            </div>
                            <label class="font-smaller">`+year+`</label>
                        </div>
                    </div>
                    <div class="veh-type">
                        <div class="veh-inline">
                            <div class="icon icon-bus"></div>
                            <label class="font-bold">`+veh.manufacturer_name+`</label>
                            <label class="font-smaller font-darker">`+veh.vehicle_type+`</label>
                        </div>
                    </div>
                    <div class="veh-bottom veh-split">
                        <div>
                            <div class="veh-inline">
                                <div class="icon icon-snow"></div>
                                <label class="font-smaller">`+veh.air_condition+`</label>
                            </div>
                            <div class="veh-inline">
                                <div class="icon"></div>
                                <label>výjezdů: `+veh.count+`</label> 
                            </div>      
                        </div>
                        <div>
                            `+lastSeen+`
                        </div>
                    </div>
                    <div class="veh-wasin font-smaller">
                        <label>`+wasIn+`</label>
                    </div>
                </div>
            `);

            let tooltip = getVehicleTooltip(veh.id);
            $("#tblVeh").children(":last-child").click(function () {
                tooltip.show();
            });

        });
    }
    else
    {   // Desktop version
        $("#divTblVeh").append(`
        <table id="tblVeh" class="tbl">
            <thead>
            <tr>
                <td>Ev. č.</td>
                <td>Dopravce</td>
                <td>Rok výroby</td>
                <td>Výrobce & Typ</td>
                <td>Klima</td>
                <td>Posl. výjezd</td>
                <td>Výjezdů</td>
                <td>Was in</td>
            </tr>
            </thead>
        </table>
        `);

        data.forEach((veh)=> {
            let color = "#000";
            switch(parseInt(veh.last_seen_type)){
                case 0: color = "#444"; break;
                case 1: color = "#080"; break;
                case 2: color = "#070"; break;
                case 3: color = "#050"; break;
                case 4: color = "#550"; break;
                case 5: color = "#506"; break;
                case 6: color = "#500"; break;
                case 7: color = "#555"; break;
                case 8: color = "#333"; break;
            }

            $("#tblVeh").append("<tr><td>"+veh.reg_num+"</td><td>"+veh.carrier_name+"</td><td>"+veh.manufactured_date+"</td><td>"+veh.manufacturer_name+"<br>"+veh.vehicle_type+"</td><td>"+veh.air_condition+"</td><td><div class='lass' style='background-color: "+color+";'>"+veh.last_seen_date+"<br>"+veh.last_seen+"</div></td><td>"+veh.count+"</td><td>"+veh.was_in_last+"<br>"+veh.was_in_count+"x</td></tr>");

            let tooltip = getVehicleTooltip(veh.id);
            $("#tblVeh").children(":last-child").click(function () {
                tooltip.show();
            });
        });
    }
}
function getVehicleTooltip(vehicleId, line=null, route=null, date=null)
{
    return new Popup(`
                <div class="popup_default">
                    <script>
                        $.post("/vehicle/view/`+vehicleId+`?mini=true",{},(data) => {
                            $(".popup_default").prepend(data);
                        });
                    </script>
                    <div style="margin: 10px;">
                        <button class="btn" style="width: 50%; min-width: 140px;" onclick="`+((line==null)?("setWasInVeh('"+vehicleId+"')"):("setWasInDep('"+line+"','"+route+"','"+vehicleId+"','"+date+"')"))+`">Set was in</button>
                    </div>
                </div>
            `);
}
function loadDepartsData(onLoad)
{
    let data = $("input, select").serializeArray();
    $("#tblDep > tr").remove();
    showLoader($("#dep-loader"),"depLoader");

    $.post("/search/loaddeparts", data)
        .done(function (info) {
            //console.log(info);
            hideLoader("depLoader");
            let deps = getJson(info);
            if(deps === false)
            {
                onLoad();
                departsData = {};
                return;
            }
            else
                departsData = deps;
            showDepartsData();

            onLoad();
        })
        .fail(onError);
}
function showDepartsData(data=null)
{
    if(data === null)
        data = departsData;
    
    $("#divTblDep").append(`
        <div id="tblDep">
            
        </div>
    `);

    let documentWidth = $(document).width();
    data.forEach((dep)=> {
        let year = "";
        if(dep.v_manuf_year != "")
            year = "vyroben: "+dep.v_manuf_year;
        let wasIn = "Tímto spojem jsi nejel";
        if(dep.was_in == true)
            wasIn = "Tímto spojem jsi jel";
        let s_date = dep.s_date.replace(/ /g, '&nbsp;');
        let a_date = dep.a_date.replace(/ /g, '&nbsp;');
        
        let vehStr = `<div class="dep-veh-id-bg" `+((documentWidth<=640)?``:`style="min-width: 120px;"`)+`><div class=\"dep-veh-id\"><label>`+dep.reg_number+`</label></div>`;
        dep.more_vehs.forEach((se)=> {
            vehStr += ` + <div class=\"dep-veh-id\"><label>`+se.reg_num+`</label></div>`;
        });
        vehStr+="</div>";
        let ac = "";
        if(dep.air_condition==="celovozová")
            ac = "<div class=\"icon icon-snow\"></div>";

        if(documentWidth <= 640) 
        {   // Mobile version
            $("#tblDep").append(`
                <div class="veh-item">
                    <div class="veh-title veh-split">
                        <div class="veh-inline">
                            <div class="dep-id">
                                <label>`+dep.line+`</label>
                            </div>
                            <label>/ `+dep.route+`</label>
                        </div>
                        <div>
                            `+vehStr+`
                        </div>
                    </div>
                    <div class="veh-type">
                        <div class="veh-inline">
                            <div class="icon icon-bus"></div>
                            <label class="font-bold">`+dep.v_manuf+`</label>
                            <label class="font-smaller font-darker">`+dep.v_type+`</label>
                        </div>
                    </div>
                    <div>
                        <div class="veh-inline">
                            <div class="icon icon-snow"></div>
                            <label class="font-smaller">`+dep.air_condition+`</label>
                        </div>
                    </div>
                    <div>
                        <div class="veh-inline">
                            <div class="icon icon-delay"></div>
                            <label>`+dep.delay+` mins</label>
                        </div>
                    </div>
                    <div>
                        <div class="veh-inline dep-stats">
                            <div class="icon icon-start"></div>
                            <div class="veh-split">
                                <label>`+dep.start_station+`</label>
                                <label class="font-smaller">`+s_date+`</label>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div class="veh-inline dep-stats">
                            <div class="icon icon-now"></div>
                            <div class="veh-split">
                                <label>`+dep.last_station+`</label>
                                <label class="font-smaller">`+a_date+`</label>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div class="veh-inline dep-stats">
                            <div class="icon icon-finish"></div>
                            <div class="veh-split">
                                <label>`+dep.final_station+`</label>
                            </div>
                        </div>
                    </div>
                    <div class="veh-wasin font-smaller">
                        <label>`+wasIn+`</label>
                    </div>
                </div>
            `);
        }
        else
        {
            $("#tblDep").append(`
            <div class="dep-item">
                <div class="dep-linestops">   
                    <div class="dep-line">
                        <div class="dep-id">
                            <label>`+dep.line+`</label>
                        </div>
                        <label>/ `+dep.route+`</label>
                    </div>
                    <div class="dep-stops">
                        <div>
                            <div class="icon icon-start"></div>
                            <div class="dep-stop">
                                <label>`+dep.start_station+`</label>
                                <label class="font-smaller">`+s_date+`</label>
                            </div>
                        </div>
                        <div>
                            <div class="icon icon-right-arrow"></div>
                            <div class="dep-stop">
                                <label>`+dep.final_station+`</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="dep-veh">
                    <div>
                        <div>
                        `+vehStr+`
                        </div>
                        <div class="dep-veh-type">
                            <label class="font-bold">`+dep.v_manuf+`</label>
                            <label class="font-smaller font-darker">`+dep.v_type+`</label>
                        </div>
                        <div class="dep-info">
                            `+ac+`
                        </div>
                    </div>
                    <div>
                        <div class="veh-inline">
                            <div class="icon icon-delay"></div>
                            <label class="font-smaller">`+dep.delay+` mins</label>
                        </div>
                        <div class="veh-inline">
                            <div class="icon icon-now"></div>
                            <div class="dep-stop">
                                <label class="font-bold">`+dep.last_station+`</label>
                                <label class="font-smaller">`+a_date+`</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `);
        }

        let tooltip = getVehicleTooltip(dep.id, dep.line, dep.route,dep.date);
        $("#tblDep").children(":last-child").click(function () {
            tooltip.show();
        });
    });
}
function prepareShowTableVeh()
{
    const table = $("#divTblVeh");
    table.html(`
        <div id="veh-loader"></div>
    `);
}

function removeContent(divId)
{
    $("#"+divId).remove();
}
function removeContentVehicle()
{
    $("#divTblVeh").children().remove();
}
function removeContentDeparts()
{
    $("#divTblDep").children().remove();
}

function prepareShowTableDep()
{
    const table = $("#divTblDep");
    table.html(`
        <div id="dep-loader"></div>
    `);
}
$(document).ready(()=> {
    Tooltip.getDefaultTooltip("#query-settings-help","Klepnutím na šipku vpravo zobrazíte filt dat",{isSolid:true});
})