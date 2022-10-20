function OnScrollEvent() {
    const artifactListDiv = document.querySelector('.list-container');
    artifactListDiv.scrollTop = 0;
}

window.OnScrollCheck = OnScrollCheck;

function OnScrollCheck() {
    const artifactListDiv = document.querySelector('.list-container');
    const pinListDiv = document.querySelector('.pin-artifacts');
    let oldScrollY = artifactListDiv.scrollTop;
    artifactListDiv.addEventListener("scroll", () => {
        if (oldScrollY < artifactListDiv.scrollTop) {
            CheckBackToTopButtonDisplay();
            pinListDiv.classList.remove('pin-artifacts-show');
            pinListDiv.classList.remove('pin-artifacts-animation-down');
        } else {
            CheckBackToTopButtonDisplay();
            pinListDiv.classList.add('pin-artifacts-show');
            pinListDiv.classList.remove('pin-artifacts-animation-up');
        }
        oldScrollY = artifactListDiv.scrollTop;
    });
    
}

function CheckBackToTopButtonDisplay() {
    const artifactListDiv = document.querySelector('.list-container');
    if (artifactListDiv.scrollTop > 15) {
        let scrollButton = document.querySelector('.position-scroll-btn');
        scrollButton.style.display = 'block';
    }
    else {
        let scrollButton = document.querySelector('.position-scroll-btn');
        scrollButton.style.display = 'none';
    }
}
