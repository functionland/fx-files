function setCodeMirrorText(text, fileName) {
    var element = document.getElementById("codeMirrorTextViewer");
    var mode = CodeMirror.findModeByFileName(fileName);
    CodeMirror.runMode(text, mode.mime, element);
}

var prevDiff = -1;
var fontsize = 14;
var lineHeight = 24;

function setupCodeMirror() {
    var el = document.getElementsByClassName("text-container")[0];
    el.addEventListener("touchmove", move_handler);
    el.addEventListener("touchcancel", end_handler);
    el.addEventListener("touchend", end_handler);
    prevDiff = -1;
    fontsize = 14;
    lineHeight = 24;
    var codeMirroElement = document.getElementById("codeMirrorTextViewer");
    codeMirroElement.style.fontSize = fontsize + 'px';
    codeMirroElement.style.lineHeight = lineHeight + 'px'; 
}

function unRegisterOnTouchEvent() {
    var el = document.getElementsByClassName("text-container")[0];
    el.removeEventListener("touchmove", move_handler);
    el.removeEventListener("touchcancel", end_handler);
    el.removeEventListener("touchend", end_handler);
}

function move_handler(ev) {
    if (ev.changedTouches.length === 2) {
        ev.preventDefault();
        handle_pinch_zoom(ev);
    }
}

function end_handler(ev) {
    if (ev.changedTouches.length === 2) {
        ev.preventDefault();
        prevDiff = -1;
    }
}

function handle_pinch_zoom(ev) {
    if (ev.changedTouches.length === 2) {
        const diffX = Math.abs(ev.changedTouches[0].clientX - ev.changedTouches[1].clientX);
        const diffY = Math.abs(ev.changedTouches[0].clientX - ev.changedTouches[1].clientX);
        var curDiff = Math.sqrt(diffX) + Math.sqrt(diffY);

        if (prevDiff > 0) {
            if (curDiff > prevDiff) {
                increaseFont();
            }
            if (curDiff < prevDiff) {

                decreaseFont();
            }
        }

        prevDiff = curDiff;
    }
}

function decreaseFont() {
    if (fontsize >= 5) {
        fontsize -= 1;
        lineHeight -= 1;
        document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
        document.getElementById("codeMirrorTextViewer").style.lineHeight = lineHeight + 'px';

    }
}

function increaseFont() {
    if (fontsize <= 30) {
        fontsize += 1;
        lineHeight += 1;
        document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
        document.getElementById("codeMirrorTextViewer").style.lineHeight = lineHeight + 'px';

    }
}
