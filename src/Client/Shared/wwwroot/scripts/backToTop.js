let timeoutID;
let oldScrollY = 0;

function OnScrollEvent() {
    const artifactListDiv = document.querySelector('.list-container');
    artifactListDiv.scrollTop = 0;
}


function HideBackToTopButton() {
    {
        var scrollButton = document.getElementsByClassName('position-scroll-btn')[0];
        scrollButton.style.display = 'none';
        if (typeof timeoutID != undefined) {
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

function getLastScrollPosition() {
    const artifactListDiv = document.querySelector('.list-container');
    let lastScrollPosition = savePositionScroll[savePositionScroll.length - 1];
    artifactListDiv.scrollTop = lastScrollPosition;
    savePositionScroll.pop();
}

function saveScrollPosition() {
    const artifactListDiv = document.querySelector('.list-container');
    savePositionScroll.push(artifactListDiv.scrollTop);
}

function SearchInputUnFocus() {
    var searchInput = document.getElementById('searchinput');
    var searchIcon = document.querySelector('.search-icon');
    if (typeof searchInput != undefined) {
        searchInput.blur();
        searchIcon.classList.remove("search-icon-active");
    }
}

function ImagePinchZoom() {
    let el = document.querySelector('#image-viewer');
    new PinchZoom.default(el, {
        draggableUnzoomed: false,
        setOffsetsOnce: true,
        maxZoom: 20,
        tapZoomFactor: 3
    });
}

function breadCrumbStyle() {
    let breadcrumbs = document.querySelector(".fx-breadcrumbs");

    if (!breadcrumbs)
        return;

    let startEllipsis = document.querySelector(".start-ellipsis");
    let endEllipsis = document.querySelector(".end-ellipsis");
    let hasHorizontalScrollbar = breadcrumbs.scrollWidth > breadcrumbs.clientWidth;

    if (hasHorizontalScrollbar) {
        startEllipsis.style.display = "block";
        breadcrumbs.scrollLeft = breadcrumbs.scrollWidth + breadcrumbs.scrollLeft;
    }

    breadcrumbs.addEventListener("scroll", () => {
        startEllipsis.style.display = "block";
        let breadcrumbsScroll = breadcrumbs.scrollLeft.toFixed();

        if (breadcrumbsScroll != 0) {
            startEllipsis.classList.add("color-changer");
            startEllipsis.style.display = "block";
            endEllipsis.classList.add("color-changer");

        } else {
            startEllipsis.classList.remove("color-changer");
            startEllipsis.style.display = "none";
        }

        if (breadcrumbs.offsetWidth + breadcrumbs.scrollLeft >= breadcrumbs.scrollWidth - 1) {
            endEllipsis.classList.remove("color-changer");
        }
    });
}

function breadCrumbStyleSelectionModal() {
    let breadcrumbs = document.querySelector(".sheet-wrapper .fx-breadcrumbs");

    if (!breadcrumbs)
        return;

    let startEllipsis = document.querySelector(".sheet-wrapper .start-ellipsis");
    let endEllipsis = document.querySelector(".sheet-wrapper .end-ellipsis");
    let hasHorizontalScrollbar = breadcrumbs.scrollWidth > breadcrumbs.clientWidth;

    if (hasHorizontalScrollbar) {
        startEllipsis.style.display = "block";
        breadcrumbs.scrollLeft = breadcrumbs.scrollWidth + breadcrumbs.scrollLeft;
    }

    breadcrumbs.addEventListener("scroll", () => {
        startEllipsis.style.display = "block";
        let breadcrumbsScroll = breadcrumbs.scrollLeft.toFixed();

        if (breadcrumbsScroll != 0) {
            startEllipsis.classList.add("color-changer");
            startEllipsis.style.display = "block";
            endEllipsis.classList.add("color-changer");

        } else {
            startEllipsis.classList.remove("color-changer");
            startEllipsis.style.display = "none";
        }

        if (breadcrumbs.offsetWidth + breadcrumbs.scrollLeft >= breadcrumbs.scrollWidth - 1) {
            endEllipsis.classList.remove("color-changer");
        }
    });
}


function OnScrollCheck() {
    const artifactListDiv = document.querySelector('.list-container');

    artifactListDiv.addEventListener("scroll", () => {
        const pinListDiv = document.querySelector('.pin-artifacts');
        if (typeof pinListDiv != undefined && pinListDiv !== null) {

            if (oldScrollY < artifactListDiv.scrollTop && artifactListDiv.scrollTop >= 350) {

                pinListDiv.classList.add('pin-artifacts-hide');
                pinListDiv.classList.remove('pin-artifacts-show');
                console.log("down");

            } else if (oldScrollY > artifactListDiv.scrollTop) {

                pinListDiv.classList.add('pin-artifacts-show');
                pinListDiv.classList.remove('pin-artifacts-hide');
                console.log("up");
            }

            oldScrollY = artifactListDiv.scrollTop;
        }

        if (artifactListDiv.scrollTop > 85) {
            ShowBackToTopButton();

            if (typeof timeoutID === 'undefined') {
                timeoutID = setTimeout(HideBackToTopButton, 3000);
            }
        } else {
            HideBackToTopButton();
            if (typeof timeoutID != undefined) {
                clearTimeout(timeoutID);
                timeoutID = undefined;
            }
        }
    });
}

function scrollToItem(itemId, listHeight) {
    let item = document.getElementById(itemId.toString());
    console.log(item);
    console.log(itemId);
    console.log(listHeight);
    let list = document.querySelector('.list-container');
    if (typeof list !== 'undefined' || list !== null) {
        list.scrollTop = listHeight;
        if (typeof item !== 'undefined' || item !== null) {
            addGrayBackground(item);
            item = null;
            return true;
        }
        return false;
    }
}

function addGrayBackground(item) {
    item.classList.add('on-scroll-item-gray-background');
}