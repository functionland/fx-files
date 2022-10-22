function OnScrollEvent() {
    document.documentElement.scrollTop = 0;
}

window.onscroll = function () { OnScrollCheck() };

let timeoutID;
function OnScrollCheck() {
    if (document.documentElement.scrollTop > 85) {
        ShowBackToTopButton();
        if (typeof timeoutID === "undefined") {
            timeoutID = setTimeout(HideBackToTopButton, 3000);
        }
    }
    else {
        HideBackToTopButton();
        if (typeof timeoutID !== "undefined") {
            clearTimeout(timeoutID);
            timeoutID = undefined;
        }
    }
}

function HideBackToTopButton() {
    {
        var scrollButton = document.getElementsByClassName('position-scroll-btn')[0];
        scrollButton.style.display = 'none';
        if (typeof timeoutID !== "undefined") {
            clearTimeout(timeoutID);
            timeoutID = undefined;
        }
    }
}

function ShowBackToTopButton() {
    var scrollButton = document.getElementsByClassName('position-scroll-btn')[0];
    scrollButton.style.display = 'block';
}