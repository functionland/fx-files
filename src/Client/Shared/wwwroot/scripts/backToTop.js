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
    var searchIcon = document.querySelector('.search-icon');
    if (typeof searchInput !== "undefined") {
        searchInput.focus();
        searchIcon.classList.add("search-icon-active");
        console.log(searchIcon);
    }
}

function ImagePinchZoom() {
    let el = document.querySelector('#image-viewer');
    new PinchZoom.default(el, {
        draggableUnzoomed: false,
        setOffsetsOnce: true,
        maxZoom: 20,
        tapZoomFactor : 3
    });
}

function breadCrumbStyle() {
    let breadcrumbs = document.querySelector(".fx-breadcrumbs");
    let startEllipsis = document.querySelector(".start-ellipsis");
    let endEllipsis = document.querySelector(".end-ellipsis");

    breadcrumbs.addEventListener("scroll", () => {
        let breadcrumbsScroll = breadcrumbs.scrollLeft.toFixed();

        if (breadcrumbsScroll != 0) {
            startEllipsis.style.display = "block";
            endEllipsis.style.display = "block";
            endEllipsis.classList.remove("color-changer");


        } else {
            startEllipsis.style.display = "none";
            endEllipsis.classList.add("color-changer");
        }

        if (breadcrumbs.offsetWidth + breadcrumbs.scrollLeft >= breadcrumbs.scrollWidth - 1) {
            endEllipsis.classList.add("color-changer");
            endEllipsis.style.display = "block";
        }
    });
}