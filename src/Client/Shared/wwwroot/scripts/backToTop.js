function OnScrollEvent() {
    document.documentElement.scrollTop = 0;
}

window.onscroll = function () { OnScrollCheck() };

function OnScrollCheck() {
    if (document.documentElement.scrollTop > 85) {
        var scrollButton = document.getElementsByClassName('position-scroll-btn')[0];
        scrollButton.style.display = 'block';
    }
    else {
        var scrollButton = document.getElementsByClassName('position-scroll-btn')[0];
        scrollButton.style.display = 'none';
    }
}