
class Popup
{
    constructor(htmlToShow)
    {
        this.showed = false;
        this.htmlToShow = htmlToShow;
        this.uuid = this.privateGenerateUUID();
        this.onShow = function () {};
        this.onHide = function () {};
    }

    isShowed()
    {
        return this.showed;
    }

    getUuid()
    {
        return this.uuid;
    }

    getHtmlToShow()
    {
        return this.htmlToShow;
    }

    setHtmlToShow(htmlToShow)
    {
        this.htmlToShow = htmlToShow;
    }

    show()
    {
        if(!this.showed)
        {
            let that = this;
            this.showed = true;

            let html = `
                <div id="` + this.uuid + `" style="width: 100%; height: 100%; position: absolute; top: 0; left: 0;">
                    <div class="popup_blur"></div>
                    <div class="popup_cover">
                        <div class="popup_content">
                            <div class="popup_close"></div>
                            ` + this.htmlToShow + `
                        </div>
                    </div>
                </div>
            `;
            let body = $('body');

            body.append(html);
            body.css('overflow', 'hidden');
            $('#'+that.uuid+' .popup_close').click(() => {
                that.hide();
            });

            let blurrer = $('#'+that.uuid+' .popup_blur');

            setTimeout(function ()
            {
                blurrer.addClass('show');
                $('#'+that.uuid+' .popup_cover').addClass('show');
            }, .3);

            blurrer.click(function ()
            {
                that.hide();
            }).children().click(function ()
            {
                return false;
            });

            this.onShow();
        }
    }

    hide()
    {
        if(this.showed)
        {
            this.showed = false;
            let that = this;
            let sel = $('#'+this.uuid+' .popup_cover');
            let blur = $('#'+this.uuid+' .popup_blur');
            blur.unbind('click');
            blur.removeClass('show');
            sel.removeClass('show');
            setTimeout(function ()
            {
                $('#'+that.uuid).remove();
                $('body').css('overflow', 'auto');
            }, 200);

            this.onHide();
        }
    }

    setOnShow(functionToDo) {
        if (typeof functionToDo === 'function')
        {
            this.onShow = functionToDo;
            return true;
        }
        else
        {
            this.onShow = function () {}
            return false;
        }
    }

    setOnHide(functionToDo) {
        if (typeof functionToDo === 'function')
        {
            this.onHide = functionToDo;
            return true;
        }
        else
        {
            this.onHide = function () {}
            return false;
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