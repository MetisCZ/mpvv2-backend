let notificationsTimer = null;

(function() {
    var ev = new $.Event('remove'),
        orig = $.fn.remove;
    $.fn.remove = function() {
        $(this).trigger(ev);
        return orig.apply(this, arguments);
    }
})();

const NotificationType = {
    Success: 0,
    Error: 1,
    Warning: 2,
    Normal: 3,
    Info: 4,
}


class Notification
{
    /**
     *
     * @param notificationType type of notification from NotificationType object or null for empty notification
     * @param text
     * @param time Time till hide in milliseconds (1000ms=1s), leave blank text if you want it to stay till page unload
     * @param color rgba color or null, recommended opacity .3
     */
    constructor(text,time,notificationType=null,color=null)
    {
        if(notificationType===null)
        {
            this.uid = this.privateAddEmptyDownRightNotification(text,time,null,color);
        }
        else
            this.uid = this.privateAddDownRightNotification(notificationType,text,time,color);
    }

    /**
     *
     * @param text
     * @param fnOnConfirm
     * @param fnOnCancel
     * @param time
     * @returns {Notification}
     */
    static addConfirmDialog(text,fnOnConfirm,fnOnCancel,time=-1,color=null)
    {
        // `
        let div = `
            <div class="notif_conf_dialog">
                <div><label>` + text + `</label></div>
                <div>
                    <button class="btn btn-green notif-btns notif_confirm">Yes</button>
                    <button class="btn btn-red notif-btns notif_cancel">Cancel</button>
                </div>
            </div>
        `;

        let notif = new Notification(div,time,null,color);
        let uid = notif.getUid();

        $(document).ready(()=>{
            let confirmed = false;
            $("#"+uid).bind('remove', function () {
                if(!confirmed)
                    fnOnCancel();
            })
            $("#"+uid+" .notif_confirm").click(()=> {
                confirmed = true;
                fnOnConfirm();
                notif.removeNotification();
            });
            $("#"+uid+" .notif_cancel").click(()=> {
                notif.removeNotification();
            });
        });
        return notif;
    }

    privateAddDownRightNotification(notificationType, text, time,color=null)
    {
        let uid = this.privateGenerateUid();
        this.privateAddDownRightNotificationWithUid(notificationType,text,time,uid,color);
        return uid;
    }
    privateAddEmptyDownRightNotification(text, time, uid=null,color=null)
    {
        if(uid === null)
            uid = this.privateGenerateUid();
        $(document).ready(function () {

            if(notificationsTimer===null)
                notificationsTimer = setInterval(Notification.privateUpdateNotifications, 10);

            let style = "";
            if(color != null)
            {
                style = "background-color: "+color;
            }

            let div =
                "<div id='"+uid+"' class='notif shadow' style='"+style+"'>                                                                                              "+
                "   <div class='notif_content' basetime='"+ time +"' acttime='"+ time +"'>                                                              "+
                text                                                                                                                                    +
                "   </div>                                                                                                                              "+
                "   <div class='notif_timer'>                                                                                                           "+
                "       <div class='notif_timer_slider'></div>                                                                                          "+
                "   </div>                                                                                                                              "+
                "   <div class='notif_close'></div>"
            "</div>                                                                                                                                 ";

            $('#notif_main_div').append(div);

            $( "#"+uid+" .notif_close" ).click(function() {
                $( "#"+uid ).remove();
            });
        });
        return uid;
    }
    privateAddDownRightNotificationWithUid(notificationType, text, time, uid,color=null)
    {
        let div =   "<div class='notif_icon_div'><div style='background-image: url(\"/public/images/notification/"+ this.privateGetMessageFromNotificationID(notificationType) +".svg\");'></div></div> "+
            "<label>"+ text +"</label>";
        if(color==null)
            color = this.privateGetColorFromNotificationID(notificationType);
        return this.privateAddEmptyDownRightNotification(div,time,uid,color);
    }
    privateGetMessageFromNotificationID(id)
    {
        switch (id)
        {
            case 0:
                return "success";
            case 1:
                return "error";
            case 2:
                return "warning"
            case 4:
                return "info";
            default:
                return "normal";
        }
    }
    privateGetColorFromNotificationID(id)
    {
        switch (id)
        {
            case 0:
                return "rgba(0,180,0,.3)";
            case 1:
                return "rgba(180,0,0,.3)";
            case 2:
                return "rgba(170,170,0,.3)";
            case 4:
                return "rgba(0,0,180,.3)";
            default:
                return "rgba(0,0,0,.3)";
        }
    }
    removeNotification()
    {
        this.getJQuerySelector().remove();
    }
    getJQuerySelector()
    {
        return $("#"+this.uid);
    }
    getUid()
    {
        return this.uid;
    }
    privateGenerateUid()
    {
        return "notif-"+this.privateGenerateUUID();
    }
    static privateGetPercent(from, value)
    {
        return (value/from)*100;
    }
    static privateUpdateNotifications()
    {
        // amount counts amount of timeout notifications (not all)
        let amount=0;

        $("#notif_main_div > div").each(function () {
            let time = $(this).children('.notif_content').attr('basetime');

            if(time!=="" && time>=0)
            {
                let actTime = $(this).children('.notif_content').attr('acttime');
                let percent = Notification.privateGetPercent(time,actTime);

                if(actTime-10<0)
                    $(this).remove();

                $(this).children('.notif_timer').children().css('width',percent+'%');
                $(this).children('.notif_content').attr('acttime',actTime-10);
                amount++;
            }
        });

        if(amount===0)
        {
            clearInterval(notificationsTimer);
            notificationsTimer=null;
        }
    }
    privateGenerateUUID() { // Public Domain/MIT
        let d = new Date().getTime();//Timestamp
        let d2 = (performance && performance.now && (performance.now()*1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            let r = Math.random() * 16;//random number between 0 and 16
            if(d > 0){//Use timestamp until depleted
                r = (d + r)%16 | 0;
                d = Math.floor(d/16);
            } else {//Use microseconds since page-load if supported
                r = (d2 + r)%16 | 0;
                d2 = Math.floor(d2/16);
            }
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
    }
}
