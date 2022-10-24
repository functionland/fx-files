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

        video.addEventListener('durationchange', () => {
            videoProgress.setAttribute('max', Math.round(video.duration));
        });

        video.addEventListener("timeupdate", () => {
            videoProgress.value = video.currentTime;
            videoDuration.innerHTML = fancyTimeFormat(video.duration);
            videoCurrentTime.innerHTML = fancyTimeFormat(video.currentTime);
        });

        videoProgress.addEventListener("input", (event) => {
            video.currentTime = event.target.value;
            videoProgress.value = video.currentTime;
        });
    }

    function fancyTimeFormat(duration) {
        // Hours, minutes and seconds
        var hrs = ~~(Math.round(duration) / 3600);
        var mins = ~~((Math.round(duration) % 3600) / 60);
        var secs = ~~Math.round(duration) % 60;

        // Output like "1:01" or "4:03:59" or "123:03:59"
        var ret = "";

        if (hrs > 0) {
            ret += "" + hrs + ":" + (mins < 10 ? "0" : "");
        }

        ret += "" + mins + ":" + (secs < 10 ? "0" : "");
        ret += "" + secs;

        return ret;
    }

    function togglePlay() {
        if (video.paused || video.ended) {
            video.play();
        } else {
            video.pause();
        }
    }

    // backward the current time
    function backward() {
        video.currentTime = video.currentTime - 15;
    }

    // forward the current time
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