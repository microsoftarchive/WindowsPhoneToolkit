using System;
using System.Collections.Generic;

namespace Weather
{
    public class Condition
    {
        // folder in which we store all icons
        private const string IconFolder = "/Weather/Icons";

        public int Code { get; set; }
        public string Desc { get; set; }
        public string IconDay { get; set; }
        public string IconNight { get; set; }

        public static IDictionary<int, Condition> All { get; private set; }

        static Condition()
        {
            string AsmName = typeof(Condition).Assembly.FullName;
            string Module = AsmName.Substring(0, AsmName.IndexOf(','));
            string fmt = string.Format("/{0};component{1}/{{0}}.png", Module, IconFolder);

            All = new Dictionary<int, Condition>();
            All.Add(395, new Condition() { Code = 395, Desc = "Moderate or heavy snow in area with thunder", IconDay = string.Format(fmt, "wsymbol_0012_heavy_snow_showers"), IconNight = string.Format(fmt, "wsymbol_0028_heavy_snow_showers_night") });
            All.Add(392, new Condition() { Code = 392, Desc = "Patchy light snow in area with thunder", IconDay = string.Format(fmt, "wsymbol_0016_thundery_showers"), IconNight = string.Format(fmt, "wsymbol_0032_thundery_showers_night") });
            All.Add(389, new Condition() { Code = 389, Desc = "Moderate or heavy rain in area with thunder", IconDay = string.Format(fmt, "wsymbol_0024_thunderstorms"), IconNight = string.Format(fmt, "wsymbol_0040_thunderstorms_night") });
            All.Add(386, new Condition() { Code = 386, Desc = "Patchy light rain in area with thunder", IconDay = string.Format(fmt, "wsymbol_0016_thundery_showers"), IconNight = string.Format(fmt, "wsymbol_0032_thundery_showers_night") });
            All.Add(377, new Condition() { Code = 377, Desc = "Moderate or heavy showers of ice pellets", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(374, new Condition() { Code = 374, Desc = "Light showers of ice pellets", IconDay = string.Format(fmt, "wsymbol_0013_sleet_showers"), IconNight = string.Format(fmt, "wsymbol_0029_sleet_showers_night") });
            All.Add(371, new Condition() { Code = 371, Desc = "Moderate or heavy snow showers", IconDay = string.Format(fmt, "wsymbol_0012_heavy_snow_showers"), IconNight = string.Format(fmt, "wsymbol_0028_heavy_snow_showers_night") });
            All.Add(368, new Condition() { Code = 368, Desc = "Light snow showers", IconDay = string.Format(fmt, "wsymbol_0011_light_snow_showers"), IconNight = string.Format(fmt, "wsymbol_0027_light_snow_showers_night") });
            All.Add(365, new Condition() { Code = 365, Desc = "Moderate or heavy sleet showers", IconDay = string.Format(fmt, "wsymbol_0013_sleet_showers"), IconNight = string.Format(fmt, "wsymbol_0029_sleet_showers_night") });
            All.Add(362, new Condition() { Code = 362, Desc = "Light sleet showers", IconDay = string.Format(fmt, "wsymbol_0013_sleet_showers"), IconNight = string.Format(fmt, "wsymbol_0029_sleet_showers_night") });
            All.Add(359, new Condition() { Code = 359, Desc = "Torrential rain shower", IconDay = string.Format(fmt, "wsymbol_0018_cloudy_with_heavy_rain"), IconNight = string.Format(fmt, "wsymbol_0034_cloudy_with_heavy_rain_night") });
            All.Add(356, new Condition() { Code = 356, Desc = "Moderate or heavy rain shower", IconDay = string.Format(fmt, "wsymbol_0010_heavy_rain_showers"), IconNight = string.Format(fmt, "wsymbol_0026_heavy_rain_showers_night") });
            All.Add(353, new Condition() { Code = 353, Desc = "Light rain shower", IconDay = string.Format(fmt, "wsymbol_0009_light_rain_showers"), IconNight = string.Format(fmt, "wsymbol_0025_light_rain_showers_night") });
            All.Add(350, new Condition() { Code = 350, Desc = "Ice pellets", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(338, new Condition() { Code = 338, Desc = "Heavy snow", IconDay = string.Format(fmt, "wsymbol_0020_cloudy_with_heavy_snow"), IconNight = string.Format(fmt, "wsymbol_0036_cloudy_with_heavy_snow_night") });
            All.Add(335, new Condition() { Code = 335, Desc = "Patchy heavy snow", IconDay = string.Format(fmt, "wsymbol_0012_heavy_snow_showers"), IconNight = string.Format(fmt, "wsymbol_0028_heavy_snow_showers_night") });
            All.Add(332, new Condition() { Code = 332, Desc = "Moderate snow", IconDay = string.Format(fmt, "wsymbol_0020_cloudy_with_heavy_snow"), IconNight = string.Format(fmt, "wsymbol_0036_cloudy_with_heavy_snow_night") });
            All.Add(329, new Condition() { Code = 329, Desc = "Patchy moderate snow", IconDay = string.Format(fmt, "wsymbol_0020_cloudy_with_heavy_snow"), IconNight = string.Format(fmt, "wsymbol_0036_cloudy_with_heavy_snow_night") });
            All.Add(326, new Condition() { Code = 326, Desc = "Light snow", IconDay = string.Format(fmt, "wsymbol_0011_light_snow_showers"), IconNight = string.Format(fmt, "wsymbol_0027_light_snow_showers_night") });
            All.Add(323, new Condition() { Code = 323, Desc = "Patchy light snow", IconDay = string.Format(fmt, "wsymbol_0011_light_snow_showers"), IconNight = string.Format(fmt, "wsymbol_0027_light_snow_showers_night") });
            All.Add(320, new Condition() { Code = 320, Desc = "Moderate or heavy sleet", IconDay = string.Format(fmt, "wsymbol_0019_cloudy_with_light_snow"), IconNight = string.Format(fmt, "wsymbol_0035_cloudy_with_light_snow_night") });
            All.Add(317, new Condition() { Code = 317, Desc = "Light sleet", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(314, new Condition() { Code = 314, Desc = "Moderate or Heavy freezing rain", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(311, new Condition() { Code = 311, Desc = "Light freezing rain", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(308, new Condition() { Code = 308, Desc = "Heavy rain", IconDay = string.Format(fmt, "wsymbol_0018_cloudy_with_heavy_rain"), IconNight = string.Format(fmt, "wsymbol_0034_cloudy_with_heavy_rain_night") });
            All.Add(305, new Condition() { Code = 305, Desc = "Heavy rain at times", IconDay = string.Format(fmt, "wsymbol_0010_heavy_rain_showers"), IconNight = string.Format(fmt, "wsymbol_0026_heavy_rain_showers_night") });
            All.Add(302, new Condition() { Code = 302, Desc = "Moderate rain", IconDay = string.Format(fmt, "wsymbol_0018_cloudy_with_heavy_rain"), IconNight = string.Format(fmt, "wsymbol_0034_cloudy_with_heavy_rain_night") });
            All.Add(299, new Condition() { Code = 299, Desc = "Modearte rain at times", IconDay = string.Format(fmt, "wsymbol_0010_heavy_rain_showers"), IconNight = string.Format(fmt, "wsymbol_0026_heavy_rain_showers_night") });
            All.Add(296, new Condition() { Code = 296, Desc = "Light rain", IconDay = string.Format(fmt, "wsymbol_0018_cloudy_with_heavy_rain"), IconNight = string.Format(fmt, "wsymbol_0034_cloudy_with_heavy_rain_night") });
            All.Add(293, new Condition() { Code = 293, Desc = "Patchy light rain", IconDay = string.Format(fmt, "wsymbol_0017_cloudy_with_light_rain"), IconNight = string.Format(fmt, "wsymbol_0033_cloudy_with_light_rain_night") });
            All.Add(284, new Condition() { Code = 284, Desc = "Heavy freezing drizzle", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(281, new Condition() { Code = 281, Desc = "Freezing drizzle", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(266, new Condition() { Code = 266, Desc = "Light drizzle", IconDay = string.Format(fmt, "wsymbol_0017_cloudy_with_light_rain"), IconNight = string.Format(fmt, "wsymbol_0033_cloudy_with_light_rain_night") });
            All.Add(263, new Condition() { Code = 263, Desc = "Patchy light drizzle", IconDay = string.Format(fmt, "wsymbol_0009_light_rain_showers"), IconNight = string.Format(fmt, "wsymbol_0025_light_rain_showers_night") });
            All.Add(260, new Condition() { Code = 260, Desc = "Freezing fog", IconDay = string.Format(fmt, "wsymbol_0007_fog"), IconNight = string.Format(fmt, "wsymbol_0007_fog") });
            All.Add(248, new Condition() { Code = 248, Desc = "Fog", IconDay = string.Format(fmt, "wsymbol_0007_fog"), IconNight = string.Format(fmt, "wsymbol_0007_fog") });
            All.Add(230, new Condition() { Code = 230, Desc = "Blizzard", IconDay = string.Format(fmt, "wsymbol_0020_cloudy_with_heavy_snow"), IconNight = string.Format(fmt, "wsymbol_0036_cloudy_with_heavy_snow_night") });
            All.Add(227, new Condition() { Code = 227, Desc = "Blowing snow", IconDay = string.Format(fmt, "wsymbol_0019_cloudy_with_light_snow"), IconNight = string.Format(fmt, "wsymbol_0035_cloudy_with_light_snow_night") });
            All.Add(200, new Condition() { Code = 200, Desc = "Thundery outbreaks in nearby", IconDay = string.Format(fmt, "wsymbol_0016_thundery_showers"), IconNight = string.Format(fmt, "wsymbol_0032_thundery_showers_night") });
            All.Add(185, new Condition() { Code = 185, Desc = "Patchy freezing drizzle nearby", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(182, new Condition() { Code = 182, Desc = "Patchy sleet nearby", IconDay = string.Format(fmt, "wsymbol_0021_cloudy_with_sleet"), IconNight = string.Format(fmt, "wsymbol_0037_cloudy_with_sleet_night") });
            All.Add(179, new Condition() { Code = 179, Desc = "Patchy snow nearby", IconDay = string.Format(fmt, "wsymbol_0013_sleet_showers"), IconNight = string.Format(fmt, "wsymbol_0029_sleet_showers_night") });
            All.Add(176, new Condition() { Code = 176, Desc = "Patchy rain nearby", IconDay = string.Format(fmt, "wsymbol_0009_light_rain_showers"), IconNight = string.Format(fmt, "wsymbol_0025_light_rain_showers_night") });
            All.Add(143, new Condition() { Code = 143, Desc = "Mist", IconDay = string.Format(fmt, "wsymbol_0006_mist"), IconNight = string.Format(fmt, "wsymbol_0006_mist") });
            All.Add(122, new Condition() { Code = 122, Desc = "Overcast", IconDay = string.Format(fmt, "wsymbol_0004_black_low_cloud"), IconNight = string.Format(fmt, "wsymbol_0004_black_low_cloud") });
            All.Add(119, new Condition() { Code = 119, Desc = "Cloudy", IconDay = string.Format(fmt, "wsymbol_0003_white_cloud"), IconNight = string.Format(fmt, "wsymbol_0004_black_low_cloud") });
            All.Add(116, new Condition() { Code = 116, Desc = "Partly Cloudy", IconDay = string.Format(fmt, "wsymbol_0002_sunny_intervals"), IconNight = string.Format(fmt, "wsymbol_0008_clear_sky_night") });
            All.Add(113, new Condition() { Code = 113, Desc = "Clear/Sunny", IconDay = string.Format(fmt, "wsymbol_0001_sunny"), IconNight = string.Format(fmt, "wsymbol_0008_clear_sky_night") });
        }
    }
}
