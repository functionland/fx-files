function OnScrollEvent() {
    const artifactListDiv = document.querySelector('.list-container');
    artifactListDiv.scrollTop = 0;
}

let oldScrollY = 0;
function OnScrollCheck() {
    const artifactListDiv = document.querySelector('.list-container');
    console.log(oldScrollY, artifactListDiv.scrollTop);

    artifactListDiv.addEventListener("scroll", () => {
        const pinListDiv = document.querySelector('.pin-artifacts');
        if (oldScrollY < artifactListDiv.scrollTop && artifactListDiv.scrollTop >= 100) {
            CheckBackToTopButtonDisplay();

            pinListDiv.classList.add('pin-artifacts-hide');
            pinListDiv.classList.remove('pin-artifacts-show');
            console.log("down");
        } else if (oldScrollY > artifactListDiv.scrollTop) {

            CheckBackToTopButtonDisplay();
            pinListDiv.classList.add('pin-artifacts-show');
            pinListDiv.classList.remove('pin-artifacts-hide');
            console.log("up");
        }
        //if (artifactListDiv.scrollTop === 0) {
        //    CheckBackToTopButtonDisplay();
        //    pinListDiv.classList.remove('pin-artifacts-hide');
        //    pinListDiv.classList.add('pin-artifacts-show');
            
        //}
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