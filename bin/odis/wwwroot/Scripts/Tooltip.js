
class Tooltip
{
    /**
     * @param cssPath
     * @param htmlToShow
     * @param data {array|null|undefined} location (up, down), isSolid (true, false), hideOnHover (true, false), defaultCss (true, false)
     */
    constructor(cssPath, htmlToShow, data)
    {
        this.showed = false;
        this.cssPath = cssPath;
        this.jQuerySelector = $(cssPath);
        this.htmlToShow = htmlToShow;
        this.uuid = this.privateGenerateUUID();
        this.maxWidth = 0;
        this.maxHeight = 0;

        this.location = "up";
        this.solid = false;
        this.hideOnHover = true;
        this.defaultCss = false;
        if(data!==undefined && data!==null)
        {
            if(data["location"]!==undefined)
            {
                if(data["location"]==="up")
                    this.location="up";
                else if(data["location"]==="down")
                    this.location="down";
            }
            if(data["isSolid"]!==undefined)
            {
                if(data["isSolid"]===true)
                    this.solid = true;
            }
            if(data["hideOnHover"]!==undefined)
            {
                if(data["hideOnHover"]===false)
                    this.hideOnHover = false;
            }
            if(data.defaultCss!==undefined)
            {
                if(data.defaultCss===true)
                    this.defaultCss = true;
            }
        }

        this.onMove = function ()
        {
        };
        this.preShow = function ()
        {
        };
        this.onShow = function ()
        {
        };
        this.onHide = function ()
        {
        };

        this.setup();
    }

    static getDefaultTooltip(cssPath,text,data)
    {
        let html = `
            <div class='popup_default'>
                <label>`+text+`</label>
            </div>
        `;
        new Tooltip(cssPath, html,data).setOnShow(function () {
            $(cssPath).css('cursor','default');
        });
    }

    setup()
    {
        let that = this;

        this.jQuerySelector.click(function (e)
        {
            if (!that.showed)
            {
                that.show(e, 'click');
            }
            return false;
        });
        this.jQuerySelector.mouseenter(function (e)
        {
            if (!that.showed)
            {
                setTimeout(() => {
                    that.show(e, 'hover');
                },1);
            }
        });
    }

    show(e, type)
    {
        if (!this.showed)
        {
            this.preShow(e, type);

            this.showed = true;
            let that = this;
            let body = $('body');
            let doc = $(document);
            this.maxWidth = doc.width();
            this.maxHeight = doc.height();

            let pre_d_css = "";
            let po_d_css = "";
            if(this.defaultCss)
            {
                pre_d_css = "<div class='tooltip_default'>";
                po_d_css = "</div>";
            }
            body.append(`
                <div id='` + this.uuid + `' class="tooltip_main">
                    `+pre_d_css + this.htmlToShow +po_d_css+ `
                </div>
            `);
            body.mousemove(function (e)
            {
                if (that.showed)
                {
                    let target = $(e.target)
                    target = target.closest(that.cssPath + ((!that.hideOnHover)?(", #" + that.uuid):("")));
                    if (target.attr('id') === that.uuid)
                        return;

                    if (!(target).length)
                    {
                        that.jQuerySelector.unbind(e);
                        that.hide();
                        return;
                    }
                }
                if(!that.solid)
                    that.calc(e);

                that.onMove(e);
            });
            that.calc(e);

            this.onShow(e, type);
        }
    }

    hide()
    {
        if(this.showed)
        {
            this.showed = false;
            $('#' + this.uuid).remove();

            this.onHide();
        }
    }

    calc(e)
    {
        if(!this.solid && this.location === "up")
            this.calcMoveCoordsUp(e);
        else if(!this.solid && this.location === "down")
            this.calcMoveCoordsDown(e);
        else if(this.solid && this.location === "up")
            this.calcCoordUp(e);
    }

    calcCoordUp()
    {
        let sel = $('#' + this.uuid);
        let height = sel.height()+5;
        let width = sel.width();
        let pos = this.jQuerySelector.offset();
        let x = pos.left - (width/2) + (this.jQuerySelector.width()/2);
        let y = pos.top - height;

        x = Tooltip.fixWidthX(this.maxWidth, width, x);
        y = Tooltip.fixWidthY(this.maxHeight, height, y);

        sel.css('top', y+'px');
        sel.css('left', x+'px');
    }

    calcMoveCoordsUp(e)
    {
        let sel = $('#' + this.uuid);
        let height = sel.height()+5;
        let width = sel.width();

        let x = e.pageX - (width / 2);
        let y = e.pageY - height - 5;

        x = Tooltip.fixWidthX(this.maxWidth, width, x);
        y = Tooltip.fixWidthY(this.maxHeight, height, y);
        sel.css('top', y+'px');
        sel.css('left', x+'px');
    }

    calcMoveCoordsDown(e)
    {
        let sel = $('#' + this.uuid);
        let height = sel.height()-5;
        let width = sel.width();

        let x = e.pageX - (width / 2);
        let y = e.pageY + 10;

        x = Tooltip.fixWidthX(this.maxWidth, width, x);
        y = Tooltip.fixWidthY(this.maxHeight, height, y);
        sel.css('top', y+'px');
        sel.css('left', x+'px');
    }

    static fixWidthY(maxHeight, height, y)
    {
        if(y<0)
            y=0;
        if((y+height)>maxHeight)
            y = maxHeight - (height);

        return y;
    }

    static fixWidthX(maxWidth, elementWidth, x)
    {
        if (x < (elementWidth/maxWidth))
        {
            x = 0;
        }
        if (x > (maxWidth - (elementWidth)))
        {
            x = (maxWidth-(elementWidth));
        }

        return x;
    }

    setPreShow(functionToDo)
    {
        if (typeof functionToDo === 'function')
            this.preShow = functionToDo;
        else
            this.preShow = function () {}
        return this;
    }

    setOnMove(functionToDo)
    {
        if (typeof functionToDo === 'function')
            this.onMove = functionToDo;
        else
            this.onMove = function () {}
        return this;
    }

    setOnShow(functionToDo)
    {
        if (typeof functionToDo === 'function')
            this.onShow = functionToDo;
        else
            this.onShow = function () {}
        return this;
    }

    setOnHide(functionToDo)
    {
        if (typeof functionToDo === 'function')
            this.onHide = functionToDo;
        else
            this.onHide = function () {}
        return this;
    }

    setHtmlToShow(htmlToShow)
    {
        this.htmlToShow = htmlToShow;
    }

    getUuid()
    {
        return this.uuid;
    }

    /**
     * @returns {boolean}
     */
    getTooltipSolid()
    {
        return this.solid;
    }
    /**
     * @param value {boolean}
     */
    setTooltipSolid(value)
    {
        if(value===true || value===false)
            this.solid = value;
    }
    /**
     * @returns {boolean}
     */
    getHideOnHover()
    {
        return this.hideOnHover;
    }
    /**
     * @param value {boolean}
     */
    setHideOnHover(value)
    {
        if(value===true || value===false)
            this.hideOnHover = value;
    }
    /**
     * @returns {string} up|down
     */
    getTooltipLocation()
    {
        return this.location;
    }
    /**
     * @param value {string} up|down
     */
    setTooltipLocation(value)
    {
        if(value==="up" || value==="down")
            this.location = value;
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