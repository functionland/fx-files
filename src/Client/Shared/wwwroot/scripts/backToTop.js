function OnScrollEvent() {
    document.documentElement.scrollTop = 0;
}

function OnScrollCheck() {
    if (document.documentElement.scrollTop > 5) {
        var scrollButton = document.getElementsByClassName('position-scroll-btn')[0];
        scrollButton.style.display = 'block';
    }
}