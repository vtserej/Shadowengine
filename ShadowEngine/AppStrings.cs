using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ShadowEngine
{
    public enum Language { English, Spanish };

    public class AppStrings
    {
        Language language = Language.Spanish;
        Dictionary<string, string> stringSpanish = new Dictionary<string, string>();
        Dictionary<string, string> stringEnglish = new Dictionary<string, string>();

        public Language Language
        {
            get { return language; }
            set { language = value; }
        }

        public string GetString(string key)
        {
            if (language == Language.English)
            {
                return stringEnglish[key];
            }
            else
            {
                return stringSpanish[key]; 
            }
        }

        /// <summary>
        /// This function builds the strings according to the language chosen by the user
        /// </summary>
        public void CreateStrings(Dictionary<string, string> appStrings)
        {
            if (language == Language.English)
            {
                stringEnglish = appStrings;
            }
            else
            {
                stringSpanish = appStrings; 
            }
        }

        /// <summary>
        /// This function reads the language set on the config file
        /// </summary>
        public void ReadLanguage()
        {
            try
            {
                string[] lines = File.ReadAllLines("config.ini");
                if (lines[0].IndexOf("English") != -1)
                {
                    Language = Language.English;
                }
                else
                {
                    Language = Language.Spanish;
                }
            }
            catch (Exception)
            {
            }
        }
        
        /// <summary>
        /// This function select a language a write it on the config file
        /// </summary>
        public void SelectLanguage(Language language)
        {
            string[] config = new string[1];
            config[0] = "Language = " + language;
            File.WriteAllLines("config.ini", config);
        }
    }
}
