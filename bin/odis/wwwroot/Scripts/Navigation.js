
var navigationShowed = false;

const NavBar = {
    Home: 0,
    All: 1
};
const NavBarData = [
    {
        url: "/",
        name: "Home",
        showInNav: true
    },
    {
        url: "/search",
        name: "All",
        showInNav: true
    }
]

function navigation(index) {
    var underlines = document.querySelectorAll(".nav-underline");

    for (var i = 0; i < underlines.length; i++) {
        underlines[i].style.transform = 'translate3d(' + index * 100 + '%,0,0)';
    }
}

window.onpopstate = function(e){
    let path = location.pathname;

    let i=0;
    let barId = null;
    NavBarData.forEach((item) => {
        if(path == item.url)
            barId = i;
        i++;
    })

    showSiteUrl(location.href,barId,false);
};

function showNavigation()
{
    if(!navigationShowed)
    {
        let sitesInNav = "";
        let i=0;
        NavBarData.forEach((site)=> {
            if(site.showInNav)
                sitesInNav += '<a onClick="showSite('+i+')">'+site.name+'</a>';
            i++;
        })
        $("nav").html(`
            <div class="nav-main">
                <div class="nav-underline"></div>
                <div class="nav-underline"></div>
                <div class="nav-underline"></div>
                `+sitesInNav+`
            </div>
            <div class="center-content">
                <button id="reload-button" class="btn btn-yellow" onclick="reload()">Reload data</button>
            </div>
        `);
        //$("nav").addClass("fade-in");
        //navigation(navBarActive);
        navigationShowed = true;
    }
}
function hideNavigation()
{
    $("nav").children().removeWithFadeOut(500);
    navigationShowed = false;
}

function showSite(navBar)
{
    let url = NavBarData[navBar].url;
    showSiteUrl(url, navBar);
}

function showSiteUrl(url, navBarId=null, changeUrl = true)
{
    let urlMini = url+"?mini=true";
    let getUrlData = url+"?data=true";

    const content = $("#content")
    //content.html("");
    //content.children().removeWithFadeOut(500);
    content.removeClass("fade-in");
    content.addClass("fade-out");
    setTimeout(()=> {
        content.html("");
    },300);
    $.post(getUrlData, {}, function (info) {
        const json = getJson(info);
        if(json!==false)
        {
            changePageTitle(json.title);
            if(changeUrl)
                changeUrlWithoutReload(url);
            if(json.showNav !== true)
                hideNavigation();
            else
                showNavigation();
        }
    });
    let ajaxTime= new Date().getTime();
    $.post(urlMini, {}, function (info) {
        let totalTime = new Date().getTime()-ajaxTime;
        let timeWait = 350-totalTime;
        if(timeWait < 0)
            timeWait = 0;
        setTimeout(()=> {
            content.html(info);
            content.removeClass("fade-out");
            content.addClass("fade-in");
        },timeWait);
    });

    if(navBarId!==null)
        navigation(navBarId);
}
function changePageTitle(title)
{
    document.title = title;
}
function changeUrlWithoutReload(urlPath){
    history.pushState({}, null, urlPath);
}