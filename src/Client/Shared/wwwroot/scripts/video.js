var FxVideo = (function () {
    var video = null;
    var videoProgress = null;
    var videoDuration = null;
    var videoCurrentTime = null;

    return { initVideo, togglePlay, backward, forward, togglePictureInPicture };

    function initVideo() {
        video = document.getElementById("fxVideoElement");
        videoDuration = document.getElementById("fxVideoDurationText");
        videoProgress = document.getElementById("fxVideoProgressElement");
        videoCurrentTime = document.getElementById("fxVideoCurrentTimeText");

        video.play();

        video.addEventListener("timeupdate", () => {
            videoProgress.value = video.currentTime;
            videoDuration.innerHTML = timeFormat(video.duration);
            videoCurrentTime.innerHTML = timeFormat(video.currentTime);
            videoProgress.setAttribute('max', Math.floor(video.duration));
        });

        videoProgress.addEventListener("input", (event) => {
            video.currentTime = event.target.value;
            videoProgress.value = video.currentTime;
        });
    }

    function timeFormat(seconds) {
        var result = ""

        if (seconds > 3600) {
            result = new Date(seconds * 1000).toISOString().slice(11, 19);
        }
        else {
            result = new Date(seconds * 1000).toISOString().slice(14, 19);
        }

        return result;
    }

    function togglePlay() {
        if (video.paused || video.ended) {
            video.play();
        } else {
            video.pause();
        }
    }

    function backward() {
        video.currentTime = video.currentTime - 15;
    }

    function forward() {
        video.currentTime = video.currentTime + 15;
    }

    function togglePictureInPicture() {
        if (document.pictureInPictureElement) {
            document.exitPictureInPicture();
        } else if (document.pictureInPictureEnabled) {
            video.requestPictureInPicture();
        }
    }
}());