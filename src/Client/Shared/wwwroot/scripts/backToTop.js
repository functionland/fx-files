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

// remove bottom navigation from display for search mode
function RemoveBottomNavigation() {
    RemoveMinHeightForSearch();
    var bottomNav = document.querySelector('footer');
    bottomNav.style.display = 'none';
}

// add bottom navigation from display for search
function AddBottomNavigation() {
    AddMinHeightForSearch();
    var bottomNav = document.querySelector('footer');
    bottomNav.style.display = 'block';
}

// remove min height from display for search mode
function RemoveMinHeightForSearch() {
    var main = document.querySelector('.content');
    main.style.minHeight = '100vh';
}

// add min height from display for search
function AddMinHeightForSearch() {
    var main = document.querySelector('.content');
    main.style.minHeight = 'calc(100vh - 81px)';
}