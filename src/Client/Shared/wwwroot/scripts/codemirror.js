function setCodeMirrorText(text) {
    var element = document.getElementById("codeMirrorTextViewer");
    CodeMirror.runMode(text, "application/text", element);
}