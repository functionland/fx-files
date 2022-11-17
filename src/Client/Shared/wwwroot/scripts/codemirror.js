function setCodeMirrorText(text) {
    var element = document.getElementById("codeMirrorTextViewer");
    CodeMirror.runMode(text, "application/text", element);
}

var prevDiff = -1;
var fontsize = 10;

function registerOnTouchEvent() {
    var el = document.getElementsByClassName("text-container")[0];
    el.addEventListener("touchstart", start_handler);
    el.addEventListener("touchmove", move_handler);
    el.addEventListener("touchcancel", end_handler);
    el.addEventListener("touchend", end_handler);
}

function unRegisterOnTouchEvent() {
    var el = document.getElementsByClassName("text-container")[0];
    el.removeEventListener("touchstart", start_handler);
    el.removeEventListener("touchmove", move_handler);
    el.removeEventListener("touchcancel", end_handler);
    el.removeEventListener("touchend", end_handler);
}

function start_handler(ev) {
    ev.preventDefault();
}

function move_handler(ev) {
    ev.preventDefault();
    handle_pinch_zoom(ev);
}

function end_handler(ev) {
    ev.preventDefault();
    prevDiff = -1;
}


function handle_pinch_zoom(ev) {
    if (ev.changedTouches.length === 2) {

        const diffX = Math.abs(ev.changedTouches[0].clientX - ev.changedTouches[1].clientX);
        const diffY = Math.abs(ev.changedTouches[0].clientX - ev.changedTouches[1].clientX);
        var curDiff = Math.sqrt(diffX) + Math.sqrt(diffY);

        if (prevDiff > 0) {
            if (curDiff > prevDiff) {
                if (fontsize <= 30) {
                    fontsize += 1;
                    document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
                }
            }
            if (curDiff < prevDiff) {
                
                if (fontsize >= 5) {
                    fontsize -= 1;
                    document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
                }
            }
        }

        prevDiff = curDiff;
    }
}

function log(name, ev, printTargetIds) {
    console.log(name, ev, printTargetIds);
}


