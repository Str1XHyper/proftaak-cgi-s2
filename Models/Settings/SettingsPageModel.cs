using Models.Agenda;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Settings
{
    public class SettingsPageModel
    {
        public PasswordSettingsModel model1 { get; set; } = new PasswordSettingsModel();
        public AgendaSettings model2 { get; set; } = new AgendaSettings();
    }
}
