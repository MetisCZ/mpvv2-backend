
jQuery.fn.activateTabs = function() {
    return this.each(function(){
        let tab = $(this)

        let id = tab.attr("id");
        if(id === undefined)
            id = "tab-"+Math.floor(Math.random()*10000000000000000);

        tab.css("visibility","visible");
        tab.addClass("shadow");
        tab.attr("id",id);
        tab.attr("active-tab",0);
        tab.prepend(`
        <div class="tab-tabs shadow">
            <div class="tab-underline"></div>
        </div>
        `);
        let tabs = tab.children(".tab-tabs");
        let i=0;
        tab.children(":not(.tab-tabs)").each(function () {
            let item = $(this);
            if(!item.hasClass("tab-tabs"))
            {
                let title = item.attr("tab-title");
                item.attr("id",id+"-"+i);

                tabs.append(`<div tab-id="`+i+`" onclick="switchTab('`+id+`',`+i+`)">`+title+`</div>`);
                if(i===0)
                    item.addClass("fade-in");
                i++;
            }
        });
        updateUnderlines(0);
    });
};
function updateUnderlines(indexId)
{
    var underlines = document.querySelectorAll(".tab-underline");

    for (var i = 0; i < underlines.length; i++) {
        underlines[i].style.transform = 'translate3d(' + indexId * 100 + 'px,0,0)';
    }
}

function getActiveTabId(tabsId)
{
    return $("#"+tabsId).attr("active-tab");
}

function switchTab(tabsId,indexId)
{
    updateUnderlines(indexId);
    let tabContents = $("#"+tabsId);
    let actContent = $("#"+tabsId+"-"+indexId);
    let contents = tabContents.children(":not(.tab-tabs)");
    tabContents.attr("active-tab",indexId);
    tabContents.children(":not(.tab-tabs) .fade-in").addClass("fade-out");

    setTimeout(()=> {
        contents.removeClass("fade-out");
        contents.removeClass("fade-in");
        actContent.addClass("fade-in");
    },300);
}

jQuery.fn.removeWithFadeOut = function(delay) {
    return this.each(function(){
        let item = $(this)
        //item.addClass("fade-ready");
        item.css("animation","fadeOutToNone "+(delay+50)+"ms ease-in");
        setTimeout(()=> {
            item.remove();
        },delay);
    });
};