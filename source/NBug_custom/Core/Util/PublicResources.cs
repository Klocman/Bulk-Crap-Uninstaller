// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicResources.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NBug.Properties;

namespace NBug.Core.Util
{
    public class PublicResources
    {
        private string ui_Console_Full_Message;
        private string ui_Console_Minimal_Message;
        private string ui_Console_Normal_Message;
        private string ui_Dialog_Full_Exception_Tab;
        private string ui_Dialog_Full_General_Tab;
        private string ui_Dialog_Full_How_to_Reproduce_the_Error_Notification;
        private string ui_Dialog_Full_Message;
        private string ui_Dialog_Full_Quit_Button;
        private string ui_Dialog_Full_Report_Contents_Tab;
        private string ui_Dialog_Full_Send_and_Quit_Button;
        private string ui_Dialog_Full_Title;
        private string ui_Dialog_Minimal_Message;
        private string ui_Dialog_Normal_Continue_Button;
        private string ui_Dialog_Normal_Message;
        private string ui_Dialog_Normal_Quit_Button;
        private string ui_Dialog_Normal_Title;

        public string UI_Console_Full_Message
        {
            get { return ui_Console_Full_Message ?? Localization.UI_Console_Full_Message; }

            set { ui_Console_Full_Message = value; }
        }

        public string UI_Console_Minimal_Message
        {
            get { return ui_Console_Minimal_Message ?? Localization.UI_Console_Minimal_Message; }

            set { ui_Console_Minimal_Message = value; }
        }

        public string UI_Console_Normal_Message
        {
            get { return ui_Console_Normal_Message ?? Localization.UI_Console_Normal_Message; }

            set { ui_Console_Normal_Message = value; }
        }

        public string UI_Dialog_Full_Exception_Tab
        {
            get { return ui_Dialog_Full_Exception_Tab ?? Localization.UI_Dialog_Full_Exception_Tab; }

            set { ui_Dialog_Full_Exception_Tab = value; }
        }

        public string UI_Dialog_Full_General_Tab
        {
            get { return ui_Dialog_Full_General_Tab ?? Localization.UI_Dialog_Full_General_Tab; }

            set { ui_Dialog_Full_General_Tab = value; }
        }

        public string UI_Dialog_Full_How_to_Reproduce_the_Error_Notification
        {
            get
            {
                return ui_Dialog_Full_How_to_Reproduce_the_Error_Notification ??
                       Localization.UI_Dialog_Full_How_to_Reproduce_the_Error_Notification;
            }

            set { ui_Dialog_Full_How_to_Reproduce_the_Error_Notification = value; }
        }

        public string UI_Dialog_Full_Message
        {
            get { return ui_Dialog_Full_Message ?? Localization.UI_Dialog_Full_Message; }

            set { ui_Dialog_Full_Message = value; }
        }

        public string UI_Dialog_Full_Quit_Button
        {
            get { return ui_Dialog_Full_Quit_Button ?? Localization.UI_Dialog_Full_Quit_Button; }

            set { ui_Dialog_Full_Quit_Button = value; }
        }

        public string UI_Dialog_Full_Report_Contents_Tab
        {
            get { return ui_Dialog_Full_Report_Contents_Tab ?? Localization.UI_Dialog_Full_Report_Contents_Tab; }

            set { ui_Dialog_Full_Report_Contents_Tab = value; }
        }

        public string UI_Dialog_Full_Send_and_Quit_Button
        {
            get { return ui_Dialog_Full_Send_and_Quit_Button ?? Localization.UI_Dialog_Full_Send_and_Quit_Button; }

            set { ui_Dialog_Full_Send_and_Quit_Button = value; }
        }

        public string UI_Dialog_Full_Title
        {
            get { return ui_Dialog_Full_Title ?? Localization.UI_Dialog_Full_Title; }

            set { ui_Dialog_Full_Title = value; }
        }

        public string UI_Dialog_Minimal_Message
        {
            get { return ui_Dialog_Minimal_Message ?? Localization.UI_Dialog_Minimal_Message; }

            set { ui_Dialog_Minimal_Message = value; }
        }

        public string UI_Dialog_Normal_Continue_Button
        {
            get { return ui_Dialog_Normal_Continue_Button ?? Localization.UI_Dialog_Normal_Continue_Button; }

            set { ui_Dialog_Normal_Continue_Button = value; }
        }

        public string UI_Dialog_Normal_Message
        {
            get { return ui_Dialog_Normal_Message ?? Localization.UI_Dialog_Normal_Message; }

            set { ui_Dialog_Normal_Message = value; }
        }

        public string UI_Dialog_Normal_Quit_Button
        {
            get { return ui_Dialog_Normal_Quit_Button ?? Localization.UI_Dialog_Normal_Quit_Button; }

            set { ui_Dialog_Normal_Quit_Button = value; }
        }

        public string UI_Dialog_Normal_Title
        {
            get { return ui_Dialog_Normal_Title ?? Localization.UI_Dialog_Normal_Title; }

            set { ui_Dialog_Normal_Title = value; }
        }
    }
}