let windowEventListeners = {};

function AddWindowWidthListener(objReference, id) {
    let eventListener = () => UpdateWindowWidth(objReference);
    window.addEventListener("resize", eventListener);
    windowEventListeners[id] = eventListener;
}

function RemoveWindowWidthListener(id) {
    window.removeEventListener("resize", windowEventListeners[id]);
    delete windowEventListeners[id];
}

function UpdateWindowWidth(objReference) {
    objReference.invokeMethodAsync("UpdateWindowWidth", window.innerWidth);
}