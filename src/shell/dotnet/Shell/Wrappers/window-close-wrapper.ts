window.close = function () {
    window?.chrome?.webview?.postMessage("closeWindow");
    window.close();
}