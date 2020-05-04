using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AgendaViewModel
    {
        public List<EventModel> eventList;
        public List<UserViewModel> userList;
        public string loggedInUserId { get; private set; }
        public AgendaViewModel(string loggedInUserId)
        {
            this.loggedInUserId = loggedInUserId;
            eventList = new List<EventModel>();
            userList = new List<UserViewModel>();
        }
    }
}
