using System;
using System.ComponentModel;
using System.Linq;

namespace Characters
{
    public enum Class 
    {
        [Description("None")]
        None,
        [Description("Ashigaru")]
        Ashigaru,
        [Description("Samurai")]
        Samurai,
        [Description("Hatamoto")]
        Hatamoto,
        [Description("Oni")]
        Oni,
        [Description("Yabusame Archer")]
        YabusameArcher,
        [Description("Ninja")]
        Ninja,
        [Description("Monk")]
        Monk,
        [Description("Ronin")]
        Ronin,
        [Description("Onna-musha")]
        Onna_musha,
        [Description("Onna-bugeisha")]
        Onna_bugeisha,
        [Description("Damiyo")]
        Damiyo,        
        [Description("Great Damiyo")]
        GreatDamiyo

    }

    public class ClassNameParser
    {
        public static string GetDescriptionFromEnum(Enum value)
        {
            DescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
