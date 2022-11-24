namespace Functionland.FxFiles.Client.App.Controls
{
    public class VideoPositionEventArgs : EventArgs
    {
        public TimeSpan Position { get; private set; }

        public VideoPositionEventArgs(TimeSpan position)
        {
            Position = position;
        }
    }
}
