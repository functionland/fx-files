function setCodeMirrorText(text) {
    var element = document.getElementById("codeMirrorTextViewer");
    CodeMirror.runMode(text, "application/text", element);
}

var prevDiff = -1;
var fontsize = 10;
var lineHeight = 15;

function registerOnTouchEvent() {
    var el = document.getElementsByClassName("text-container")[0];
    el.addEventListener("touchmove", move_handler);
    el.addEventListener("touchcancel", end_handler);
    el.addEventListener("touchend", end_handler);
    prevDiff = -1;
    fontsize = 10;
    lineHeight = 15;
    document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
    document.getElementById("codeMirrorTextViewer").style.lineHeight = lineHeight + 'px';
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
                if (fontsize <= 30) {
                    fontsize += 1;
                    lineHeight += 1;
                    document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
                    document.getElementById("codeMirrorTextViewer").style.lineHeight = lineHeight + 'px';

                }
            }
            if (curDiff < prevDiff) {

                if (fontsize >= 5) {
                    fontsize -= 1;
                    lineHeight -= 1;
                    document.getElementById("codeMirrorTextViewer").style.fontSize = fontsize + 'px';
                    document.getElementById("codeMirrorTextViewer").style.lineHeight = lineHeight + 'px';

                }
            }
        }

        prevDiff = curDiff;
    }
}
