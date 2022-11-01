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

// array for store scroll top value for back button
let savePositionScroll = [];

function getLastScrollPossition() {
    let lastScrollPosition = savePositionScroll[savePositionScroll.length - 1];
    document.documentElement.scrollTop = lastScrollPosition;
    savePositionScroll.pop();
}

function saveScrollPosition() {
    savePositionScroll.push(document.documentElement.scrollTop);
}

// search input focus 
function SearchInputFocus() {
    var searchInput = document.getElementById('searchinput');
    if (typeof searchInput !== "undefined") {
        searchInput.focus();
    }
}

function Test() {
    let el = document.querySelector('#image-viewer');
    new PinchZoom.default(el, {
        draggableUnzoomed: false,
        setOffsetsOnce: true,
        maxZoom: 20,
        tapZoomFactor : 3
    });
}