using DAL.Verlof;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Verlof
{
    public class VerlofManager
    {
        private readonly VerlofHandler verlofHandler;
        public VerlofManager()
        {
            verlofHandler = new VerlofHandler();
        }
        public void Afwijzen(string verlofID) => verlofHandler.Afwijzen(verlofID);
        public void Goedkeuren(string verlofID) => verlofHandler.Goedkeuren(verlofID);
        public List<string[]> VerlofverzoekenOphalen()
        {
            List<List<string[]>> data = verlofHandler.GegevensOphalen();
            List<string[]> verzoeken = data[0];
            List<string[]> werknemers = data[1];
            List<string[]> verzoekGegevens = data[2];
            List<string[]> RequestData = new List<string[]>();
            foreach (string[] request in verzoeken)
            {
                foreach (string[] name in werknemers)
                {
                    if (request[1] == name[0])
                    {
                        string[] temp = new string[request.Length + 4];
                        if (name[2] != string.Empty)
                        {
                            temp[0] = name[1] + " " + name[2] + " " + name[3];
                        }
                        else
                        {
                            temp[0] = name[1] + " " + name[3];
                        }
                        for (int i = 1; i <= request.Length; i++)
                        {
                            temp[i] = request[i - 1];
                        }
                        foreach (string[] array in verzoekGegevens)
                        {
                            if (request[2] == array[0])
                            {
                                temp[temp.Length - 3] = array[1];
                                temp[temp.Length - 2] = array[2];
                                temp[temp.Length - 1] = array[3];
                                verzoekGegevens.Remove(array);
                                break;
                            }
                        }
                        RequestData.Add(temp);
                    }
                }
            }
            return RequestData;
        }
    }
}
