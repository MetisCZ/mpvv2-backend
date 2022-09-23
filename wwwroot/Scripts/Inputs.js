/* Adapted from http://thecodeplayer.com/walkthrough/ripple-click-effect-google-material-design */
"use strict";
document.getScroll = function() {
    if (window.pageYOffset != undefined) {
        return [pageXOffset, pageYOffset];
    } else {
        var sx, sy, d = document,
            r = d.documentElement,
            b = d.body;
        sx = r.scrollLeft || b.scrollLeft || 0;
        sy = r.scrollTop || b.scrollTop || 0;
        return [sx, sy];
    }
}

function animate(e) {
    if(this.disabled)
        return;

    const parent = this;

    if (parent.querySelectorAll(".ink").length === 0) {
        const span = document.createElement("span");
        span.classList.add("ink");
        parent.insertBefore(span, parent.firstChild);
    }

    const ink = parent.querySelectorAll(".ink")[0];

    ink.classList.remove("animate");

    if (!ink.offsetHeight && !ink.offsetWidth) {
        const d = Math.max(parent.offsetHeight, parent.offsetWidth);
        ink.style.height = `${d}px`;
        ink.style.width = `${d}px`;
    }

    const rect = parent.getBoundingClientRect();
    const scroll = document.getScroll();

    const offset = {
        top: rect.top + scroll[1],
        left: rect.left + scroll[0]
    }

    const x = e.pageX - (offset.left) - ink.offsetWidth / 2;
    const y = e.pageY - (offset.top) - ink.offsetHeight / 2;


    ink.style.top = `${y}px`;
    ink.style.left = `${x}px`;
    ink.classList.add("animate");
}

function showButtonLoading(jQuerySelector)
{
    let black = false;
    if(jQuerySelector.hasClass("btn-yellow"))
        black = true;
    jQuerySelector.prop("disabled", true);
    jQuerySelector.append('<span class="l-loader-small '+((black)?"l-loader-black":"")+'"><span class="l-loader-inner '+((black)?"l-loader-inner-black":"")+'"></span></span>')
}
function removeButtonLoading(jQuerySelector)
{
    jQuerySelector.prop("disabled", false);
    jQuerySelector.children(".l-loader-small").remove();
}

/*parent.addEventListener('click', function(e) {
    if(e.target.classList.contains('btn')) {
        animate(e,e.parentNode);
    }
});*/
/*Array.prototype.forEach.call(document.querySelectorAll('.btn'), function(button) {
    button.addEventListener('click', animate);
});*/
/*$(document).on('click',".btn",(elem) =>
{
    let e = $(elem.target)[0];
    animate(e);
});*/
$(document).on('mousedown',".btn",animate);
/*$(document).ready(()=> {
    const links = document.querySelectorAll(".btn");
    links.forEach(link => link.addEventListener("click", animate));
});*/
