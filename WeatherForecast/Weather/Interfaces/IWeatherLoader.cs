using System;
using System.Threading;

namespace Weather
{
    public delegate void WeatherLoadAsyncHandler(object sender, WeatherLoadAsyncEventArgs e);
    public class WeatherLoadAsyncEventArgs : EventArgs
    {
        public EventWaitHandle WaitHandle;
        public Exception Error;
        public WeatherData Weather;
    }

    public interface IWeatherLoader
    {
        event WeatherLoadAsyncHandler LoadAsyncCompleted;

        string GetUrl(WeatherArea area);
        WeatherData Load(WeatherArea area);
        void LoadAsync(WeatherArea area);
    }
}
