using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class AgendaViewModel
    {
        public List<EventModel> eventList;
        public List<UserViewModel> userList;
        public AgendaViewModel()
        {
            eventList = new List<EventModel>();
            userList = new List<UserViewModel>();
        }
    }
}
