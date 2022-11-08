let windowEventListeners = {};

function AddWindowWidthListener(objReference) {
    let eventListener = () => UpdateWindowWidth(objReference);
    window.addEventListener("resize", eventListener);
    windowEventListeners[id] = eventListener;
    var id = Math.floor(Math.random() * 100).toString();
    objReference.invokeMethodAsync("SetResizeEventListenerId", id);
}

function RemoveWindowWidthListener(id) {
    window.removeEventListener("resize", windowEventListeners[id]);
    delete windowEventListeners[id];
}

function UpdateWindowWidth(objReference) {
    objReference.invokeMethodAsync("UpdateWindowWidth", window.innerWidth);
}