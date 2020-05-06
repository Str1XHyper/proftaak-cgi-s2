using System;
using System.Collections.Generic;
using System.Text;
using Models.Settings;

namespace ClassLibrary.Classes
{
    public class GetPageInformation
    {
        public PageModel GetPages(string rol)
        {
            PageModel model = new PageModel();
            List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `Rollen` WHERE `Naam` = '{rol}'");
            if(response.Count > 0)
            {
                model.AmountPages = response[0].Length - 1;
                model.Permissions = response[0];
                model.PageNames = SQLConnection.ExecuteSearchQuery($"SELECT Column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Rollen'").ToArray();
                model.Role = rol;
            }

            return model;
        }

        public List<string> GetRoles()
        {
            List<string> response = SQLConnection.ExecuteSearchQuery($"SELECT `naam` FROM `Rollen`");
            return response;
        }

        public void InsertNewPermissions(string rol, List<int> permissions, List<string> pages)
        {
            string sql  = string.Empty;
            List<string> final = new List<string>();
            for(int i = 0; i < pages.Count; i++)
            {
                final.Add("`" + pages[i] + "` = '" + permissions[i] + "'");
            }
            sql = string.Join(",", final);
            sql = $"UPDATE Rollen SET " + sql + "WHERE Naam = '" + rol + "'";
            SQLConnection.ExecuteNonSearchQuery(sql);
        }
    }
}
